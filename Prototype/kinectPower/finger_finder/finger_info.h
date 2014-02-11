#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace finger_finder {

class FingerInfo {
 public:
  FingerInfo(const cv::Point& position, int depth)
      : position_(position), depth_(depth) {}

  const cv::Point& position() const { return position_; }
  int depth() const { return depth_; }

 private:
  cv::Point position_;
  int depth_;
};

typedef std::vector<FingerInfo> FingerInfoVector;

}  // namespace finger_finder
