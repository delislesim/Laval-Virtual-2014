#pragma once

#include <opencv2/core/core.hpp>
#include <string>
#include <vector>

#include "base/base.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectBuffer;
class KinectSensor;
class KinectSkeletonFrame;

#define kMaxNumSensors (6)

class KinectWrapper {
 public:
  // Singleton.
  static KinectWrapper* instance();
  static void Release();

  // Initialization.
  void Initialize();
  void StartSensorThread(int sensor_index);
  void Shutdown();

  // Replay.
  bool RecordSensor(int sensor_index, const std::string& filename);

  // Depth.
  bool QueryDepth(int sensor_index, cv::Mat* mat) const;

  // Skeletons.
  bool QuerySkeletonFrame(int sensor_index,
                          KinectSkeletonFrame* skeleton_frame) const;

 private:
  KinectWrapper();
	~KinectWrapper();

  struct SensorInfo {
    SensorInfo();

    KinectSensor* sensor;
    KinectBuffer* depth_buffer;
    KinectSkeletonFrame* skeleton_buffer;
    size_t current_skeleton_buffer;
    HANDLE thread;
    HANDLE close_event;
    kinect_replay::KinectRecorder recorder;

    DISALLOW_COPY_AND_ASSIGN(SensorInfo);
  };

  struct SensorThreadParams {
    KinectWrapper* wrapper;
    int sensor_index;
  };
  static DWORD SensorThread(SensorThreadParams* params);

  KinectSensor* CreateSensorByIndex(int index, std::string* error);
  int GetSensorCount();

  static KinectWrapper* instance_;

  SensorInfo sensor_info_[kMaxNumSensors];

  DISALLOW_COPY_AND_ASSIGN(KinectWrapper);
};

}  // namespace kinect_wrapper
