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
  depth_buffer_ = cv::Mat(kKinectDepthHeight,
                          kKinectDepthWidth,
                          CV_16U);
  color_buffer_ = cv::Mat(kKinectColorHeight,
                          kKinectColorWidth,
                          CV_8UC4);
}

bool KinectSensorState::QueryDepth(cv::Mat* mat) const {
  assert(mat != NULL);

  if (depth_buffer_.total() == 0)
    return false;

  *mat = depth_buffer_;
  return true;
}

bool KinectSensorState::QueryColor(cv::Mat* mat) const {
  assert(mat != NULL);

  if (color_buffer_.total() == 0)
    return false;

  *mat = color_buffer_;
  return true;
}

bool KinectSensorState::QuerySkeletonFrame(
    KinectSkeletonFrame* skeleton_frame) const {
  assert(skeleton_frame != NULL);

  *skeleton_frame = skeleton_buffer_;

  return true;
}

void KinectSensorState::InsertDepthFrame(const char* depth_frame,
                                         size_t depth_frame_size) {
  assert(depth_frame_size == depth_buffer_.total() * depth_buffer_.elemSize());

  memcpy_s(depth_buffer_.ptr(), depth_buffer_.total() * depth_buffer_.elemSize(),
           depth_frame, depth_frame_size);

  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveDepth(depth_buffer_, *this));
}

void KinectSensorState::InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                                         const size_t& num_pixels) {
  unsigned short* buffer_run =
    reinterpret_cast<unsigned short*>(depth_buffer_.ptr());
  NUI_DEPTH_IMAGE_PIXEL const* src_run = start;
  NUI_DEPTH_IMAGE_PIXEL const* end = start + num_pixels;

  while (src_run < end) {
    *buffer_run = src_run->depth;

    ++buffer_run;
    ++src_run;
  }

  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveDepth(depth_buffer_, *this));
}

void KinectSensorState::InsertColorFrame(const char* color_frame,
                                         const size_t& color_frame_size) {
  assert(color_frame_size == color_buffer_.total() * color_buffer_.elemSize());

  memcpy_s(color_buffer_.ptr(), color_buffer_.total() * color_buffer_.elemSize(),
           color_frame, color_frame_size);

  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveColor(color_buffer_, *this));
}

void KinectSensorState::InsertSkeletonFrame(
    const KinectSkeletonFrame& skeleton_frame) {
  skeleton_buffer_ = skeleton_frame;

  FOR_EACH_OBSERVER(KinectObserver, observers_,
                    ObserveSkeleton(skeleton_buffer_, *this));
}

void KinectSensorState::AddObserver(KinectObserver* obs) {
  observers_.AddObserver(obs);
}

}  // namespace kinect_wrapper