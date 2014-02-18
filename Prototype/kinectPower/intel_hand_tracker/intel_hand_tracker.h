#pragma once

#ifdef USE_INTEL_CAMERA
/*
#include "hsklu.h"

#include <cstdlib>

#include "base/base.h"

namespace intel_hand_tracker {

class IntelHandTracker {
 public:  
  static IntelHandTracker* instance() {
    if (instance_ == NULL)
      instance_ = new IntelHandTracker();
    return instance_;
  }

  static void ReleaseInstance();

  bool Initialize();

  void GetFrame(hskl::float3* positions,
                float* tracking_error);

 private:
  IntelHandTracker();
  ~IntelHandTracker();

  static IntelHandTracker* instance_;

  hskl::Tracker tracker_;

  bool initialized_;

  DISALLOW_COPY_AND_ASSIGN(IntelHandTracker);
};

}  // namespace intel_hand_tracker
*/
#endif
