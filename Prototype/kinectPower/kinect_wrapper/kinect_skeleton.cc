#include "kinect_wrapper/kinect_skeleton.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectSkeleton::KinectSkeleton() {
  ZeroMemory(&data_, sizeof(data_));
}

KinectSkeleton::~KinectSkeleton() {
}

void KinectSkeleton::GetJointPosition(JointIndex joint_index,
                                      cv::Vec3f* position,
                                      JointStatus* status) {
  Vector4 position_raw;
  GetJointPositionRaw(joint_index, &position_raw, status);

  (*position)[0] = position_raw.x;
  (*position)[1] = position_raw.y;
  (*position)[2] = position_raw.z;
}

void KinectSkeleton::GetJointPositionRaw(JointIndex joint_index,
                                         Vector4* position,
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
    position->x = 0;
    position->y = 0;
    position->z = 0;
    position->w = 0;
    return;
  }
  else if (data_.eSkeletonPositionTrackingState[joint_index] ==
    NUI_SKELETON_POSITION_INFERRED) {
    *status = INFERRED;
  }
  else {
    *status = TRACKED;
  }

  *position = data_.SkeletonPositions[joint_index];
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