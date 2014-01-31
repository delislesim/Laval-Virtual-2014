#include "kinect_replay/kinect_recorder.h"

#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "kinect_wrapper/kinect_sensor_state.h"

using namespace kinect_wrapper;

namespace kinect_replay {

KinectRecorder::KinectRecorder()
    : is_recording_(false) {
}

KinectRecorder::~KinectRecorder() {
  StopRecording();
}

bool KinectRecorder::StartRecording(const std::string& filename) {
  assert(!is_recording_);

  out_.open(filename,
            std::ofstream::out | std::ofstream::trunc | std::ofstream::binary);

  if (!out_.good())
    return false;

  // Write a nice header.
  const char* kHeaderString = "KINECT LIB REPLAY V01";
  out_.write(kHeaderString, strlen(kHeaderString) + 1);

  is_recording_ = true;
  return true;
}

bool KinectRecorder::RecordFrame(
    const kinect_wrapper::KinectSensorState& sensor_state) {
  if (!is_recording_)
    return false;

  const size_t kSectionHeaderSize = 5;
  const char* kDepthHeader =    "DEPTH";
  const char* kSkeletonHeader = "SKELE";
  const char* kColorHeader =    "COLOR";

  if (!out_.good())
    return false;

  // Write the depth frame.
  cv::Mat depth_mat;
  sensor_state.GetData()->QueryDepth(&depth_mat);

  size_t depth_frame_size = depth_mat.total() *  depth_mat.elemSize();
  out_.write(kDepthHeader, kSectionHeaderSize);
  out_.write(reinterpret_cast<const char*>(&depth_frame_size),
             sizeof(depth_frame_size));
  out_.write(reinterpret_cast<const char*>(depth_mat.ptr()),
             depth_frame_size);

  // Write the color frame.
  cv::Mat color_mat;
  sensor_state.GetData()->QueryColor(&color_mat);

  size_t color_frame_size = color_mat.total() * color_mat.elemSize();
  out_.write(kColorHeader, kSectionHeaderSize);
  out_.write(reinterpret_cast<const char*>(&color_frame_size),
             sizeof(color_frame_size));
  out_.write(reinterpret_cast<const char*>(color_mat.ptr()),
             color_frame_size);

  // Write the skeleton frame.
  const KinectSkeletonFrame* skeleton_frame =
      sensor_state.GetData()->GetSkeletonFrame();

  out_.write(kSkeletonHeader, kSectionHeaderSize);
  out_.write(reinterpret_cast<const char*>(skeleton_frame),
             sizeof(*skeleton_frame));

  return true;
}

bool KinectRecorder::StopRecording() {
  if (!is_recording_)
    return false;

  const char* kFooter = "THE_END";
  out_.write(kFooter, strlen(kFooter) + 1);

  out_.close();
  is_recording_ = false;

  return true;
}

}  // kinect_replay