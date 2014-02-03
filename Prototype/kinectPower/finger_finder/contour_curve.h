#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

class ContourCurve {
 public:
  ContourCurve();

  void ComputeContourCurve(const std::vector<cv::Point>& contour,
                           std::vector<double>* curve) const;

  // Retourne le nombre minimal de points requis de chaque côté d'un point
  // afin d'être capable de calculer sa curve.
  size_t GetMultipliersCount() const {
    return multipliers_.size();
  }

 private:
  double ComputePointCurve(size_t point_index,
                           const std::vector<cv::Point>& contour) const;

  std::vector<double> multipliers_;
  double multipliers_sum_;

};

}  // namespace finger_finder