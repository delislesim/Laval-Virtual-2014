#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "algos/kalman_filter.h"

namespace finger_finder {

class Tip {
 public:
  Tip();

  const cv::Point& position() const {
    return position_;
  }

  int depth() const {
    return depth_;
  }

 private:
  // Position of the tip.
  cv::Point position_;

  // Depth of the tip.
  int depth_;

  // Kalman filter.
  algos::KalmanFilter kalman_;
};

class HandParameters {
 public:
  typedef std::vector<Tip> TipVector;
  typedef TipVector::const_iterator TipIterator;

  HandParameters();

  const cv::Point& contour_center() const {
    return contour_center_;
  }

  void SetContourCenter(const cv::Point& contour_center) {
    contour_center_ = contour_center;
  }

  TipIterator TipBegin() const {
    return tips_.begin();
  }

  TipIterator TipEnd() const {
    return tips_.end();
  }

  size_t TipCount() const {
    return tips_.size();
  }

  const Tip& TipAtIndex(size_t index) const {
    return tips_.at(index);
  }

 private:
   // Point central du contour de la main.
   cv::Point contour_center_;

  TipVector tips_;

};

}  // namespace hand_extractor