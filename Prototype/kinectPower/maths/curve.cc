#include "maths/maths.h"

#include "base/logging.h"
#include "maths/maths.h"

namespace maths {

namespace {

const size_t kSkipPoints = 1;

const double kAngleMultiplier[] = {
    0.05, 0.2, 1.0, 0.9, 0.7, 0.7, 0.7, 0.7, 0.6, 0.2, 0.1, 0.1
};
const size_t kNumAngleMultiplier = 12;
const double kSumAngleDivisor = 0.05 + 0.2 + 0.1 + 0.9 + 0.70 + 0.70 + 0.70 + 0.60 + 0.70 + 0.2 + 0.1 + 0.1;

}  // namespace

double Curve(size_t point_index, const std::vector<cv::Point>& contour_walk) {
  if (contour_walk.size() < kNumAngleMultiplier * kSkipPoints)
    return 0.0;

  double sum_angle = 0;
  for (size_t i = 0; i < kNumAngleMultiplier; ++i) {
    double angle =
        maths::AngleBetween(contour_walk[maths::Previous(i * kSkipPoints, point_index, contour_walk.size())],
                            contour_walk[point_index],
                            contour_walk[maths::Next(i * kSkipPoints, point_index, contour_walk.size())]);
    sum_angle += kAngleMultiplier[i] * angle;
  }
  double curve = sum_angle / kSumAngleDivisor;
  return curve;
}

}  // namespace maths

