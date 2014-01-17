#pragma once

#include "base/base.h"

namespace kinect_wrapper {

class KinectCoordinateMapper {
 public:
	KinectCoordinateMapper();
	~KinectCoordinateMapper();

 private:
  DISALLOW_COPY_AND_ASSIGN(KinectCoordinateMapper);
};

}  // namespace kinect_wrapper
