#pragma once

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectBuffer;

class KinectSensor {
 public:
  KinectSensor(INuiSensor* native_sensor);

  // Depth stream.
  bool OpenDepthStream();
  bool PollNextDepthFrame(KinectBuffer* buffer);
  HANDLE GetDepthFrameReadyEvent() const {
    return depth_frame_ready_event_;
  }
  size_t depth_stream_width() const {
    return depth_stream_width_;
  }
  size_t depth_stream_height() const {
    return depth_stream_height_;
  }

 private:
  friend class KinectWrapper;
  ~KinectSensor();

  INuiSensor* native_sensor_;

  bool near_mode_enabled_;

  // Depth stream.
  bool depth_stream_opened_;
  HANDLE depth_frame_ready_event_;
  HANDLE depth_stream_handle_;
  size_t depth_stream_width_;
  size_t depth_stream_height_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensor);
};

}  // namespace kinect_wrapper