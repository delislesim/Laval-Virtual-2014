#include "kinect_lib/lib.h"

#include <iostream>
#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"

using namespace kinect_wrapper;

bool Initialize(bool near_mode) {
  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Initialize();
  wrapper->GetSensorByIndex(0)->SetNearModeEnabled(near_mode);
  wrapper->StartSensorThread(0);
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
  if (!wrapper->QueryDepth(0, &mat))
    return false;

  NiceImageFromDepthMat(mat, kMaxDepth, kMinDepth, kColorDepth,
                        pixels, pixels_size);

  return true;
}

bool GetColorImage(unsigned char* pixels, unsigned int pixels_size) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->QueryColor(0, &mat))
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

  KinectSkeletonFrame skeleton_frame;
  wrapper->QuerySkeletonFrame(0, &skeleton_frame);
 
  KinectSkeleton skeleton;
  if (!skeleton_frame.GetTrackedSkeleton(skeleton_id, &skeleton))
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

  KinectSkeletonFrame skeleton_frame;
  wrapper->QuerySkeletonFrame(0, &skeleton_frame);

  KinectSkeleton skeleton;
  if (!skeleton_frame.GetTrackedSkeleton(skeleton_id, &skeleton))
    return false;

  skeleton.CalculateBoneOrientations(bone_orientations);
  return true;
}

bool GetJointsPositionDepth(int skeleton_id, int* joint_positions) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  KinectSkeletonFrame skeleton_frame;
  wrapper->QuerySkeletonFrame(0, &skeleton_frame);

  KinectSkeleton skeleton;
  if (!skeleton_frame.GetTrackedSkeleton(skeleton_id, &skeleton))
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