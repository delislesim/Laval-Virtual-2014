#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

void CannyContour(const cv::Mat& color_mat,
                  cv::Mat* canny_contour);

}  // namespace finger_finder