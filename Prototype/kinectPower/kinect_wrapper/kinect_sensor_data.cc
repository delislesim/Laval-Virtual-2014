#include "kinect_wrapper/kinect_sensor_data.h"

#include "base/logging.h"
#include "kinect_wrapper/kinect_observer.h"

namespace kinect_wrapper {

KinectSensorData::KinectSensorData()
    : sensor_index_(static_cast<size_t>(-1)) {
}

KinectSensorData::~KinectSensorData() {
}

void KinectSensorData::CreateBuffers() {
  depth_buffer_ = cv::Mat(kKinectDepthHeight,
                          kKinectDepthWidth,
                          CV_16U);
  color_buffer_ = cv::Mat(kKinectColorHeight,
                          kKinectColorWidth,
                          CV_8UC4);
}

bool KinectSensorData::QueryDepth(cv::Mat* mat) const {
  assert(mat != NULL);

  if (depth_buffer_.total() == 0)
    return false;

  *mat = depth_buffer_;
  return true;
}

bool KinectSensorData::QueryColor(cv::Mat* mat) const {
  assert(mat != NULL);

  if (color_buffer_.total() == 0)
    return false;

  *mat = color_buffer_;
  return true;
}

void KinectSensorData::InsertDepthFrame(const char* depth_frame,
                                         size_t depth_frame_size) {
  assert(depth_frame_size == depth_buffer_.total() * depth_buffer_.elemSize());

  memcpy_s(depth_buffer_.ptr(), depth_buffer_.total() * depth_buffer_.elemSize(),
           depth_frame, depth_frame_size);

  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveDepth(depth_buffer_, *this));
}

void KinectSensorData::InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
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

void KinectSensorData::InsertColorFrame(const char* color_frame,
                                         const size_t& color_frame_size) {
  assert(color_frame_size == color_buffer_.total() * color_buffer_.elemSize());

  memcpy_s(color_buffer_.ptr(), color_buffer_.total() * color_buffer_.elemSize(),
           color_frame, color_frame_size);

  FOR_EACH_OBSERVER(KinectObserver, observers_, ObserveColor(color_buffer_, *this));
}

void KinectSensorData::InsertSkeletonFrame(
    const KinectSkeletonFrame& skeleton_frame) {
  skeleton_buffer_ = skeleton_frame;

  FOR_EACH_OBSERVER(KinectObserver, observers_,
                    ObserveSkeleton(skeleton_buffer_, *this));
}

void KinectSensorData::InsertInteractionFrame(
    const NUI_INTERACTION_FRAME& interaction_frame) {
  *interaction_buffer_.GetInteractionFramePtr() = interaction_frame;

  FOR_EACH_OBSERVER(KinectObserver, observers_,
                    ObserveInteraction(interaction_buffer_, *this));
}

void KinectSensorData::AddObserver(KinectObserver* obs) {
  observers_.AddObserver(obs);
}

}  // namespace kinect_wrapper