#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

// Calcule la profondeur moyenne des points retrouv�s en partant de
// |position| en en suivant la direction sp�cifi�e par |direction|. Seuls
// les points ayant une profondeur dans les limites sp�cifi�es sont consid�r�s.
int AverageDepth(const cv::Point& position, const cv::Vec2i direction,
                 const cv::Mat& depth_mat,
                 unsigned short min_depth, unsigned short max_depth);

}  // namespace finger_finder
