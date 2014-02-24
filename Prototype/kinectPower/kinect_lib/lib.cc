#include "kinect_lib/lib.h"

#include <iostream>
#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "creative/creative_wrapper.h"
#include "kinect_interaction/interaction_client_menu.h"
#include "kinect_interaction/interaction_frame.h"
#include "kinect_face_tracker/face_tracker.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"
#include "piano/piano.h"

using namespace kinect_wrapper;

namespace {
// TODO(fdoray)
static piano::Piano the_piano;

static kinect_face_tracker::FaceTracker the_face_tracker;

}  // namespace

bool Initialize(bool near_mode, bool with_sensor_thread) {
  KinectWrapper::Release();

  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Initialize();
  wrapper->AddObserver(0, &the_piano);
  wrapper->AddObserver(0, &the_face_tracker);

  if (with_sensor_thread) {
    // Check that the expected sensors are connected.
    if (wrapper->GetSensorCount() != 1)
      return false;

	the_face_tracker.initializeTracker();

    // Initialize sensor 0.
    wrapper->GetSensorByIndex(0)->SetNearModeEnabled(near_mode);
    wrapper->GetSensorByIndex(0)->OpenDepthStream();
    wrapper->GetSensorByIndex(0)->OpenColorStream();
    wrapper->GetSensorByIndex(0)->OpenSkeletonStream();
    wrapper->GetSensorByIndex(0)->OpenInteractionStream(
        kinect_interaction::InteractionClientMenu::instance());
    wrapper->StartSensorThread(0);
  }
  return true;
}

bool Shutdown() {
  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Shutdown();
  return true;
}

bool RecordSensor(int sensor_index, const char* filename) {
  KinectWrapper* wrapper = KinectWrapper::instance();
  return wrapper->RecordSensor(sensor_index, filename);
}

bool StartPlaySensor(int sensor_index, const char* filename) {
  KinectWrapper* wrapper = KinectWrapper::instance();
  return wrapper->StartPlaySensor(sensor_index, filename);
}

bool PlayNextFrame(int sensor_index) {
  KinectWrapper* wrapper = KinectWrapper::instance();
  return wrapper->PlayNextFrame(sensor_index);
}

bool GetDepthImage(unsigned char* pixels, unsigned int pixels_size) {
  const int kMinDepth = 690;
  const int kColorDepth = 700;
  const int kMaxDepth = 3500;
   
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->GetSensorData(0)->QueryDepth(&mat))
    return false;

  NiceImageFromDepthMat(mat, kMaxDepth, kMinDepth, kColorDepth,
                        pixels, pixels_size);

  return true;
}

bool GetColorImage(unsigned char* pixels, unsigned int pixels_size) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->GetSensorData(0)->QueryColor(&mat))
    return false;

  assert(pixels_size == mat.total() * mat.elemSize());

  memcpy_s(pixels, pixels_size, mat.ptr(), mat.total() * mat.elemSize());
  for (size_t i = 3; i < pixels_size; i += 4)
    pixels[i] = 255;

  return true;
}

bool GetJointsPosition(int skeleton_id, float* joint_positions,
                       unsigned char* joint_status) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  const KinectSkeletonFrame* skeleton_frame =
      wrapper->GetSensorData(0)->GetSkeletonFrame();
 
  KinectSkeleton skeleton;
  if (!skeleton_frame->GetTrackedSkeleton(skeleton_id, &skeleton))
    return false;

  for (int joint_index = 0;
       joint_index < KinectSkeleton::JointCount; ++joint_index) {
    cv::Vec3f pos;
    KinectSkeleton::JointIndex joint =
        static_cast<KinectSkeleton::JointIndex>(joint_index);

    KinectSkeleton::JointStatus status = KinectSkeleton::NOT_TRACKED;
    skeleton.GetJointPosition(joint, &pos, &status);

    joint_status[joint_index] = static_cast<unsigned char>(status);

    joint_positions[joint_index*3 + 0] = pos[0];
    joint_positions[joint_index*3 + 1] = pos[1];
    joint_positions[joint_index*3 + 2] = pos[2];
  }

  return true;
}

bool GetBonesOrientation(int skeleton_id,
                         NUI_SKELETON_BONE_ORIENTATION* bone_orientations) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  const KinectSkeletonFrame* skeleton_frame =
      wrapper->GetSensorData(0)->GetSkeletonFrame();

  KinectSkeleton skeleton;
  if (!skeleton_frame->GetTrackedSkeleton(skeleton_id, &skeleton))
    return false;

  skeleton.CalculateBoneOrientations(bone_orientations);
  return true;
}

bool GetJointsPositionDepth(int skeleton_id, int* joint_positions) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  const KinectSkeletonFrame* skeleton_frame =
      wrapper->GetSensorData(0)->GetSkeletonFrame();

  KinectSkeleton skeleton;
  if (!skeleton_frame->GetTrackedSkeleton(skeleton_id, &skeleton))
    return false;

  KinectSensor* sensor = wrapper->GetSensorByIndex(0);

  for (int joint_index = 0;
    joint_index < KinectSkeleton::JointCount; ++joint_index) {
    Vector4 pos_skeleton;
    KinectSkeleton::JointIndex joint =
        static_cast<KinectSkeleton::JointIndex>(joint_index);

    KinectSkeleton::JointStatus status = KinectSkeleton::NOT_TRACKED;
    skeleton.GetJointPositionRaw(joint, &pos_skeleton, &status);

    cv::Vec2i pos_depth(0, 0);
    int depth = 0;

    if (status != KinectSkeleton::NOT_TRACKED)
      sensor->MapSkeletonPointToDepthPoint(pos_skeleton, &pos_depth, &depth);

    joint_positions[joint_index * 3 + 0] = pos_depth.val[0];
    joint_positions[joint_index * 3 + 1] = pos_depth.val[1];
    joint_positions[joint_index * 3 + 2] = depth;
  }

  return true;
}

bool GetHandsInteraction(int skeleton_id, NUI_HANDPOINTER_INFO* hands) {
  assert(hands);

  KinectWrapper* wrapper = KinectWrapper::instance();

  const kinect_interaction::InteractionFrame* interaction_frame =
      wrapper->GetSensorData(0)->GetInteractionFrame();

  return interaction_frame->GetHands(skeleton_id,
                                     &hands[0], &hands[1]);
}

bool GetFaceRotation(float* face_rotation) {
	assert(face_rotation);

	if (the_face_tracker.isTracking()) {
		cv::Vec3f rotation = the_face_tracker.FaceRotation();
		face_rotation[0] = rotation[0];
		face_rotation[1] = rotation[1];
		face_rotation[2] = rotation[2];
		return true;
	}
	else {
		return false;
	}
}

#ifdef USE_INTEL_CAMERA

creative::CreativeWrapper creative_wrapper;

bool InitializeHandTracker() {
  creative_wrapper.Initialize();
  return true;
}

bool GetHandsSkeletons(creative::JointInfo* joints) {
  creative_wrapper.UpdateJoints();
  creative_wrapper.QueryJoints(joints);
  return true;
}

#endif

bool GetPianoImage(unsigned char* pixels, unsigned int pixels_size) {
  the_piano.QueryNiceImage(pixels, pixels_size);
  return true;
}

bool GetPianoFingers(unsigned int* values, unsigned char* known) {
  const int kNumFingers = 25;

  finger_finder::FingerInfoVector fingers;
  the_piano.QueryFingers(&fingers);

  int i = 0;
  for (; i < kNumFingers && i < static_cast<int>(fingers.size()); ++i) {
    finger_finder::FingerInfo finger_info = fingers[i];
    values[i*3 + 0] = finger_info.position().x;
    values[i*3 + 1] = finger_info.position().y;
    values[i*3 + 2] = finger_info.depth();
    known[i] = 1;
  }

  for (; i < kNumFingers; ++i) {
    known[i] = 0;
  }

  return true;
}