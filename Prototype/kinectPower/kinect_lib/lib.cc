#include "kinect_lib/lib.h"

#include <iostream>
#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "creative/creative_wrapper.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"

using namespace kinect_wrapper;

bool Initialize() {
  KinectSensor* sensor = KinectSensor::Instance();
  sensor->StartSensorThread();
  return true;
}

bool Shutdown() {
  KinectSensor* sensor = KinectSensor::Instance();
  sensor->Shutdown();
  return true;
}

bool GetJoints(float* positions, float* orientations, int* tracking_state, int* is_new) {
  KinectSensor* sensor = KinectSensor::Instance();
  KinectSkeleton* body = sensor->GetLastBody();

  if (!body->tracked)
    return false;

  int index = 0;
  for (int i = 0; i < 25; ++i) {
    positions[index++] = body->positions[i].val[0];
    positions[index++] = body->positions[i].val[1];
    positions[index++] = body->positions[i].val[2];
  }

  index = 0;
  for (int i = 0; i < 25; ++i) {
    orientations[index++] = body->orientations[i].val[0];
    orientations[index++] = body->orientations[i].val[1];
    orientations[index++] = body->orientations[i].val[2];
    orientations[index++] = body->orientations[i].val[3];
  }

  for (int i = 0; i < 25; ++i) {
    tracking_state[i] = static_cast<int>(body->tracking_state[i]);
  }

  *is_new = !body->polled;
  body->polled = true;

  return true;
}

bool GetJointsPositionDepth(int* joint_positions) {
  KinectSensor* sensor = KinectSensor::Instance();
  KinectSkeleton* body = sensor->GetLastBody();

  if (!body->tracked)
    return false;

  ICoordinateMapper* mapper = sensor->GetCoordinateMapper();

  int index = 0;
  for (int i = 0; i < 25; ++i) {
    CameraSpacePoint cameraSpacePoint;
    cameraSpacePoint.X = body->positions[i].val[0];
    cameraSpacePoint.Y = body->positions[i].val[1];
    cameraSpacePoint.Z = body->positions[i].val[2];

    DepthSpacePoint depthPoint = { 0 };
    mapper->MapCameraPointToDepthSpace(cameraSpacePoint, &depthPoint);

    joint_positions[index++] = depthPoint.X;
    joint_positions[index++] = depthPoint.Y;
  }

  return true;
}

bool AvoidCurrentSkeleton() {
  /*
  KinectWrapper* wrapper = KinectWrapper::instance();
  KinectSensor* sensor = wrapper->GetSensorByIndex(0);
  if (sensor == NULL)
    return false;

  sensor->AvoidCurrentSkeleton();
  */
  return true;
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

bool SetHandMeasurements(float width, float height) {
  creative_wrapper.SetHandMeasurements(width, height);
  return true;
}

#endif
