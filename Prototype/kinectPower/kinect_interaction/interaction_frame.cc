#include "kinect_interaction/interaction_frame.h"

#include "base/logging.h"

namespace kinect_interaction {

InteractionFrame::InteractionFrame() {
  ZeroMemory(&frame_, sizeof(frame_));
  for (int i = 0; i < kNumTrackedSkeletons; ++i)
    tracked_skeletons_[i] = static_cast<DWORD>(-1);
}

InteractionFrame::~InteractionFrame() {
}

bool InteractionFrame::GetHands(int skeleton_id,
                                NUI_HANDPOINTER_INFO* left_hand,
                                NUI_HANDPOINTER_INFO* right_hand) const {
  assert(left_hand);
  assert(right_hand);

  DWORD track_id = tracked_skeletons_[skeleton_id];

  for (int i = 0; i < NUI_SKELETON_COUNT; ++i) {
    if (frame_.UserInfos[i].SkeletonTrackingId != track_id)
      continue;

    *left_hand = frame_.UserInfos[i].HandPointerInfos[0];
    *right_hand = frame_.UserInfos[i].HandPointerInfos[1];

    return true;
  }

  return false;
}

void InteractionFrame::SetTrackedSkeletons(DWORD track_id_1,
                                           DWORD track_id_2) {
  tracked_skeletons_[0] = track_id_1;
  tracked_skeletons_[1] = track_id_2;
  frame_.UserInfos[0].HandPointerInfos[0];
}

}  // namespace kinect_interaction