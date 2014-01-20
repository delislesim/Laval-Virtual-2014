#pragma once

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {  

class KinectSensorState;

class KinectSensor {
 public:
  KinectSensor(INuiSensor* native_sensor);
  ~KinectSensor();

  // Depth stream.
  bool OpenDepthStream();
  bool PollNextDepthFrame(KinectSensorState* state);
  HANDLE GetDepthFrameReadyEvent() const {
    return depth_frame_ready_event_;
  }
  size_t depth_stream_width() const {
    return depth_stream_width_;
  }
  size_t depth_stream_height() const {
    return depth_stream_height_;
  }

  // Skeleton stream.
  bool OpenSkeletonStream();
  bool PollNextSkeletonFrame(KinectSensorState* state);
  HANDLE GetSkeletonFrameReadyEvent() const {
    return skeleton_frame_ready_event_;
  }

 private:
  INuiSensor* native_sensor_;

  bool near_mode_enabled_;

  // Depth stream.
  bool depth_stream_opened_;
  HANDLE depth_frame_ready_event_;
  HANDLE depth_stream_handle_;
  size_t depth_stream_width_;
  size_t depth_stream_height_;

  // Skeleton stream.
  bool skeleton_seated_enabled_;
  bool skeleton_near_enabled_;  
  bool skeleton_stream_opened_;
  HANDLE skeleton_frame_ready_event_;
  DWORD skeleton_sticky_ids_[kNumTrackedSkeletons];


  DISALLOW_COPY_AND_ASSIGN(KinectSensor);
};

}  // namespace kinect_wrapper