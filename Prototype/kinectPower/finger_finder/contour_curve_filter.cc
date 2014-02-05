#include "finger_finder/contour_curve_filter.h"

#include "algos/sliding_window.h"
#include "kinect_wrapper/kinect_include.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {
 
const double kMultipliers[] = {
  1.0, 0.9, 0.9, 0.6, 0.6, 0.6, 0.1
};

const size_t kSlidingWindowSize = 4;
const double kFilterThresholdFirstPass = (1 + 2 * kSlidingWindowSize) * maths::kPi / 4.0;

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

  // Normaliser les angles.
  // Un bout de doigt aura une grande valeur, une ligne droite sera zéro et
  // une creux de doigts aura une valeur négative.
  std::vector<double> normalized_curve(raw_curve.size());
  for (size_t i = 0; i < raw_curve.size(); ++i) {
    double val = raw_curve[i];
    if (val < 0) {
      normalized_curve[i] = - (maths::kPi + raw_curve[i]);
    } else {
      normalized_curve[i] = maths::kPi - raw_curve[i];
    }
  }

  // Calculer la somme des points autour de chaque point de la courbe.
  std::vector<double> sliding_window;
  algos::SlidingWindow(normalized_curve, kSlidingWindowSize, &sliding_window);

  // TODO(fdoray): Inventer un filtre intelligent :)

  *filtered_curve = std::vector<double>(raw_curve.size());
  for (size_t i = 0; i < raw_curve.size(); ++i) {
    if (sliding_window[i] > kFilterThresholdFirstPass) {
      (*filtered_curve)[i] = normalized_curve[i];
    } else {
      (*filtered_curve)[i] = 0;
    }
  }
}

}  // namespace finger_finder