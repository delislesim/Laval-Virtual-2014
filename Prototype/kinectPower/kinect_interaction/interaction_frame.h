#pragma once

#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_interaction {

class InteractionFrame {
 public:
  InteractionFrame();
  ~InteractionFrame();
  
  bool GetHands(int skeleton_id,
                NUI_HANDPOINTER_INFO* left_hand,
                NUI_HANDPOINTER_INFO* right_hand) const;

  // Used to load the interaction data from the sensor.
  NUI_INTERACTION_FRAME* GetInteractionFramePtr() {
    return &frame_;
  }
  void SetTrackedSkeletons(DWORD track_id_1, DWORD track_id_2);

 private:
  NUI_INTERACTION_FRAME frame_;
  DWORD tracked_skeletons_[kNumTrackedSkeletons];

};

}  // namespace kinect_interaction