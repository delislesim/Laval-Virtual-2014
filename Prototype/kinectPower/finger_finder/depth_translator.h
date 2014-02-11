#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Traduit une image de profondeur en coordonnées de la caméra de profondeur
// en une image de profondeur en coordonnées de la caméra de couleur.
void DepthTranslator(int sensor_index, const cv::Mat& depth_mat,
                     cv::Mat* depth_mat_color_coordinates);

}  // namespace finger_finder
