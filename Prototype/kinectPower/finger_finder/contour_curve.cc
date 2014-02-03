#include "finger_finder/contour_curve.h"

#include "kinect_wrapper/kinect_include.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {
  
const double kMultipliers[] = {
  0.05, 0.2, 1.0, 0.9, 0.7, 0.7, 0.7, 0.7, 0.6, 0.2, 0.1, 0.1
};

}  // namespace

ContourCurve::ContourCurve() {
  const size_t kNumMultipliers = ARRAYSIZE(kMultipliers);
  multipliers_sum_ = 0;
  for (size_t i = 0; i < kNumMultipliers; ++i) {
    multipliers_.push_back(kMultipliers[i]);
    multipliers_sum_ += kMultipliers[i];
  }
}

void ContourCurve::ComputeContourCurve(const std::vector<cv::Point>& contour,
                                       std::vector<double>* curve) const {
  // Compute the curve of each point of the contour individually.
  for (size_t i = 0; i < contour.size(); ++i) {
    double curve_value = ComputePointCurve(i, contour);
    curve->push_back(curve_value);
  }
}

double ContourCurve::ComputePointCurve(
    size_t point_index,
    const std::vector<cv::Point>& contour) const {
  if (contour.size() < multipliers_.size())
    return 0.0;

  double sum_angle = 0;
  for (size_t i = 0; i < multipliers_.size(); ++i) {
    double angle = maths::AngleBetween(
        contour[maths::Previous(i, point_index, contour.size())],
        contour[point_index],
        contour[maths::Next(i, point_index, contour.size())]);
    sum_angle += multipliers_[i] * angle;
  }
  double curve = sum_angle / multipliers_sum_;
  return curve;
}

}  // namespace finger_finder