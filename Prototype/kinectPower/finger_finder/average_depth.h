#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Calcule la profondeur moyenne des points retrouvés en partant de
// |position| en en suivant la direction spécifiée par |direction|. Seuls
// les points ayant une profondeur dans les limites spécifiées sont considérés.
int AverageDepth(const cv::Point& position, const cv::Vec2i direction,
                 const cv::Mat& depth_mat,
                 unsigned short min_depth, unsigned short max_depth);

}  // namespace finger_finder
