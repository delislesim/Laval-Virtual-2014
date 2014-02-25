#include "gesture_controller.h"

GestureController::GestureController():detectedGesture_(GestureID::NONE)
{

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
     }
  }
}

GestureID GestureController::GetGestureStatus()
{
  return detectedGesture_;
}

