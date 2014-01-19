#include "kinect_wrapper/kinect_skeleton.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectSkeleton::KinectSkeleton() {
  ZeroMemory(&data_, sizeof(data_));
}

KinectSkeleton::~KinectSkeleton() {
}

bool KinectSkeleton::GetJointPosition(
    JointIndex joint_index, cv::Vec3f* position, bool* inferred) {
  DCHECK(joint_index < JointCount);
  DCHECK(position != NULL);
  DCHECK(inferred != NULL);

  if (data_.eTrackingState == NUI_SKELETON_NOT_TRACKED)
    return false;

  if (data_.eSkeletonPositionTrackingState[joint_index] ==
          NUI_SKELETON_POSITION_NOT_TRACKED) {
    return false;
  }

  if (data_.eSkeletonPositionTrackingState[joint_index] ==
          NUI_SKELETON_POSITION_INFERRED) {
    *inferred = true;
  } else {
    *inferred = false;
  }

  (*position)[0] = data_.SkeletonPositions[joint_index].x; 
  (*position)[1] = data_.SkeletonPositions[joint_index].y;
  (*position)[2] = data_.SkeletonPositions[joint_index].z; 

  return true;
}

void KinectSkeleton::SetSkeletonData(const NUI_SKELETON_DATA& skeleton_data) {
  memcpy_s(&data_, sizeof(data_), &skeleton_data, sizeof(NUI_SKELETON_DATA));
}

}  // namespace kinect_wrapper