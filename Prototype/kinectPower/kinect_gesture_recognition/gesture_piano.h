#include "gesture.h"
#include <opencv2/core/core.hpp>

#define LATERAL_MOVEMENT 0.3
#define GESTURE_TIMEOUT 3
#define MOVES_NUMBER 3

enum PianoMove
{
  NO_MOVE_DETECTED, HANDS_PLACED, FIRST_LATERAL_MOVE, SUBSEQUENT_LATERAL_MOVES
};

class PianoGesture: public Gesture {
public:
  PianoGesture();
  bool trackGesture(const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data);
  
private:

  void updateHandPositionX(float rightHandPos, float leftHandPos);

  unsigned int elapsedTimeSinceLastMove_;

  // Id of last detected move in the sequence
  PianoMove currentMove_;

  // Number of lateral moves to activate the piano
  unsigned int nbLateralMoves_;

  // Registered position of each hand for the last move
  float lastMoveHandXPosition_[2];

};