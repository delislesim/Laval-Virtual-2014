#pragma once

#include "base/base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"

namespace kinect_wrapper {

class KinectSkeletonBuffer {
 public:
  KinectSkeletonBuffer();
  ~KinectSkeletonBuffer();

  // Retrieve the current skeleton.
  void GetCurrentSkeletonFrame(KinectSkeletonFrame* frame) const;

  // Insert the next skeleton in the buffer.
  void SetNextSkeleton(const KinectSkeletonFrame& next_frame,
                       DWORD track_id_1, DWORD track_id_2);

 private:
  size_t current_buffer_index_;
  KinectSkeletonFrame frames_[kNumBuffers];

  DISALLOW_COPY_AND_ASSIGN(KinectSkeletonBuffer);
};

}  // namespace kinect_wrapper