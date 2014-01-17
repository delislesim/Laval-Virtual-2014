#pragma once

#include "base/base.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectSensor;

class KinectStream {
 public:
	KinectStream(KinectSensor* sensor);
	~KinectStream();

 protected:
  KinectSensor* GetSensor() const {
    return sensor_;
  }

  HANDLE GetFrameReadyEvent() const {
    return frame_ready_event_;
  }

 private:
  KinectSensor* sensor_;
  HANDLE frame_ready_event_;

  DISALLOW_COPY_AND_ASSIGN(KinectStream);
};

}  // namespace kinect_wrapper
