#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder {

class FingerInfo {
 public:
  FingerInfo(int x, int y, int depth)
      : x_(x), y_(y), depth_(depth) {}

  int x() const { return x_; }
  int y() const { return y_; }
  int depth() const { return depth_; }

 private:
  int x_;
  int y_;
  int depth_;

};

}  // namespace finger_finder
