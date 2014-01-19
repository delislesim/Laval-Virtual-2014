#include "kinect_wrapper/kinect_skeleton.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectSkeleton::KinectSkeleton() {
  ZeroMemory(&frame_, sizeof(frame_));
}

KinectSkeleton::~KinectSkeleton() {
}

bool KinectSkeleton::GetJointPosition(
    size_t skeleton_id, JointIndex joint_index, cv::Vec3f* position,
    bool* inferred) {
  DCHECK(skeleton_id < kNumTrackedSkeletons);
  DCHECK(joint_index < JointCount);
  DCHECK(position != NULL);
  DCHECK(inferred != NULL);

  DWORD track_id = tracked_skeletons_[skeleton_id];

  for (int i = 0; i < NUI_SKELETON_COUNT; ++i) {
    if (frame_.SkeletonData[i].dwTrackingID == track_id) {
      if (frame_.SkeletonData[i].eTrackingState == NUI_SKELETON_NOT_TRACKED)
        return false;

      if (frame_.SkeletonData[i].eSkeletonPositionTrackingState[joint_index] ==
              NUI_SKELETON_POSITION_NOT_TRACKED) {
        return false;
      }

      if (frame_.SkeletonData[i].eSkeletonPositionTrackingState[joint_index] ==
              NUI_SKELETON_POSITION_INFERRED) {
        *inferred = true;
      }

      (*position)[0] = frame_.SkeletonData[i].SkeletonPositions[joint_index].x; 
      (*position)[1] = frame_.SkeletonData[i].SkeletonPositions[joint_index].y; 
      (*position)[2] = frame_.SkeletonData[i].SkeletonPositions[joint_index].z; 

      return true;
    }
  }

  return false;
}

void KinectSkeleton::SetTrackedSkeletons(DWORD track_id_1, DWORD track_id_2) {
  tracked_skeletons_[0] = track_id_1;
  tracked_skeletons_[1] = track_id_2;
}

}  // namespace kinect_wrapper