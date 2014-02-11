#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Calcule la profondeur moyenne des points entourant |position| et
// ayant une profondeur dans les limites spécifiées.
int AverageDepth(const cv::Point& position, const cv::Mat& depth_mat,
                 unsigned short min_depth, unsigned short max_depth);

}  // namespace finger_finder
