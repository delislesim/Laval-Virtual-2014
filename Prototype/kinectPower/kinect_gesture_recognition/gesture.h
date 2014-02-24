#pragma once

#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_include.h"

class Gesture {
public:
  void trackGesture();
  
private:
  unsigned int nbSteps_;
  unsigned int achievedSteps_;
};