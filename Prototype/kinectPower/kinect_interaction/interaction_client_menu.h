#pragma once

#include "kinect_interaction/interaction_client_base.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_interaction {

class InteractionClientMenu : public InteractionClientBase {
 public:
  static InteractionClientMenu* instance();
  void ReleaseInstance();

 private:
  InteractionClientMenu();

  STDMETHODIMP GetInteractionInfoAtLocation(
      DWORD skeletonTrackingId,
      NUI_HAND_TYPE handType,
      FLOAT x,
      FLOAT y,
      NUI_INTERACTION_INFO* pInteractionInfo);
};

}  // namespace kinect_interaction