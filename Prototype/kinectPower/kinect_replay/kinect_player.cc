#include "kinect_replay/kinect_player.h"

#include <vector>

#include "base/logging.h"
#include "kinect_wrapper/kinect_sensor_state.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"

namespace kinect_replay {

KinectPlayer::KinectPlayer() : is_playing_(false) {
}

KinectPlayer::~KinectPlayer() {
  CloseFile();
}

bool KinectPlayer::LoadFile(const std::string& filename) {
  DCHECK(!is_playing_);

  filename_ = filename;

  in_.open(filename, std::ofstream::in | std::ofstream::binary);

  if (!in_.good())
    return false;

  // Read the header.
  const std::string kHeaderString = "BOUBOU V01";
  const size_t kHeaderStringLen = 10;
  
  char header_buffer[kHeaderStringLen + 1];
  in_.read(header_buffer, kHeaderStringLen + 1);
  header_buffer[kHeaderStringLen] = '\0';

  if (kHeaderString != header_buffer) {
    in_.close();
    return false;
  }

  is_playing_ = true;
  return true;
}

bool KinectPlayer::ReadFrame(kinect_wrapper::KinectSensorState* sensor_state) {
  DCHECK(sensor_state);

  if (!is_playing_)
    return false;

  const size_t kSectionHeaderSize = 5;
  const std::string kDepthHeader =    "DEPTH";
  const std::string kSkeletonHeader = "SKELE";

  if (!in_.good()) {
    // Play again!
    CloseFile();
    LoadFile(filename_);
    if (!in_.good())
      return false;
  }

  std::vector<char> buffer;

  // Read the depth frame.
  buffer.resize(kSectionHeaderSize + 1);
  in_.read(&buffer[0], kSectionHeaderSize);
  buffer[kSectionHeaderSize] = '\0';

  if (kDepthHeader != &buffer[0])
    return false;

  size_t depth_length = 0;
  in_.read(reinterpret_cast<char*>(&depth_length), sizeof(depth_length));

  buffer.resize(depth_length);
  in_.read(&buffer[0], depth_length);
  sensor_state->InsertDepthFrame(&buffer[0], depth_length);

  // Read the skeleton frame.
  buffer.resize(kSectionHeaderSize + 1);
  in_.read(&buffer[0], kSectionHeaderSize);
  buffer[kSectionHeaderSize] = '\0';

  if (kSkeletonHeader != &buffer[0])
    return false;

  kinect_wrapper::KinectSkeletonFrame skeleton_frame;
  const size_t kSkeletonFrameSize =
      sizeof(*skeleton_frame.GetSkeletonFramePtr());

  in_.read(reinterpret_cast<char*>(skeleton_frame.GetSkeletonFramePtr()),
           kSkeletonFrameSize);

  DWORD tracked_ids[kNumTrackedSkeletons];
  for (int i = 0; i < kNumTrackedSkeletons; ++i) {
    in_.read(reinterpret_cast<char*>(&tracked_ids[i]), sizeof(DWORD));
  }

  sensor_state->InsertSkeletonFrame(skeleton_frame,
                                    tracked_ids[0], tracked_ids[1]);

  return true;
}

bool KinectPlayer::CloseFile() {
  if (is_playing_) {
    in_.close();
    is_playing_ = false;
  }
  return true;
}

}  // namespace kinect_replay