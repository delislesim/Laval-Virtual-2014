#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Crée une matrice dans laquelle chaque pixel contient comme valeur
// l'index du pixel suivant le long d'un contour.
void BitmapGraph(const cv::Mat& contours, cv::Mat* bitmap_graph);

}  // namespace finger_finder
