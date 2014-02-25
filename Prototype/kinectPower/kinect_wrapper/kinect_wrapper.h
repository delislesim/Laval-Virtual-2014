#pragma once

#include <opencv2/core/core.hpp>
#include <string>
#include <vector>

#include "base/base.h"
#include "kinect_replay/kinect_player.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_sensor_state.h"
#include "kinect_gesture_recognition/gesture_controller.h"

namespace kinect_wrapper {

class KinectBuffer;
class KinectObserver;
class KinectSensor;
class KinectSkeletonFrame;

class KinectWrapper {
 public:
  // Singleton.
  static KinectWrapper* instance();
  static void Release();

  // Access the sensors.
  KinectSensor* GetSensorByIndex(int index) {
    assert(sensor_state_[index].GetSensor() != NULL);
    return sensor_state_[index].GetSensor();
  }
  int GetSensorCount();

  // Initialization.
  void Initialize();
  void StartSensorThread(int sensor_index);
  void Shutdown();

  // Replay.
  bool RecordSensor(int sensor_index, const std::string& filename);
  bool StartPlaySensor(int sensor_index, const std::string& filename);
  bool PlayNextFrame(int sensor_index);

  // Access data.
  const KinectSensorData* GetSensorData(int sensor_index) const {
    return sensor_state_[sensor_index].GetData();
  }
  void AddObserver(int sensor_index, KinectObserver* observer);

  GestureController* GetGestureControllerInstance() {return gestureContInst_;}

 private:
  KinectWrapper();
	~KinectWrapper();

  struct SensorThreadParams {
    KinectWrapper* wrapper;
    int sensor_index;
  };
  static DWORD SensorThread(SensorThreadParams* params);

  KinectSensor* CreateSensorByIndex(int index);

  static KinectWrapper* instance_;

  KinectSensorState sensor_state_[kMaxNumSensors];

  GestureController* gestureContInst_;

  DISALLOW_COPY_AND_ASSIGN(KinectWrapper);
};

}  // namespace kinect_wrapper
