#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace image {

void InitializeBlackImage(cv::Mat* image);

void RgbImageToRgbaImage(const cv::Mat& rgb, cv::Mat* rgba);

}  // namespace image