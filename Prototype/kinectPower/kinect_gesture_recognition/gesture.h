#pragma once

#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_skeleton.h"

class Gesture {
public:
  Gesture();
  virtual bool trackGesture(const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data) = 0;
  
protected:
  void setNbSteps(unsigned int* nbSteps);
  unsigned int* nbSteps_;
  unsigned int achievedSteps_[2];
};