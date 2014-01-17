#pragma once

#include <string>
#include <vector>

#include "base/base.h"
#include "kinect_wrapper/kinect_sensor.h"

namespace kinect_wrapper {

// TODO(fdoray): Singleton.
class KinectWrapper {
 public:
	KinectWrapper();
	~KinectWrapper();

  KinectSensor* CreateSensorByIndex(int index, std::string* error);
  int GetSensorCount();

 private:
  typedef std::vector<KinectSensor*> SensorVector;
  SensorVector sensors_;

  DISALLOW_COPY_AND_ASSIGN(KinectWrapper);
};

}  // namespace kinect_wrapper
