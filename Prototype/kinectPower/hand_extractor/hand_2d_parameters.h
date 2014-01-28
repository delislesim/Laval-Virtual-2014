#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/logging.h"

namespace hand_extractor {

class Hand2dParameters {
 public:
   Hand2dParameters();

  enum HandJoint {
    THUMB_TIP = 0,
    INDEX_TIP,
    MIDDLE_TIP,
    RING_TIP,
    LITTLE_TIP,
    JOINTS_COUNT
  };

  struct PotentialTip {
    cv::Point position;
    float lifetime;
  };

  typedef std::vector<PotentialTip> PotentialTipVector;

  cv::Point GetContourCenter() const {
    return contour_center_;
  }

  void SetContourCenter(const cv::Point center) {
    contour_center_ = center;
  }

  bool GetJointPosition(HandJoint joint, cv::Point* position) const {
    assert(position);
    *position = joints_positions_[joint];
    return joints_known_[joint];
  }

  void SetJointPosition(HandJoint joint, const cv::Point& position) {
    joints_known_[joint] = true;
    joints_positions_[joint] = position;
  }

  void PushPotentialTip(const PotentialTip& potential_tip) {
    potential_tips_.push_back(potential_tip);
  }

  const PotentialTipVector& GetPotentialTips() const {
    return potential_tips_;
  }

 private:
  cv::Point contour_center_;

  // Position of each point of the hand skeleton.
  std::vector<cv::Point> joints_positions_;

  // Indicates whether the position of each joint is known.
  std::vector<bool> joints_known_;

  PotentialTipVector potential_tips_;
};

}  // namespace hand_extractor