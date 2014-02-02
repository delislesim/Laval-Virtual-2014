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

double AngleBetween(const cv::Vec2i& vec_a,
                    const cv::Vec2i& vec_b) {
  // Avoid division by 0.
  if ((vec_a.val[0] == 0 && vec_a.val[1] == 0) ||
      (vec_b.val[0] == 0 && vec_b.val[1] == 0)) {
    return 0.0;
  }

  double cos_angle = static_cast<double>(vec_a.dot(vec_b)) /
      (cv::norm(vec_a) * cv::norm(vec_b));
  double angle = acos(cos_angle);

  if (angle == 0.0)
    return 0.0;

  int determinant = (vec_a.val[0] * vec_b.val[1]) -
                    (vec_a.val[1] * vec_b.val[0]);

  if (determinant < 0)
    angle = -angle;
  
  return angle;
}

float Area(const std::vector<cv::Point>& points) {
  cv::RotatedRect rect = cv::minAreaRect(points);
  float area = rect.size.width * rect.size.height;
  return area;
}

int DistanceSquare(const cv::Point& point_a, const cv::Point& point_b) {
  cv::Point dist = point_a - point_b;
  return dist.x * dist.x + dist.y * dist.y;
}

double Distance(const cv::Point& point_a, const cv::Point& point_b) {
  int distance_square = DistanceSquare(point_a, point_b);
  if (distance_square == 0)
    return 0.0;
  double distance = sqrt(static_cast<double>(distance_square));
  return distance;
}

}  // namespace maths

