#pragma once

#include <opencv2/core/core.hpp>

#include "finger_finder/contour_angle.h"

namespace finger_finder {

// Cr�er des listes de points appartenant � des contours � partir
// d'un graphe de bitmap. !! Le graphe re�u en param�tre est d�truit. !!
void BitmapGraphToContoursList(cv::Mat* bitmap_graph,
                               ContoursList* contours_list);

}  // namespace finger_finder
