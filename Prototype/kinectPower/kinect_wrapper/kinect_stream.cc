#include "kinect_wrapper/kinect_stream.h"

namespace kinect_wrapper {

KinectStream::KinectStream(KinectSensor* sensor)
    : sensor_(sensor) {
  frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
}

KinectStream::~KinectStream() {
  CloseHandle(frame_ready_event_);
}

}  // namespace kinect_wrapper