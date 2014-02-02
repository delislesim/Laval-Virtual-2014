#pragma once

#include <opencv2/core/core.hpp>

namespace algos {

void ContourWalk(double step, const std::vector<cv::Point>& contour,
                 std::vector<cv::Point>* walk);

}  // namespace algos