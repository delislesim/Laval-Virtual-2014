#pragma once

#include "base/base.h"
#include "kinect_wrapper/kinect_stream.h"

namespace kinect_wrapper {

class KinectBuffer;
class KinectSensor;

class KinectDepthStream : public KinectStream {
 public:
	KinectDepthStream(KinectSensor* sensor);
	~KinectDepthStream();

  bool opened() const {
    return opened_;
  }

  bool OpenStream();
  bool PollNextFrame(KinectBuffer* buffer);


 private:
  bool opened_;

  HANDLE stream_handle_;
  bool near_mode_enabled_;

  size_t stream_width_;
  size_t stream_height_;

  DISALLOW_COPY_AND_ASSIGN(KinectDepthStream);
};

}  // namespace kinect_wrapper
