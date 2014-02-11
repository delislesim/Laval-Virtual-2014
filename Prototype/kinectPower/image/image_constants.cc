#include "image/image_constants.h"

namespace image {

const int kBlueIndex = 0;
const int kGreenIndex = 1;
const int kRedIndex = 2;
const int kAlphaIndex = 3;

const cv::Scalar kBlue(255, 0, 0);
const cv::Scalar kGreen(0, 255, 0);
const cv::Scalar kRed(0, 0, 255);
const cv::Scalar kGrey(150, 150, 150);

const int kThickness1 = 1;
const int kThickness2 = 2;

unsigned char kRoundedDilaterData[] = {0, 1, 0, 1, 1, 1, 0, 1, 0};
const cv::Mat kRoundedDilater(cv::Size(3, 3), CV_8U, kRoundedDilaterData);
const cv::Point kRoundedDilaterCenter(-1, -1);

const int kIteration1 = 1;
const int kIteration2 = 2;
const int kIteration3 = 3;
const int kIteration4 = 4;
const int kIteration5 = 5;
const int kIteration6 = 6;

}  // namespace image
