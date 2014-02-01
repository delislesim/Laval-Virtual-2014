#pragma once

#include "kinect_wrapper/kinect_include.h"

namespace kinect_interaction {

class InteractionClientBase : public INuiInteractionClient {
  STDMETHODIMP_(ULONG) AddRef() {
    return 2;
  }

  STDMETHODIMP_(ULONG) Release() {
    return 1;
  }

  STDMETHODIMP QueryInterface(REFIID /* riid */, void** /* ppv */) {
    return S_OK;
  }

  STDMETHODIMP GetInteractionInfoAtLocation(
      DWORD /* skeletonTrackingId */,
      NUI_HAND_TYPE /* handType */,
      FLOAT /* x */,
      FLOAT /* y */,
      NUI_INTERACTION_INFO* /* pInteractionInfo */) {
    return E_POINTER;
  }
};

}  // namespace kinect_interaction