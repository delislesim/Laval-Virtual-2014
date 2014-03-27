#include "kinect_wrapper/kinect_skeleton.h"

#include <iostream>

#include "base/logging.h"
#include "base/timer.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

KinectSkeleton::KinectSkeleton()
  : tracked (false),
    polled(false) {
}

}  // namespace kinect_wrapper