#pragma once

#include <opencv2/core/core.hpp>

namespace image {

// Index of each color in an OpenCV matrix.
extern const int kBlueIndex ;
extern const int kGreenIndex;
extern const int kRedIndex;
extern const int kAlphaIndex;

// Common colors.
extern const cv::Scalar kBlue;
extern const cv::Scalar kGreen;
extern const cv::Scalar kRed;
extern const cv::Scalar kGrey;

// Thickness of a border.
extern const int kThickness1;


}  // namespace image