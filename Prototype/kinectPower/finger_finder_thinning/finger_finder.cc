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

class SkeletonConnector {
 public:
  SkeletonConnector(const cv::Mat* depth_mat, cv::Mat* mat_with_skeleton_to_remove)
      : depth_mat_(depth_mat), mat_with_skeleton_to_remove_(mat_with_skeleton_to_remove) {}

  void ObserveComponentStart() {
    component_indexes_.clear();
    num_bad_depth_ = 0;
  }
  void ObserveComponentEnd() {
    if (num_bad_depth_ > 10 || (num_bad_depth_ > 0 && component_indexes_.size() < 15)) {
      // Remove the component.
      for (size_t i = 0; i < component_indexes_.size(); ++i) {
        mat_with_skeleton_to_remove_->ptr()[component_indexes_[i]] = 0;
      }
    }
  }

  void ObserveIntersectionStart(int index) {
    component_indexes_.push_back(index);
    if (depth_mat_->ptr()[index] == 0) {
      ++num_bad_depth_;
    }
  }
  void ObserveIntersectionEnd() {
  }

  void ObservePixel(int index) {
    component_indexes_.push_back(index);
    if (depth_mat_->ptr()[index] == 0) {
      ++num_bad_depth_;
    }
  }
  void ObserveLeaf(int index) {
    component_indexes_.push_back(index);
    if (depth_mat_->ptr()[index] == 0) {
      ++num_bad_depth_;
    }
  }

 private:
  cv::Mat* mat_with_skeleton_to_remove_;
  const cv::Mat* depth_mat_;
  std::vector<int> component_indexes_;
  int num_bad_depth_;
};

void CloseContours(cv::Mat* contours) {
  int rows = contours->rows;
  int cols = contours->cols;

  
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
  assert(color_mat.type() == CV_8UC4);

  // Image pour les tests.
  *nice_image = cv::Mat::zeros(depth_mat.size(), CV_8UC4);
  image::InitializeBlackImage(nice_image);

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
    cv::fillPoly(depth_contours_mat, points, &num_points, 1, cv::Scalar(1));
  }
  cv::Mat dilated_depth_contours_mat;
  cv::dilate(depth_contours_mat, dilated_depth_contours_mat, cv::Mat(), cv::Point(-1, -1), 5);

  // Extraire des contours de l'image de couleur.
  cv::Mat all_contours;
  CannyContour(color_mat, &all_contours);

  // Enlever tous les contours de couleur qui ne sont pas à la bonne profondeur.
  unsigned char* dilated_run = dilated_depth_contours_mat.ptr();
  unsigned char* contours_run = all_contours.ptr();
  for (size_t i = 0; i < dilated_depth_contours_mat.total(); ++i) {
    if (*dilated_run != 1)
      *contours_run = 0;
    ++dilated_run;
    ++contours_run;
  }

  // Enlever les contours trop courts.
  cv::Mat all_contours_copy;
  all_contours.copyTo(all_contours_copy);

  SmallContourRemover contour_remover(&all_contours);
  bitmap_graph::BitmapRun contour_remover_run;
  contour_remover_run.Run(&all_contours_copy, &contour_remover);

  // Inverser les valeurs pour faire plaisir à l'algorithme.
  cv::Mat all_contours_inverse = cv::Mat(all_contours.size(), CV_8U, 255) - all_contours;

  // Calculer la distance de chaque point par rapport au contour le plus proche.
  cv::Mat distance_mat;
  cv::distanceTransform(all_contours_inverse, distance_mat, CV_DIST_L1, 3);

  // Convertir la matrice de distance de float --> char.
  cv::Mat distance_mat_char(distance_mat.size(), CV_8U);
  float* distance_src_run = reinterpret_cast<float*>(distance_mat.ptr());
  unsigned char* distance_dst_run = distance_mat_char.ptr();

  for (size_t i = 0; i < distance_mat.total(); ++i) {
    if (*distance_src_run > 127)
      *distance_src_run = 127;

    *distance_dst_run = *distance_src_run * 2;

    ++distance_src_run;
    ++distance_dst_run;
  }
  
  /*
  // Un peu d'opérateur de Laplace.
  cv::Mat laplacian_result;
  cv::Laplacian(distance_mat_char, laplacian_result, CV_16S, 1, 1, 0);

  // Éliminer les lignes correspondant à des dérivées du mauvais signe
  // sur le résultat de Laplace.
  cv::threshold(laplacian_result, laplacian_result, 0, 0, 4); // 3: tresh_tozero

  // Garder seulement les valeurs positives.
  cv::Mat laplacian_result_positif(laplacian_result.size(), CV_8U);
  cv::convertScaleAbs(laplacian_result, laplacian_result_positif);

  // Enlever tout ce qui n'est pas à la bonne profondeur.
  dilated_run = dilated_depth_contours_mat.ptr();
  unsigned char* laplacian_result_positif_run = laplacian_result_positif.ptr();
  for (size_t i = 0; i < dilated_depth_contours_mat.total(); ++i) {
    if (*dilated_run != 1)
      *laplacian_result_positif_run = 0;
    ++dilated_run;
    ++laplacian_result_positif_run;
  }
  */
  
  // Sobel.
  int scale = 1;
  int delta = 0;
  int ddepth = CV_16S;
  cv::Mat grad_x, grad_y;

  /// Gradient X
  //Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat_char, grad_x, ddepth, 1, 0, 1, scale, delta, cv::BORDER_DEFAULT );

  /// Gradient Y
  //Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
  cv::Sobel( distance_mat_char, grad_y, ddepth, 0, 1, 1, scale, delta, cv::BORDER_DEFAULT );

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
      } else if (critical_x && abs(sobel_y) < 4) {
        *current_squelette = 255;
      } else if (critical_y && abs(sobel_x) < 4) {
        *current_squelette = 255;
      } else {
        *current_squelette = 0;
      }

    }
  }

  /*
  // Faire une image plus nice.
  unsigned char* laplacian_result_run = laplacian_result_positif.ptr();
  for (size_t i = 0; i < laplacian_result_positif.total(); ++i) {
    if (*laplacian_result_run > 0 && *laplacian_result_run < 63)
      *laplacian_result_run *= 4;
    else if (*laplacian_result_run > 0)
      *laplacian_result_run = 255;
    ++laplacian_result_run;
  }

  // Copier les contours de l'image couleur dans nice_image.
  cv::cvtColor(laplacian_result_positif, *nice_image, CV_GRAY2RGBA);
  */

  // Enlever les parties du squelette qui ne sont pas à la bonne profondeur.
  // TODO(fdoray): Aussi enlever les squelettes qui touchent trop aux bordures.
  squelette_run = squelette_mat.ptr();
  for (size_t i = 0; i < squelette_mat.total(); ++i) {
    if (depth_contours_mat.ptr()[i] != 1) {
      *squelette_run = 0;
    }
    ++squelette_run;
  }

  // Essayer de compléter les lignes du squelette.

  //cv::dilate(squelette_mat, squelette_mat, cv::Mat(), cv::Point(-1, -1), 1);
  /*
  std::vector<cv::Vec4i> lines;
  HoughLinesP(squelette_mat, lines, 1, CV_PI/180, 15, 50,  15);
  
  for( size_t i = 0; i < lines.size(); i++ ) {
    cv::Vec4i l = lines[i];
    cv::line( squelette_mat, cv::Point(l[0], l[1]), cv::Point(l[2], l[3]), cv::Scalar(255), 4);
  }
  */
  // Mettre le squelette en blanc sur l'image résultat.
  cv::cvtColor( distance_mat_char, *nice_image, CV_GRAY2RGBA);
  
  //cv::cvtColor(squelette_mat, *nice_image, CV_GRAY2RGBA);

  // Mettre du rouge sur les contours.
  unsigned char* nice_image_run = nice_image->ptr();
  contours_run = all_contours.ptr();
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