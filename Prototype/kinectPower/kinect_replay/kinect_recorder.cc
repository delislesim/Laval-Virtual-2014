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
  DCHECK(!is_recording_);

  out_.open(filename,
            std::ofstream::out | std::ofstream::trunc | std::ofstream::binary);

  if (!out_.good())
    return false;

  // Write a nice header.
  const char* kHeaderString = "BOUBOU V01";
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

  if (!out_.good())
    return false;

  // Query the depth frame.
  cv::Mat depth_mat;
  sensor_state.QueryDepth(0, &depth_mat);

  // Write the depth frame.
  size_t depth_frame_size = depth_mat.total() *  depth_mat.elemSize();
  out_.write(kDepthHeader, kSectionHeaderSize);
  out_.write(reinterpret_cast<const char*>(&depth_frame_size),
             sizeof(depth_frame_size));
  out_.write(reinterpret_cast<const char*>(depth_mat.ptr()),
             depth_frame_size);

  // Query the skeleton frame.
  KinectSkeletonFrame skeleton_frame;
  sensor_state.QuerySkeletonFrame(&skeleton_frame);

  // Write the skeleton frame.
  out_.write(kSkeletonHeader, kSectionHeaderSize);
  const size_t kSkeletonFrameSize =
      sizeof(*skeleton_frame.GetSkeletonFramePtr());
  out_.write(
      reinterpret_cast<const char*>(skeleton_frame.GetSkeletonFramePtr()),
      kSkeletonFrameSize);

  for (int i = 0; i < kNumTrackedSkeletons; ++i) {
    DWORD tracked_id = skeleton_frame.GetSkeletonTrackId(i);
    out_.write(reinterpret_cast<const char*>(&tracked_id),
               sizeof(tracked_id));
  }

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