#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/logging.h"

namespace hand_extractor {

class Hand2dParameters {
 public:
  struct Tip {
    cv::Point position;
    int depth;
    cv::Point smoothed_position;
    int smoothed_depth;
  };

  typedef std::vector<Tip> TipVector;
  typedef std::vector<Tip>::const_iterator TipIterator;

  Hand2dParameters();

  cv::Point GetContourCenter() const {
    return contour_center_;
  }

  void SetContourCenter(const cv::Point center) {
    contour_center_ = center;
  }

  TipIterator TipBegin() const {
    return tips_.begin();
  }

  TipIterator TipEnd() const {
    return tips_.end();
  }

  size_t TipSize() const {
    return tips_.size();
  }

  const Tip& TipAtIndex(size_t index) const {
    return tips_[index];
  }

  void PushTip(const Tip& tip) {
    tips_.push_back(tip);
  }

  void SmoothUsingPreviousParameters(const Hand2dParameters* previous_parameters);

 private:
  cv::Point contour_center_;

  TipVector tips_;
};

}  // namespace hand_extractor