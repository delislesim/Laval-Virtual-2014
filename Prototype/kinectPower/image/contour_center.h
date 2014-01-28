#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace image {

cv::Point ContourCenter(const std::vector<cv::Point>& contour);

}  // namespace image