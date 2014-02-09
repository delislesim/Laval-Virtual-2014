#include "finger_finder_thinning/finger_finder.h"

#include <iostream>
#include <opencv2/imgproc/imgproc.hpp>
#include <queue>

#include "finger_finder/segmenter.h"
#include "finger_finder_thinning/canny_contour.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "maths/maths.h"

#include "bitmap_graph/bitmap_run.h"

namespace finger_finder_thinning {

namespace {

const cv::Rect kRegionOfInterest(0, 100, 640, 280);

int IndexOf(int x, int y) {
  return y*640 + x;
}

cv::Point PositionOf(int index, int cols) {
  cv::Point position;
  position.x = index % cols;
  position.y = index / cols;
  return position;
}

class SmallContourRemover {
 public:
  SmallContourRemover(cv::Mat* mat_with_contours_to_remove, int num_pixels)
      : mat_with_contours_to_remove_(mat_with_contours_to_remove),
        num_pixels_(num_pixels) {}

  void ObserveComponentStart() {
    component_indexes_.clear();
  }
  void ObserveComponentEnd() {
    if (component_indexes_.size() < num_pixels_) {
      unsigned char* mat_ptr = mat_with_contours_to_remove_->ptr();
      for (size_t i = 0; i < component_indexes_.size(); ++i) {
        mat_ptr[component_indexes_[i]] = 0;
      }
    }
  }

  void ObserveIntersectionStart(int index) {
    component_indexes_.push_back(index);
  }
  void ObserveIntersectionEnd() {
  }

  void ObservePixel(int index) {
    component_indexes_.push_back(index);
  }
  void ObserveLeaf(int index) {
    component_indexes_.push_back(index);
  }

 private:
  int num_pixels_;
  cv::Mat* mat_with_contours_to_remove_;
  std::vector<int> component_indexes_;
};

const unsigned char kBelongsToComponent = 255;
const unsigned char kAlreadyAdded = 1;

const int kNeighboursOffsets[4][2] = {
  /*{-1, -1},*/ { 0, -1}, /*{ 1, -1},*/
    {-1,  0},               { 1,  0},
  /*{-1,  1},*/ { 0,  1}, /*{ 1,  1}*/
};
const int kNumNeighbours = 4;

class SkeletonConnector {
 public:
  SkeletonConnector(const cv::Mat* distance, const cv::Mat* contours, cv::Mat* skeleton)
      : distance_(distance), contours_(contours), skeleton_(skeleton) {
    skeleton_ptr_ = skeleton_->ptr();
    distance_ptr_ = reinterpret_cast<const float*>(distance_->ptr());
    contours_ptr_ = contours_->ptr();

    component_ = cv::Mat(skeleton->size(), CV_8U);
    component_ptr_ = component_.ptr();
  }

  void ObserveComponentStart() {
    original_leaf_vector_.clear();
    leaf_queue_ = std::priority_queue<LeafInfo>();
    component_ = cv::Scalar(0);
  }
  void ObserveComponentEnd() {
    ConnectComponent();
  }

  void ObserveIntersectionStart(int index) {
    component_ptr_[index] = kBelongsToComponent;
    
    // Toujours ajouter le premier point rencontré.
    if (original_leaf_vector_.empty()) {
      AddLeaf(index, index);
      original_leaf_vector_.push_back(index);
    }
  }

  void ObserveIntersectionEnd() {
  }

  void ObservePixel(int index) {
    component_ptr_[index] = kBelongsToComponent;

    // Toujours ajouter le premier point rencontré.
    if (original_leaf_vector_.empty()) {
      AddLeaf(index, index);
      original_leaf_vector_.push_back(index);
    }
  }

  void ObserveLeaf(int index) {
    AddLeaf(index, index);
    component_ptr_[index] = kBelongsToComponent;
    original_leaf_vector_.push_back(index);
  }

  void DrawConnexions() {
    for (size_t i = 0; i < connexions_.size(); ++i) {
      ConnexionInfo connexion = connexions_[i];
      cv::line(*skeleton_,
               PositionOf(connexion.source, skeleton_->cols),
               PositionOf(connexion.destination, skeleton_->cols),
               cv::Scalar(255), 1);
    }
  }

 private:
  void AddLeaf(int source, int index) {
    // Ajouter la feuille seulement si elle ne fait pas partie du contour
    // ni de la composante, et qu'elle n'a pas déjà été ajoutée.
    if (component_ptr_[index] == 0 && contours_ptr_[index] == 0) {
      leaf_queue_.push(LeafInfo(source, index, distance_ptr_[index]));
      component_ptr_[index] = kAlreadyAdded;
    }
  }

  void ConnectComponent() {
    const int kNumSkeletonConnectorSteps = 400;

    int num_essais = 0;
    while (num_essais < kNumSkeletonConnectorSteps && !leaf_queue_.empty()) {
      // Prendre le point le plus clair rencontré jusqu'ici.
      LeafInfo info = leaf_queue_.top();
      leaf_queue_.pop();

      // Si le point appartient au squelette, mais pas à cette composante,
      // on a une connexion!
      if (skeleton_ptr_[info.index] != 0 && component_ptr_[info.index] != kBelongsToComponent) {
        cv::Point destination(PositionOf(info.index, skeleton_->cols));

        // Essayer de voir s'il n'y aurait pas une autre feuille plus près de la destination.
        int best_distance = maths::DistanceSquare(destination, PositionOf(info.source, skeleton_->cols));
        int best_source = info.source;

        for (size_t i = 0; i < original_leaf_vector_.size(); ++i) {
          cv::Point source(PositionOf(original_leaf_vector_[i], skeleton_->cols));
          int distance = maths::DistanceSquare(destination, source);

          if (distance < best_distance) {
            best_source = original_leaf_vector_[i];
          }
        }

        connexions_.push_back(ConnexionInfo(best_source, info.index));
        break;
      }

      // Ajouter tous les voisins de ce point aux candidats.
      cv::Point position = PositionOf(info.index, skeleton_->cols);
      
      for (int i = 0; i < kNumNeighbours; ++i) {
        cv::Point neighbour_position(position.x + kNeighboursOffsets[i][0],
                                     position.y + kNeighboursOffsets[i][1]);
        if (neighbour_position.x >= 0 && neighbour_position.x < skeleton_->cols &&
            neighbour_position.y >= 0 && neighbour_position.y < skeleton_->rows) {
          AddLeaf(info.source, IndexOf(neighbour_position.x, neighbour_position.y));
        }
      }

      ++num_essais;
    }
  }
  
  struct ConnexionInfo {
    ConnexionInfo(int source, int destination)
        : source(source), destination(destination) {}
    int source;
    int destination;
  };

  struct LeafInfo {
    LeafInfo(int source, int index, float distance)
        : source(source), index(index), distance(distance) {}
    int source;
    int index;
    float distance;

    bool operator<(const LeafInfo& other) const {
      return distance < other.distance;
    }
  };

  cv::Mat* skeleton_;
  unsigned char* skeleton_ptr_;

  const cv::Mat* distance_;
  const float* distance_ptr_;

  const cv::Mat* contours_;
  const unsigned char* contours_ptr_;

  cv::Mat component_;
  unsigned char* component_ptr_;

  std::vector<int> original_leaf_vector_;
  std::priority_queue<LeafInfo> leaf_queue_;

  std::vector<ConnexionInfo> connexions_;
};

void ConnectSkeleton(const cv::Mat& distances, const cv::Mat& contours, cv::Mat* skeleton) {
  assert(distances.type() == CV_32F);
  assert(contours.type() == CV_8U);
  assert(skeleton->type() == CV_8U);

  assert(distances.size() == contours.size());
  assert(distances.size() == skeleton->size());

  // 1. Faire une copie du squelette que le runner pourra utiliser.
  cv::Mat skeleton_copy;
  skeleton->copyTo(skeleton_copy);

  // 2. Parcourir le squelette à l'aide du runner.
  SkeletonConnector connector(&distances, &contours, skeleton);
  bitmap_graph::BitmapRun connector_run;
  connector_run.Run(&skeleton_copy, &connector);

  // 3. Dessiner les connexions trouvées.
  connector.DrawConnexions();
}

}  // namespace

FingerFinder::FingerFinder(int hands_depth, int hands_depth_tolerance)
    : min_hands_depth_(hands_depth - hands_depth_tolerance),
      max_hands_depth_(hands_depth + hands_depth_tolerance) {
}

void FingerFinder::FindFingers(const kinect_wrapper::KinectSensorData& data,
                               cv::Mat* nice_image) {
  assert(nice_image);

  cv::Mat depth_mat;
  cv::Mat color_mat;
  if (!data.QueryDepth(&depth_mat) || !data.QueryColor(&color_mat))
    return;
  
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols == static_cast<int>(kinect_wrapper::kKinectDepthWidth));
  assert(depth_mat.rows == static_cast<int>(kinect_wrapper::kKinectDepthHeight));

  assert(color_mat.type() == CV_8UC4);
  assert(color_mat.cols == static_cast<int>(kinect_wrapper::kKinectColorWidth));
  assert(color_mat.rows == static_cast<int>(kinect_wrapper::kKinectColorHeight));

  // Trouver dans l'image de profondeur des contours de mains à la bonne
  // profondeur.
  std::vector<std::vector<cv::Point> > depth_contours;
  finger_finder::Segmenter(depth_mat, 1, max_hands_depth_,
                           &depth_contours);

  // Convertir les contours trouvés dans l'image de profondeur en
  // coordonnées de l'image couleur.
  kinect_wrapper::KinectSensor* sensor =
      kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0);
  std::vector<std::vector<cv::Point> > depth_contours_color_coordinates;
  for (size_t i = 0; i < depth_contours.size(); ++i) {
    depth_contours_color_coordinates.push_back(std::vector<cv::Point>());
    for (size_t j = 0; j < depth_contours[i].size(); ++j) {
      NUI_DEPTH_IMAGE_POINT depth_point;
      depth_point.x = depth_contours[i][j].x;
      depth_point.y = depth_contours[i][j].y;
      depth_point.depth = depth_mat.at<unsigned short>(depth_contours[i][j]);
      
      NUI_COLOR_IMAGE_POINT color_point;
      sensor->MapDepthPointToColorPoint(depth_point, &color_point);
      depth_contours_color_coordinates[i].push_back(
          cv::Point(color_point.x, color_point.y));      
    }
  }

  // Dessiner les contours trouvés dans l'image de profondeur sur une
  // image binaire (en coordonnées couleur) et dilater le résultat.
  cv::Mat depth_contours_mat = cv::Mat::zeros(color_mat.size(), CV_8U);
  for (size_t i = 0; i < depth_contours_color_coordinates.size(); ++i) {
    const cv::Point* points[1];
    points[0] = &depth_contours_color_coordinates[i][0];
    int num_points = depth_contours_color_coordinates[i].size();
    cv::fillPoly(depth_contours_mat, points, &num_points, 1, cv::Scalar(255));
  }
  cv::Mat dilated_depth_contours_mat;
  cv::dilate(depth_contours_mat, dilated_depth_contours_mat, cv::Mat(), cv::Point(-1, -1), 5);

  // Extraire des contours de l'image de couleur.
  cv::Mat all_contours;
  CannyContour(color_mat, &all_contours);

  // Enlever tous les contours de couleur qui ne sont pas à la bonne profondeur.
  all_contours = all_contours & dilated_depth_contours_mat;

  // Réduire la taille région à traiter.
  all_contours = all_contours(kRegionOfInterest);

  // Enlever les contours trop courts.
  cv::Mat all_contours_copy;
  all_contours.copyTo(all_contours_copy);

  SmallContourRemover contour_remover(&all_contours, 25);
  bitmap_graph::BitmapRun contour_remover_run;
  contour_remover_run.Run(&all_contours_copy, &contour_remover);

  // Inverser les valeurs pour faire plaisir à l'algorithme.
  cv::Mat all_contours_inverse = 255 - all_contours;

  // Calculer la distance de chaque point par rapport au contour le plus proche.
  cv::Mat distance_mat;
  cv::distanceTransform(all_contours_inverse, distance_mat, CV_DIST_L1, 3);

  // Sobel.
  int scale = 1;
  int delta = 0;
  int ddepth = CV_16S;
  cv::Mat grad_x, grad_y;

  /// Gradient X
  //Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat, grad_x, ddepth, 1, 0, 1, scale, delta, cv::BORDER_DEFAULT );

  /// Gradient Y
  //Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat, grad_y, ddepth, 0, 1, 1, scale, delta, cv::BORDER_DEFAULT );

  // Générer une image avec le squelette.
  cv::Mat squelette_mat(all_contours.size(), CV_8U, cv::Scalar(0));
  unsigned char* squelette_run = squelette_mat.ptr();
  short* sobel_x_run = reinterpret_cast<short*>(grad_x.ptr());
  short* sobel_y_run = reinterpret_cast<short*>(grad_y.ptr());

  for (int y = 1; y < all_contours.rows - 1; ++y) {
    for (int x = 1; x < all_contours.cols -1; ++x) {
      int index = IndexOf(x, y);
      short sobel_x = sobel_x_run[index];
      short sobel_y = sobel_y_run[index];
      unsigned char* current_squelette = &squelette_run[index];
      
      bool critical_x = (sobel_x == 0 || (
        sobel_x < 0 && (
          sobel_x_run[IndexOf(x-1, y  )] > 0 ||
          sobel_x_run[IndexOf(x+1, y  )] > 0 ||
          sobel_x_run[IndexOf(x  , y-1)] > 0 ||
          sobel_x_run[IndexOf(x  , y+1)] > 0
        )
      ));

      bool critical_y = (sobel_y == 0 || (
        sobel_y < 0 && (
          sobel_y_run[IndexOf(x-1, y  )] > 0 ||
          sobel_y_run[IndexOf(x+1, y  )] > 0 ||
          sobel_y_run[IndexOf(x  , y-1)] > 0 ||
          sobel_y_run[IndexOf(x  , y+1)] > 0
        )
      ));

      if (all_contours.at<unsigned char>(cv::Point(x, y)) != 0) {
        // On est sur le contour.
        *current_squelette = 0;
      } else if (critical_x && critical_y) {
        *current_squelette = 255;
      } else if (critical_x && abs(sobel_y) < 2) {
        *current_squelette = 255;
      } else if (critical_y && abs(sobel_x) < 2) {
        *current_squelette = 255;
      } else {
        *current_squelette = 0;
      }

    }
  }

  // Enlever les parties du squelette qui ne sont pas à la bonne profondeur.
  // TODO(fdoray): Aussi enlever les squelettes qui touchent trop aux bordures.
  squelette_run = squelette_mat.ptr();
  for (size_t i = 0; i < squelette_mat.total(); ++i) {
    if (depth_contours_mat.ptr()[i + 640*kRegionOfInterest.y] != 255) {
      *squelette_run = 0;
    }
    ++squelette_run;
  }

  // Essayer de compléter les lignes du squelette.
  cv::dilate(all_contours, all_contours, cv::Mat(), cv::Point(-1, -1), 1);
  ConnectSkeleton(distance_mat, all_contours, &squelette_mat);

  // Enlever ce qui est trop petit ou touche au contour et dilater le squelette.
  cv::Mat squelette_mat_copy;
  squelette_mat.copyTo(squelette_mat_copy);

  SmallContourRemover squelette_remover(&squelette_mat, 100);
  bitmap_graph::BitmapRun squelette_remover_run;
  squelette_remover_run.Run(&squelette_mat_copy, &squelette_remover);

  squelette_mat = squelette_mat & (all_contours == 0);

  unsigned char dilater[] = {0, 1, 0, 1, 1, 1, 0, 1, 0};
  cv::Mat rounded_dilater(cv::Size(3, 3), CV_8U, dilater);
  cv::dilate(squelette_mat, squelette_mat, rounded_dilater, cv::Point(-1, -1), 3);

  // Détection de coins.
  int blockSize = 5;
  int apertureSize = 3;
  double k = 0.02;

  cv::Mat corners_mat = cv::Mat::zeros( squelette_mat.size(), CV_32FC1 );

  cv::cornerHarris(squelette_mat, corners_mat, blockSize, apertureSize, k, cv::BORDER_DEFAULT);

  cv::Mat corners_mat_normalized;
  cv::normalize(corners_mat, corners_mat_normalized, 0, 255, cv::NORM_MINMAX, CV_32FC1, cv::Mat());

  // Créer une image pour montrer le résultat.

  cv::Mat distance_mat_char(distance_mat.size(), CV_8U);
  float* distance_src_run = reinterpret_cast<float*>(distance_mat.ptr());
  unsigned char* distance_dst_run = distance_mat_char.ptr();

  for (size_t i = 0; i < distance_mat.total(); ++i) {
    if (*distance_src_run > 127)
      *distance_src_run = 127;

    *distance_dst_run = static_cast<unsigned char>(*distance_src_run * 2);

    ++distance_src_run;
    ++distance_dst_run;
  }


  //cv::cvtColor( distance_mat_char, *nice_image, CV_GRAY2RGBA);
  *nice_image = cv::Mat(squelette_mat.size(), CV_8UC4, cv::Scalar(0));

  unsigned char* nice_image_run = nice_image->ptr();
  unsigned char* contours_run = all_contours.ptr();
  float* corners_run = reinterpret_cast<float*>(corners_mat_normalized.ptr());
  squelette_run = squelette_mat.ptr();

  for (size_t i = 0; i < nice_image->total(); ++i) {
    /*if (*contours_run != 0) {
      nice_image_run[image::kBlueIndex] = 0;
      nice_image_run[image::kRedIndex] = 255;
      nice_image_run[image::kGreenIndex] = 0;*/
    /*
    if (*squelette_run != 0) {
      nice_image_run[image::kRedIndex] = 0;
      nice_image_run[image::kGreenIndex] = 0;
      nice_image_run[image::kBlueIndex] = 255;
    }
    */
    if (*corners_run > 100) {
      cv::circle(*nice_image, PositionOf(i, nice_image->cols), 1, cv::Scalar(255, 255, 0), 1);
    }

    nice_image_run += 4;
    ++contours_run;
    ++corners_run;
    ++squelette_run;
  }
}

}  // namespace hand_extractor