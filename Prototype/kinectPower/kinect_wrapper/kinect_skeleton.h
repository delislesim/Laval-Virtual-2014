#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {


class KinectSkeleton {
 public:
  KinectSkeleton();
  ~KinectSkeleton();

    typedef enum _JointIndex {
    HipCenter = 0,
    Spine,
    ShoulderCenter,
    Head,
    ShoulderLeft,
    ElbowLeft,
    WristLeft,
    HandLeft,
    ShoulderRight,
    ElbowRight,
    WristRight,
    HandRight,
    HipLeft,
    KneeLeft,
    AnkleLeft,
    FootLeft,
    HipRight,
    KneeRight,
    AnkleRight,
    FootRight,
    JointCount
  } JointIndex;

  bool GetJointPosition(size_t skeleton_id, JointIndex joint_index,
                        cv::Vec3f* position, bool* inferred);

  // Method to load the skeleton data from the sensor.
  NUI_SKELETON_FRAME* frame_ptr() {
    return &frame_;
  }
  void SetTrackedSkeletons(DWORD track_id_1, DWORD track_id_2);

 private:
  bool ready_;

  NUI_SKELETON_FRAME frame_;
  DWORD tracked_skeletons_[kNumTrackedSkeletons];

  // DISALLOW_COPY_AND_ASSIGN(KinectSkeleton);
};

}  // namespace kinect_wrapper