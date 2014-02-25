#pragma once

#include <vector>
#include "gesture.h"
#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_skeleton.h"
#include "kinect_Wrapper/kinect_skeleton_frame.h"

enum GestureID {
  NONE = -1
};

class GestureController:public kinect_wrapper::KinectObserver {
public:
  GestureController();
  void AddGesture(Gesture* gesture);
  void ObserveSkeleton(const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data);
  GestureID GetGestureStatus();
  
private:

  std::vector<Gesture*> gestureList_;
  GestureID detectedGesture_;
};