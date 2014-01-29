#include "kinect_power/lib.h"

#include <iostream>
#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"
#include "piano/piano.h"

using namespace kinect_wrapper;

namespace {

piano::Piano the_piano;

}  // namespace


bool Initialize() {
  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Initialize();
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

bool GetNiceDepthMap(unsigned char* pixels, unsigned int pixels_size) {
  const int kMinDepth = 690;
  const int kColorDepth = 700;
  const int kMaxDepth = 2500;
   
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->QueryDepth(0, &mat))
    return false;

  NiceImageFromDepthMat(mat, kMaxDepth, kMinDepth, kColorDepth,
                        pixels, pixels_size);

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

bool GetPianoInfo(unsigned char* notes, unsigned int notes_size,
                  unsigned char* pixels, unsigned int pixels_size) {
  the_piano.QueryNiceImage(pixels, pixels_size);
  the_piano.QueryNotes(notes, notes_size);

  return true;
}
