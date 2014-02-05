#include "intel_hand_tracker/intel_hand_tracker.h"
#include "kinect_wrapper/kinect_wrapper.h"

namespace intel_hand_tracker {

namespace {

const float	kHandWidth = 0.08f;
const float	kHandLength = 0.19f;

const int kNumTrackedHands = 1;
const hskl_model kHandModelType = HSKL_MODEL_RIGHT_HAND;

}  // namespace

IntelHandTracker* IntelHandTracker::instance_ = NULL;

IntelHandTracker::IntelHandTracker() : initialized_(false) {
}

IntelHandTracker::~IntelHandTracker() {
}

bool IntelHandTracker::Initialize() {
  if (initialized_)
    return true;

  // Initialize kinect to feed the tracking algorithm with kinect values
  kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0)->color_stream_type(NUI_IMAGE_TYPE_COLOR_INFRARED);
  kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0)->depth_stream_type(NUI_IMAGE_TYPE_DEPTH);

  if (!tracker_.Init())
    return false;

  tracker_.SetHandMeasurements(kHandWidth, kHandLength);
  tracker_.SetModelType(kHandModelType);

  initialized_ = true;
  return true;
}

void IntelHandTracker::GetFrame(hskl::float3* positions,
                                float* tracking_error) {
  tracker_.Update();

  for (int i = 0; i < tracker_.GetBoneCount(); ++i) {
    positions[i] = tracker_.GetPosition(i);
    tracking_error[i] = tracker_.GetTrackingError(i);
  }
}

}  // namespace intel_hand_tracker