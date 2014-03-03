#include "kinect_interaction/interaction_client_menu.h"

namespace kinect_interaction {

namespace {
static InteractionClientMenu* interaction_client_menu_instance = NULL;
}  // namespace

InteractionClientMenu::InteractionClientMenu() {
}

InteractionClientMenu* InteractionClientMenu::instance() {
  if (interaction_client_menu_instance == NULL)
    interaction_client_menu_instance = new InteractionClientMenu();

  return interaction_client_menu_instance;
}

void InteractionClientMenu::ReleaseInstance() {
  delete interaction_client_menu_instance;
  interaction_client_menu_instance = NULL;
}

STDMETHODIMP InteractionClientMenu::GetInteractionInfoAtLocation(
    DWORD /* skeletonTrackingId */,
    NUI_HAND_TYPE handType,
    FLOAT /*x */,
    FLOAT /* y */,
    NUI_INTERACTION_INFO* pInteractionInfo) {
  if (!pInteractionInfo)
    return E_POINTER;

  /*
  if (NUI_HAND_TYPE_RIGHT == handType)
  {
    pInteractionInfo->IsGripTarget = false;
    pInteractionInfo->PressTargetControlId = 1;
  }
  else if (NUI_HAND_TYPE_LEFT == handType)
  {
    pInteractionInfo->IsGripTarget = false;
    pInteractionInfo->PressTargetControlId = 2;
  }
  else if (NUI_HAND_TYPE_NONE == handType)
  {
    pInteractionInfo->PressTargetControlId = 0;
  }
  */
  pInteractionInfo->IsGripTarget = true;

  pInteractionInfo->PressAttractionPointX = 0.f;
  pInteractionInfo->PressAttractionPointY = 0.f;

  return S_OK;
}

}  // namespace kinect_interaction