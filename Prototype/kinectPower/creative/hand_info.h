#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "creative/joint_info.h"
#include "hsklu.h"

namespace creative {

class HandInfo {
 public:
  HandInfo() : joints_(NUM_JOINTS) {}

  enum Joint {
    FOREARM = 0,
    PALM,
    PINKY_BASE,
    PINKY_MID,
    PINKY_TIP,
    RING_BASE,
    RING_MID,
    RING_TIP,
    MIDDLE_BASE,
    MIDDLE_MID,
    MIDDLE_TIP,
    INDEX_BASE,
    INDEX_MID,
    INDEX_TIP,
    THUMB_BASE,
    THUMB_MID,
    THUMB_TIP,
    NUM_JOINTS
  };

  void SetJoint(Joint joint, const JointInfo& joint_info) {
    joints_[static_cast<int>(joint)] = joint_info;
  }

  const JointInfo& GetJoint(Joint joint) const {
    return joints_[static_cast<int>(joint)];
  }

 private:
  std::vector<JointInfo> joints_;
};

}  // namespace creative
