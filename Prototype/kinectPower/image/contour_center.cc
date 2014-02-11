#include "image/contour_center.h"

#include <opencv2/imgproc/imgproc.hpp>

#include "algos/maths.h"

namespace image {

cv::Point ContourCenter(const std::vector<cv::Point>& contour) {
  cv::Moments hand_moments = cv::moments(contour);
  cv::Point center = cv::Point(
      maths::RoundToInt(hand_moments.m10 / hand_moments.m00),
      maths::RoundToInt(hand_moments.m01 / hand_moments.m00));
  return center;
}

}  // namespace image