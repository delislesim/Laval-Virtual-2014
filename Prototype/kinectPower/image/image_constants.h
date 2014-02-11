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
extern const int kThickness2;

// Matrice à utiliser comme kernel de la fonction cv::dilate pour avoir
// une forme arrondie.
extern const cv::Mat kRoundedDilater;
extern const cv::Point kRoundedDilaterCenter;
extern const int kIteration1;
extern const int kIteration2;
extern const int kIteration3;
extern const int kIteration4;
extern const int kIteration5;
extern const int kIteration6;

}  // namespace image