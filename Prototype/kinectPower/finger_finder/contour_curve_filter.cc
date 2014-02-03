#include "finger_finder/contour_curve_filter.h"

#include "kinect_wrapper/kinect_include.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {
 
const double kMultipliers[] = {
  1.0, 0.9, 0.9, 0.6, 0.6, 0.6, 0.1
};

const double kFilterThresholdFirstPass = maths::kPi / 4.0;

}

ContourCurveFilter::ContourCurveFilter() {
  const size_t kNumMultipliers = ARRAYSIZE(kMultipliers);
  for (size_t i = 0; i < kNumMultipliers; ++i) {
    multipliers_.push_back(kMultipliers[i]);
  }
}

void ContourCurveFilter::FilterContourCurve(
    const std::vector<double>& raw_curve,
    std::vector<double>* filtered_curve) const {
  assert(filtered_curve);
  assert(filtered_curve->empty());

  // TODO(fdoray): Inventer un filtre intelligent :)

  for (size_t i = 0; i < raw_curve.size(); ++i) {
    filtered_curve->push_back(raw_curve[i]);
  }
}

}  // namespace finger_finder