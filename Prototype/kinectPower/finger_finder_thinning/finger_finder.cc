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

namespace finger_finder_thinning {

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
  finger_finder::Segmenter(depth_mat, min_hands_depth_, max_hands_depth_,
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
  cv::Mat dilated_depth_contours_img = cv::Mat::zeros(color_mat.size(), CV_8U);
  for (size_t i = 0; i < depth_contours_color_coordinates.size(); ++i) {
    const cv::Point* points[1];
    points[0] = &depth_contours_color_coordinates[i][0];
    int num_points = depth_contours_color_coordinates[i].size();
    cv::fillPoly(dilated_depth_contours_img, points, &num_points, 1, cv::Scalar(1));
  }
  cv::dilate(dilated_depth_contours_img, dilated_depth_contours_img, cv::Mat(), cv::Point(-1, -1), 5);

  // Retrouver les contours dans l'image de profondeur dilatée.
  std::vector<std::vector<cv::Point> > dilated_depth_contours_color_coordinates;
  cv::findContours(dilated_depth_contours_img, 
                   dilated_depth_contours_color_coordinates,
                   CV_RETR_LIST,
                   CV_CHAIN_APPROX_SIMPLE);

  // Extraire des contours de l'image de couleur.
  cv::Mat all_contours;
  CannyContour(color_mat, &all_contours);

  // Enlever tous les contours de couleur qui ne sont pas à la bonne profondeur.
  unsigned char* dilated_run = dilated_depth_contours_img.ptr();
  unsigned char* contours_run = all_contours.ptr();
  for (size_t i = 0; i < dilated_depth_contours_img.total(); ++i) {
    if (*dilated_run != 1)
      *contours_run = 0;
    ++dilated_run;
    ++contours_run;
  }

  // Dilater un peu canny.
  cv::dilate(all_contours, all_contours, cv::Mat(), cv::Point(-1, -1), 1);

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

  // Un peu d'opérateur de Laplace.
  cv::Mat laplacian_result;
  cv::Laplacian(distance_mat_char, laplacian_result, CV_16S, 3, 1, 0);

  // Éliminer les lignes correspondant à des dérivées du mauvais signe
  // sur le résultat de Laplace.
  cv::threshold(laplacian_result, laplacian_result, 0, 0, 4); // 3: tresh_tozero

  // Garder seulement les valeurs positives.
  cv::Mat laplacian_result_positif(laplacian_result.size(), CV_8U);
  cv::convertScaleAbs(laplacian_result, laplacian_result_positif);

  // Soustraire le contour dilaté (et éventuellement tout ce qui n'est pas dans le depth TODO(fdoray))
  cv::dilate(all_contours, all_contours, cv::Mat(), cv::Point(-1, -1), 1);
  
  unsigned char* dilated_contour_run = all_contours.ptr();
  unsigned char* laplace_run = laplacian_result_positif.ptr();

  for (size_t i = 0; i < laplacian_result_positif.total(); ++i) {
    if (*dilated_contour_run != 0)
      *laplace_run = 0;
    ++dilated_contour_run;
    ++laplace_run;
  }

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

}

}  // namespace hand_extractor