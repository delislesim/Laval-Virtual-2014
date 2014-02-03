#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace finger_finder {

class ContourCurveFilter {
 public:
  ContourCurveFilter();

  void FilterContourCurve(const std::vector<double>& raw_curve,
                          std::vector<double>* filtered_curve) const;

  size_t GetMultipliersCount() const {
    return multipliers_.size();
  }

 private:
  std::vector<double> multipliers_;

};

}  // finger_finder