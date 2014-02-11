#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Traduit une image de profondeur en coordonn�es de la cam�ra de profondeur
// en une image de profondeur en coordonn�es de la cam�ra de couleur.
void DepthTranslator(int sensor_index, const cv::Mat& depth_mat,
                     cv::Mat* depth_mat_color_coordinates);

}  // namespace finger_finder
