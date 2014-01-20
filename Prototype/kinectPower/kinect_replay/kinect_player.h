#pragma once

#include <fstream>
#include <string>

#include "base/base.h"

namespace kinect_wrapper {
class KinectSensorState;
}  // namespace kinect_wrapper

namespace kinect_replay {

class KinectPlayer {
 public:
  KinectPlayer();
  ~KinectPlayer();

  bool LoadFile(const std::string& filename);
  bool ReadFrame(kinect_wrapper::KinectSensorState* sensor_state);
  bool CloseFile();

 private:
  bool is_playing_;

  // The stream in which the frames are saved.
  std::ifstream in_;

  std::string filename_;

  DISALLOW_COPY_AND_ASSIGN(KinectPlayer);
};

}  // namespace kinect_replay