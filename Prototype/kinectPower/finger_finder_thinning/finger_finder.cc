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

#include "bitmap_graph/bitmap_run.h"

namespace finger_finder_thinning {

namespace {

int IndexOf(int x, int y) {
  return y*640 + x;
}

class SmallContourRemover {
 public:
  SmallContourRemover(cv::Mat* mat_with_contours_to_remove)
      : mat_with_contours_to_remove_(mat_with_contours_to_remove) {}

  void ObserveComponentStart() {
    component_indexes_.clear();
  }
  void ObserveComponentEnd() {
    if (component_indexes_.size() < 25) {
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
  cv::Mat* mat_with_contours_to_remove_;
  std::vector<int> component_indexes_;
};

void ConnectSkeleton(const cv::Mat& distances, cv::Mat* skeleton) {
  assert(distances.type() == CV_32F);
  assert(skeleton->type() == CV_8U);

  // 1. Copier le squelette pour le donner au runner.
  cv::Mat skeleton_copy;
  skeleton->copyTo(skeleton_copy);


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

  // Extraire des zones potentielles de mains de l'image de profondeur.
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

  // Enlever les contours trop courts.
  cv::Mat all_contours_copy;
  all_contours.copyTo(all_contours_copy);

  SmallContourRemover contour_remover(&all_contours);
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

  cv::Mat distance_mat_2 = distance_mat * 2;

  /// Gradient X
  //Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat, grad_x, ddepth, 1, 0, 1, scale, delta, cv::BORDER_DEFAULT );

  /// Gradient Y
  //Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat, grad_y, ddepth, 0, 1, 1, scale, delta, cv::BORDER_DEFAULT );

  // Générer une image avec le squelette.
  cv::Mat squelette_mat(depth_mat.size(), CV_8U);
  unsigned char* squelette_run = squelette_mat.ptr();
  short* sobel_x_run = reinterpret_cast<short*>(grad_x.ptr());
  short* sobel_y_run = reinterpret_cast<short*>(grad_y.ptr());

  for (int y = 1; y < depth_mat.rows - 1; ++y) {
    for (int x = 1; x < depth_mat.cols -1; ++x) {
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
    if (depth_contours_mat.ptr()[i] != 255) {
      *squelette_run = 0;
    }
    ++squelette_run;
  }

  // Essayer de compléter les lignes du squelette.
  ConnectSkeleton(distance_mat, &squelette_mat);

  // Convertir la matrice de distance de float --> char.
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

  // Mettre le squelette en blanc sur l'image résultat.
  cv::cvtColor( distance_mat_char, *nice_image, CV_GRAY2RGBA);
  
  // Mettre du rouge sur les contours.
  unsigned char* nice_image_run = nice_image->ptr();
  unsigned char* contours_run = all_contours.ptr();
  squelette_run = squelette_mat.ptr();
  for (size_t i = 0; i < nice_image->total(); ++i) {
    if (*contours_run != 0) {
      nice_image_run[image::kBlueIndex] = 0;
      nice_image_run[image::kRedIndex] = 255;
      nice_image_run[image::kGreenIndex] = 0;
    } else if (*squelette_run == 255) {
      nice_image_run[image::kBlueIndex] = 0;
      nice_image_run[image::kRedIndex] = 0;
      nice_image_run[image::kGreenIndex] = 255;
    }
    nice_image_run += 4;
    ++contours_run;
    ++squelette_run;
  }
}

}  // namespace hand_extractor