#include "gesture.h"
#include <opencv2/core/core.hpp>

class PianoGesture: public Gesture {
public:
  PianoGesture();
  bool trackGesture(const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data);

};