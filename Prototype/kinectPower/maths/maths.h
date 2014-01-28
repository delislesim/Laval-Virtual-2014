#pragma once

#include <opencv2/core/core.hpp>

namespace maths {

extern const double kPi;

double RadToDegrees(double rad);

double Round(double number);
int RoundToInt(double number);

double AngleBetween(const cv::Point& angle_point,
                    const cv::Point& before_angle_point,
                    const cv::Point& after_angle_point);

// Calculates the area of the minimum rotated rectangle enclosing the
// set of points passed as a parameter.
// @param points the points enclosed by the rectangle.
// @returns the area of the rectangle.
float Area(const std::vector<cv::Point>& points);

int DistanceSquare(const cv::Point& point_a,
                   const cv::Point& point_b);

template<typename T>
T Clamp(T val, T min, T max) {
  assert(min < max);
  if (val < min)
    return min;
  if (val > max)
    return max;
  return val;
}

}  // maths