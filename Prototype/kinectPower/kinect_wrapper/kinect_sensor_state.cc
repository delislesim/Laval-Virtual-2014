#include "kinect_wrapper/kinect_sensor_state.h"

#include "base/logging.h"

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
  return recorder.StartRecording(filename);
}

void KinectSensorState::StopRecording() {
  recorder.StopRecording();
}

bool KinectSensorState::LoadReplayFile(const std::string& filename) {
  return player.LoadFile(filename);
}

bool KinectSensorState::ReplayFrame() {
  return player.ReadFrame(this);
}

bool KinectSensorState::RecordFrame() {
  return recorder.RecordFrame(*this);
}

void KinectSensorState::CreateBuffers() {
  depth_buffer_.reset(new KinectBuffer(
      kKinectDepthWidth, kKinectDepthHeight, kKinectDepthBytesPerPixel));
  skeleton_buffer_.reset(new KinectSwitch<KinectSkeletonFrame>);
}

void KinectSensorState::ReleaseBuffers() {
  depth_buffer_.reset(NULL);
  skeleton_buffer_.reset(NULL);
}

bool KinectSensorState::QueryDepth(cv::Mat* mat) const {
  DCHECK(mat != NULL);

  if (depth_buffer_.get() == NULL)
    return false;
  depth_buffer_->GetDepthMat(mat);
  return true;
}

bool KinectSensorState::QuerySkeletonFrame(
    KinectSkeletonFrame* skeleton_frame) const {
  DCHECK(skeleton_frame != NULL);

  if (skeleton_buffer_.get() == NULL)
    return false;
  skeleton_buffer_->GetCurrent(skeleton_frame);
  return true;
}

void KinectSensorState::InsertDepthFrame(const char* depth_frame,
                                         size_t depth_frame_size) {
  depth_buffer_->CopyData(depth_frame, depth_frame_size);
}

void KinectSensorState::InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                                         const NUI_DEPTH_IMAGE_PIXEL* end) {
  depth_buffer_->CopyDepthTexture(start, end);
}

void KinectSensorState::InsertSkeletonFrame(
    const KinectSkeletonFrame& skeleton_frame) {
  skeleton_buffer_->SetNext(skeleton_frame);
}

}  // namespace kinect_wrapper