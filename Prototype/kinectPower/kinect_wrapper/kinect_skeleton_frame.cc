#include "kinect_wrapper/kinect_skeleton_frame.h"

#include "base/logging.h"
#include "kinect_wrapper/kinect_skeleton.h"

namespace kinect_wrapper {

KinectSkeletonFrame::KinectSkeletonFrame() {
  ZeroMemory(&frame_, sizeof(frame_));
  for (int i = 0; i < kNumTrackedSkeletons; ++i)
    tracked_skeletons_[i] = 0;
}

KinectSkeletonFrame::~KinectSkeletonFrame() {
}

bool KinectSkeletonFrame::GetTrackedSkeleton(size_t tracked_skeleton_id,
                                             KinectSkeleton* skeleton) const {
  assert(skeleton != NULL);
  assert(tracked_skeleton_id < kNumTrackedSkeletons);
  
  DWORD track_id = tracked_skeletons_[tracked_skeleton_id];

  for (int i = 0; i < NUI_SKELETON_COUNT; ++i) {
    if (frame_.SkeletonData[i].dwTrackingID != track_id)
      continue;

    skeleton->SetSkeletonData(frame_.SkeletonData[i]);
    return true;
  }

  return false;
}

DWORD KinectSkeletonFrame::GetSkeletonTrackId(
    size_t tracked_skeleton_id) const {
  assert(tracked_skeleton_id < kNumTrackedSkeletons);
  return tracked_skeletons_[tracked_skeleton_id];
}

void KinectSkeletonFrame::SetSkeletonFrame(
    const NUI_SKELETON_FRAME& frame) {
  memcpy_s(&frame_, sizeof(frame_), &frame, sizeof(frame));
}

void KinectSkeletonFrame::SetTrackedSkeletons(DWORD track_id_1,
                                              DWORD track_id_2) {
  tracked_skeletons_[0] = track_id_1;
  tracked_skeletons_[1] = track_id_2;
}

}  // namespace kinect_wrapper