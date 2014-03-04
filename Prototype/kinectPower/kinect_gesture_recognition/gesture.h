#pragma once

#define FRAME_PER_SECONDS 30

#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_skeleton.h"

enum Direction
{
  NO_DIRECTION, RIGHT, LEFT
};

class Gesture {
public:
  Gesture();
  virtual bool trackGesture(const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data) = 0;
  
protected:
  void setNbSteps(unsigned int* nbSteps);
  unsigned int* nbSteps_;
  unsigned int achievedSteps_[2];
};