#pragma once

#include <opencv2/core/core.hpp>

#include "finger_finder/contour_angle.h"

namespace finger_finder {

// Créer des listes de points appartenant à des contours à partir
// d'un graphe de bitmap. !! Le graphe reçu en paramètre est détruit. !!
void BitmapGraphToContoursList(cv::Mat* bitmap_graph,
                               ContoursList* contours_list);

}  // namespace finger_finder
