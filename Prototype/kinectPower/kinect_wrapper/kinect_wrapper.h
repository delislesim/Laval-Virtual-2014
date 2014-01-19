#pragma once

#include <opencv2/core/core.hpp>
#include <string>
#include <vector>

#include "base/base.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectBuffer;
class KinectSensor;
class KinectSkeleton;

// TODO(fdoray): Singleton.
class KinectWrapper {
 public:
  static KinectWrapper* instance() {
    if (instance_ == NULL)
      instance_ = new KinectWrapper();
    return instance_;
  }

  static void Release();

  void Initialize();
  void StartSensorThread(int sensor_index);
  void Shutdown();

  // Depth.
  bool QueryDepth(int sensor_index, cv::Mat* mat);

  // Skeletons.
  bool QuerySkeleton(int sensor_index, KinectSkeleton* skeleton);

 private:
  KinectWrapper();
	~KinectWrapper();

  struct SensorInfo {
    KinectSensor* sensor;
    KinectBuffer* depth_buffer;
    KinectSkeleton* skeleton_buffer;
    size_t current_skeleton_buffer;
    HANDLE thread;
    HANDLE close_event;
  };

  struct SensorThreadParams {
    KinectWrapper* wrapper;
    int sensor_index;
  };
  static DWORD SensorThread(SensorThreadParams* params);

  KinectSensor* CreateSensorByIndex(int index, std::string* error);
  int GetSensorCount();

  static KinectWrapper* instance_;

  typedef std::vector<SensorInfo> SensorInfoVector;
  SensorInfoVector sensor_info_;

  DISALLOW_COPY_AND_ASSIGN(KinectWrapper);
};

}  // namespace kinect_wrapper
