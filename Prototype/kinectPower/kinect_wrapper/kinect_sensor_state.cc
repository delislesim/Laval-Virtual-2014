#include "kinect_wrapper/kinect_sensor_state.h"

#include "base/logging.h"
#include "kinect_wrapper/kinect_observer.h"

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

void KinectSensorState::CreateBuffers() {
  depth_buffer_.reset(new KinectBuffer(
      kKinectDepthWidth, kKinectDepthHeight, CV_16U));
  skeleton_buffer_.reset(new KinectSwitch<KinectSkeletonFrame>);
}

void KinectSensorState::ReleaseBuffers() {
  depth_buffer_.reset(NULL);
  skeleton_buffer_.reset(NULL);
}

bool KinectSensorState::QueryDepth(int past_frame, cv::Mat* mat) const {
  assert(past_frame < kNumBuffers);
  assert(mat != NULL);

  if (depth_buffer_.get() == NULL)
    return false;
  depth_buffer_->GetMatrix(past_frame, mat);
  return true;
}

bool KinectSensorState::QuerySkeletonFrame(
    KinectSkeletonFrame* skeleton_frame) const {
  assert(skeleton_frame != NULL);

  if (skeleton_buffer_.get() == NULL)
    return false;
  skeleton_buffer_->GetCurrent(skeleton_frame);
  return true;
}

void KinectSensorState::InsertDepthFrame(const char* depth_frame,
                                         size_t depth_frame_size) {
  depth_buffer_->CopyData(depth_frame, depth_frame_size);

  cv::Mat depth_mat;
  QueryDepth(0, &depth_mat);
  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveDepth(depth_mat, *this));
}

void KinectSensorState::InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                                         const NUI_DEPTH_IMAGE_PIXEL* end) {
  depth_buffer_->CopyDepthTexture(start, end);

  cv::Mat depth_mat;
  QueryDepth(0, &depth_mat);
  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveDepth(depth_mat, *this));
}

void KinectSensorState::InsertSkeletonFrame(
    const KinectSkeletonFrame& skeleton_frame) {
  skeleton_buffer_->SetNext(skeleton_frame);

  cv::Mat depth_mat;
  QueryDepth(0, &depth_mat);
  FOR_EACH_OBSERVER(KinectObserver, observers_,
                    ObserveSkeleton(skeleton_frame, *this));
}

void KinectSensorState::AddObserver(KinectObserver* obs) {
  observers_.AddObserver(obs);
}

}  // namespace kinect_wrapper