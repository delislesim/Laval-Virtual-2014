#include "gesture_controller.h"
#include "gesture_piano.h"
#include <iostream>

GestureController::GestureController():detectedGesture_(GestureID::NONE)
{
  PianoGesture* pianoGesture = new PianoGesture();
  AddGesture(pianoGesture);
}

void GestureController::AddGesture(Gesture* gesture)
{
  gestureList_.push_back(gesture);
}

void GestureController::ObserveSkeleton( const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data )
{

  for(int i = 0; i < gestureList_.size();i++)
  {
     if((gestureList_[i])->trackGesture(frame, data))
     {
        detectedGesture_ = (GestureID) i;
        std::cout << i;
     }
  }
}

GestureID GestureController::GetGestureStatus()
{
  return detectedGesture_;
}

