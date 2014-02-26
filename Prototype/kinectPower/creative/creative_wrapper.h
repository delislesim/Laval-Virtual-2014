#ifdef USE_INTEL_CAMERA

#pragma once

#include "creative/hand_info.h"
#include "creative/joint_info.h"
#include "hsklu.h"


namespace creative {

class CreativeWrapper {
 public:
  CreativeWrapper();

  void Initialize();

  int GetNumJoints() const {
    return HandInfo::NUM_JOINTS * 2;
  }

  void UpdateJoints();

  void QueryJoints(JointInfo* joints) {
    memcpy_s(joints, GetNumJoints() * sizeof(JointInfo), &joints_[0], joints_.size() * sizeof(JointInfo));
  }

 private:
  hskl::Tracker tracker_;

  bool initialized_;

  std::vector<JointInfo> joints_;
};

}  // namespace creative

#endif
