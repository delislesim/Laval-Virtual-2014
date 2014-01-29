#include "kinect_wrapper/kinect_skeleton.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectSkeleton::KinectSkeleton() {
  ZeroMemory(&data_, sizeof(data_));
}

KinectSkeleton::~KinectSkeleton() {
}

void KinectSkeleton::GetJointPosition(
    JointIndex joint_index,
    cv::Vec3f* position,
    JointStatus* status) {
  assert(joint_index < JointCount);
  assert(position != NULL);
  assert(status != NULL);

  if (data_.eTrackingState == NUI_SKELETON_NOT_TRACKED) {
    *status = NOT_TRACKED;
    return;
  }

  if (data_.eSkeletonPositionTrackingState[joint_index] ==
          NUI_SKELETON_POSITION_NOT_TRACKED) {
    *status = NOT_TRACKED;
    (*position)[0] = 0;
    (*position)[1] = 0;
    (*position)[2] = 0;
    return;
  } else if (data_.eSkeletonPositionTrackingState[joint_index] ==
                 NUI_SKELETON_POSITION_INFERRED) {
    *status = INFERRED;
  } else {
    *status = TRACKED;
  }

  (*position)[0] = data_.SkeletonPositions[joint_index].x; 
  (*position)[1] = data_.SkeletonPositions[joint_index].y;
  (*position)[2] = data_.SkeletonPositions[joint_index].z; 
}

void KinectSkeleton::SetSkeletonData(const NUI_SKELETON_DATA& skeleton_data) {
  memcpy_s(&data_, sizeof(data_), &skeleton_data, sizeof(NUI_SKELETON_DATA));
}

void KinectSkeleton::CalculateBoneOrientations(
    NUI_SKELETON_BONE_ORIENTATION* bone_orientations) const {
  assert(bone_orientations);
  NuiSkeletonCalculateBoneOrientations(&data_, bone_orientations);
}

}  // namespace kinect_wrapper