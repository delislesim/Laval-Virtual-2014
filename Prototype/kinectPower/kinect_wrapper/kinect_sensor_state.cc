#include "kinect_wrapper/kinect_sensor_state.h"

#include "base/logging.h"
#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

KinectSensorState::KinectSensorState() {
}

KinectSensorState::~KinectSensorState() {
}

void KinectSensorState::SetThread(HANDLE thread) {
  thread_.Set(thread);
}

void KinectSensorState::SetCloseEvent(HANDLE close_event) {
  close_event_.Set(close_event);
}

HANDLE KinectSensorState::GetCloseEvent() {
  return close_event_.get();
}

void KinectSensorState::SendCloseEvent() {
  if (close_event_.get() != INVALID_HANDLE_VALUE)
    ::SetEvent(close_event_.get());
}

void KinectSensorState::WaitThreadCloseAndDelete() {
  if (thread_.get() == INVALID_HANDLE_VALUE)
    return;
  ::WaitForSingleObject(thread_.get(), INFINITE);
  thread_.Close();
  close_event_.Close();
}

bool KinectSensorState::StartRecording(const std::string& filename) {
  return recorder_.StartRecording(filename);
}

void KinectSensorState::StopRecording() {
  recorder_.StopRecording();
}

bool KinectSensorState::LoadReplayFile(const std::string& filename) {
  return player_.LoadFile(filename);
}

bool KinectSensorState::ReplayFrame() {
  return player_.ReadFrame(this);
}

bool KinectSensorState::RecordFrame() {
  return recorder_.RecordFrame(*this);
}

}  // namespace kinect_wrapper