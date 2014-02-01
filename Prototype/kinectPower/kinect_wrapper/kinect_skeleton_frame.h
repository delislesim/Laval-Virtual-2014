#pragma once

#include "base/base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectSkeleton;

class KinectSkeletonFrame {
 public:
  KinectSkeletonFrame();
  ~KinectSkeletonFrame();

  bool GetTrackedSkeleton(size_t tracked_skeleton_id,
                          KinectSkeleton* skeleton) const;

  // Methods to load the skeleton data from the sensor.
  NUI_SKELETON_FRAME* GetSkeletonFramePtr() {
    return &frame_;
  }
  void SetTrackedSkeletons(DWORD track_id_1, DWORD track_id_2);

 private:
  NUI_SKELETON_FRAME frame_;
  DWORD tracked_skeletons_[kNumTrackedSkeletons];

  // DISALLOW_COPY_AND_ASSIGN(KinectSkeletonFrame);
};

}  // namespace kinect_wrapper