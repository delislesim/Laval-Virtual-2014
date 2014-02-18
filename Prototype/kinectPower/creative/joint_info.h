#pragma once

#include <opencv2/core/core.hpp>

#include "hsklu.h"

namespace creative {

class JointInfo {
 public:
  JointInfo() : error_(-1) {}
  JointInfo(const cv::Vec3f& position, float error)
      : position_(position),
        error_(error) {}
  
  const cv::Vec3f& position() const { return position_; }
  float error() const { return error_; }

 private:
  cv::Vec3f position_;
  float error_;

};

}  // namespace creative
