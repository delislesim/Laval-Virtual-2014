#pragma once

#include <opencv2/core/core.hpp>
#include <string>
#include <vector>

#include "base/base.h"
#include "kinect_replay/kinect_player.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_sensor_state.h"

namespace kinect_wrapper {

class KinectBuffer;
class KinectObserver;
class KinectSensor;
class KinectSkeletonFrame;

#define kMaxNumSensors (6)

class KinectWrapper {
 public:
  // Singleton.
  static KinectWrapper* instance();
  static void Release();

  // Access the sensor.
  KinectSensor* GetSensorByIndex(int index) {
    assert(sensor_state_[index].GetSensor() != NULL);
    return sensor_state_[index].GetSensor();
  }

  // Initialization.
  void Initialize();
  void StartSensorThread(int sensor_index);
  void Shutdown();

  // Replay.
  bool RecordSensor(int sensor_index, const std::string& filename);
  bool StartPlaySensor(int sensor_index, const std::string& filename);
  bool PlayNextFrame(int sensor_index);

  // Access streams.
  bool QueryDepth(int sensor_index, cv::Mat* mat) const;
  bool QuerySkeletonFrame(int sensor_index,
                          KinectSkeletonFrame* skeleton_frame) const;
  void AddObserver(int sensor_index, KinectObserver* observer);

 private:
  KinectWrapper();
	~KinectWrapper();

  struct SensorThreadParams {
    KinectWrapper* wrapper;
    int sensor_index;
  };
  static DWORD SensorThread(SensorThreadParams* params);

  KinectSensor* CreateSensorByIndex(int index, std::string* error);
  int GetSensorCount();

  static KinectWrapper* instance_;

  KinectSensorState sensor_state_[kMaxNumSensors];

  DISALLOW_COPY_AND_ASSIGN(KinectWrapper);
};

}  // namespace kinect_wrapper
