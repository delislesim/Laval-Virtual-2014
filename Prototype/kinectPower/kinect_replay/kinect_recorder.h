#pragma once

#include <fstream>
#include <string>

#include "base/base.h"

namespace kinect_wrapper {
class KinectWrapper;
}  // namespace kinect_wrapper

namespace kinect_replay {

class KinectRecorder {
 public:
  KinectRecorder();
  ~KinectRecorder();

  bool StartRecording(const std::string& filename);
  bool RecordFrame(const kinect_wrapper::KinectWrapper& wrapper,
                   int sensor_index);
  bool StopRecording();

 private:
  // Indicates whether a recording session is in progress.
  bool is_recording_;

  // The stream in which the frames are saved.
  std::ofstream out_;

  DISALLOW_COPY_AND_ASSIGN(KinectRecorder);
};

}  // namespace kinect_replay