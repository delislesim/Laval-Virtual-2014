#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace maths {

double Curve(size_t point_index, const std::vector<cv::Point>& contour_walk);

}  // maths