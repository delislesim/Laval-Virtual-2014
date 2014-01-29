#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectSkeleton {
 public:
  KinectSkeleton();
  ~KinectSkeleton();

  typedef enum _JointIndex {
    HipCenter = 0,
    Spine,
    ShoulderCenter,
    Head,
    ShoulderLeft,
    ElbowLeft,
    WristLeft,
    HandLeft,
    ShoulderRight,
    ElbowRight,
    WristRight,
    HandRight,
    HipLeft,
    KneeLeft,
    AnkleLeft,
    FootLeft,
    HipRight,
    KneeRight,
    AnkleRight,
    FootRight,
    JointCount
  } JointIndex;

  typedef enum _JointStatus {
    TRACKED = 0,
    INFERRED,
    NOT_TRACKED
  } JointStatus;

  void GetJointPosition(JointIndex joint_index,
                        cv::Vec3f* position, JointStatus* status);

  // Method to load the skeleton data from the sensor.
  void SetSkeletonData(const NUI_SKELETON_DATA& skeleton_data);

  void CalculateBoneOrientations(
      NUI_SKELETON_BONE_ORIENTATION* bone_orientations) const;

 private:
  NUI_SKELETON_DATA data_;

  DISALLOW_COPY_AND_ASSIGN(KinectSkeleton);
};

}  // namespace kinect_wrapper