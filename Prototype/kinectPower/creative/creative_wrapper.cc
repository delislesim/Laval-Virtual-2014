#include "creative/creative_wrapper.h"

namespace creative {

namespace {

const float	kHandWidth = 0.08f /* 0.0816f*/;
const float	kHandLength = 0.19f /* 0.1559f*/;

const hskl_model kHandModelType = HSKL_MODEL_TWO_HAND;

}  // namespace

CreativeWrapper::CreativeWrapper() : initialized_(false), joints_(GetNumJoints()) {
}

void CreativeWrapper::Initialize() {
  if (initialized_)
    return;

  // Initialiser le SDK de la caméra creative.
  if (!tracker_.Init()) {
    // TODO(fdoray)
    assert(false);
  }

  tracker_.SetHandMeasurements(kHandWidth, kHandLength);
  tracker_.SetModelType(kHandModelType);

  initialized_ = true;
}

void CreativeWrapper::UpdateJoints() {
  tracker_.Update();

  for (size_t i = 0; i < joints_.size(); ++i) {
    hskl::float3 position = tracker_.GetPosition(i);
    float error = tracker_.GetTrackingError(i);
    cv::Vec3f cv_position(position.x, position.y, position.z);

    JointInfo joint_info(cv_position, error);
    joints_[i] = joint_info;
  }
}

}  // namespace creative