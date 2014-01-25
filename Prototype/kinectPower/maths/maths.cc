#include "maths/maths.h"

#include <cmath>
#include <opencv2/imgproc/imgproc.hpp>

namespace maths {

const double kPi = 3.141592653589793238462643383279502884197;

double RadToDegrees(double rad) {
  return rad * 360 / (2 * maths::kPi);
}

double Round(double number) {
  return number < 0.0 ? ceil(number - 0.5) : floor(number + 0.5);
}

int RoundToInt(double number) {
  return static_cast<int>(Round(number));
}

double AngleBetween(const cv::Point& angle_point,
                    const cv::Point& before_angle_point,
                    const cv::Point& after_angle_point) {
  double angle = abs(static_cast<int>(round(maths::RadToDegrees(
      atan2(static_cast<double>(before_angle_point.x - angle_point.x),
      static_cast<double>(before_angle_point.y - angle_point.y)) -
      atan2(static_cast<double>(after_angle_point.x - angle_point.x),
      static_cast<double>(after_angle_point.y - angle_point.y))))));
  return angle;
}

float Area(const std::vector<cv::Point>& points) {
  cv::RotatedRect rect = cv::minAreaRect(points);
  float area = rect.size.width * rect.size.height;
  return area;
}

}  // namespace maths

