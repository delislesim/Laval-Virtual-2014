#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/observer_list.h"
#include "base/scoped_handle.h"
#include "base/scoped_ptr.h"
#include "kinect_replay/kinect_player.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_sensor_data.h"

namespace kinect_wrapper {  

class KinectObserver;

class KinectSensorState {
 public:
  KinectSensorState();
  ~KinectSensorState();

  KinectSensor* GetSensor() {
    return sensor_.get();
  }
  void SetSensor(size_t sensor_index, KinectSensor* sensor) {
    sensor_.reset(sensor);
    data_.SetSensorIndex(sensor_index);
  }

  KinectSensorData* GetData() {
    return &data_;
  }
  const KinectSensorData* GetData() const {
    return &data_;
  }

  void SetThread(HANDLE thread);
  void SetCloseEvent(HANDLE close_event);
  HANDLE GetCloseEvent();
  void SendCloseEvent();
  void WaitThreadCloseAndDelete();

  void SetStatus(const std::string& status) {
    status_ = status;
  }

  std::string GetStatus() {
    return status_;
  }

  bool StartRecording(const std::string& filename);
  void StopRecording();
  bool LoadReplayFile(const std::string& filename);
  bool ReplayFrame();
  bool RecordFrame();

 private:

  scoped_ptr<KinectSensor> sensor_;
  
  KinectSensorData data_;

  base::ScopedHandle thread_;
  base::ScopedHandle close_event_;

  std::string status_;

  kinect_replay::KinectRecorder recorder_;
  kinect_replay::KinectPlayer player_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensorState);
};

}  // namespace kinect_wrapper