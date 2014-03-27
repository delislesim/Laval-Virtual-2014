#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "base/scoped_handle.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {  

namespace {
}  // namespace

// Represente le capteur Kinect. Est un singleton.
class KinectSkeleton {
 public:
  KinectSkeleton();

  // Indique si le squelette est valide.
  bool tracked;

  // Positions des joints.
  cv::Vec3f positions[JointType_Count];

  // Orientation des joints.
  cv::Vec4f orientations[JointType_Count];

  // Statut des joints.
  TrackingState tracking_state[JointType_Count];

  // Indique si le squelette a ete poll.
  bool polled;

 private:
  DISALLOW_COPY_AND_ASSIGN(KinectSkeleton);
};

}  // namespace kinect_wrapper