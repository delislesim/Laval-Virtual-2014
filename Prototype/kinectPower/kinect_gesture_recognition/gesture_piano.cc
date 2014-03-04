#include "gesture_piano.h"
#include <iostream>

PianoGesture::PianoGesture() : elapsedTimeSinceLastMove_(0), currentMove_(NO_MOVE_DETECTED), nbLateralMoves_(0)
{
  nbSteps_ = new unsigned int[2];
  nbSteps_[0] = 30;
  nbSteps_[1] = 30;
  lastMoveHandXPosition_[0] = 0;
  lastMoveHandXPosition_[1] = 0;
}

bool PianoGesture::trackGesture( const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data )
{
  // Track the gesture for both skeletons if necessary
  for(int i=0; i < 2; i++)
  {
    kinect_wrapper::KinectSkeleton curSkel;
    if(!frame.GetTrackedSkeleton(0, &curSkel))
    {
      achievedSteps_[i] = 0;
      continue;
    }

    // Get relevant joint positions for lower bound
    cv::Vec3f leftHandPos, rightHandPos, leftHipPos, centerHipPos, rightHipPos;
    kinect_wrapper::KinectSkeleton::JointStatus leftHandStatus, rightHandStatus, leftHipStatus, centerHipStatus, rightHipStatus;

    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::HandLeft, &leftHandPos, &leftHandStatus);
    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::HandRight, &rightHandPos, &rightHandStatus);
    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::HipLeft, &leftHipPos, &leftHipStatus);
    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::HipCenter, &centerHipPos, &centerHipStatus);
    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::HipRight, &rightHipPos, &rightHipStatus);

    cv::Vec3f rightKneePos, leftKneePos;
    kinect_wrapper::KinectSkeleton::JointStatus rightKneeStatus, leftKneeStatus;

    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::KneeLeft, &leftKneePos, &leftKneeStatus);
    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::KneeRight, &rightKneePos, &rightKneeStatus);

    // Upper limit to hand position
    cv::Vec3f headPos;
    kinect_wrapper::KinectSkeleton::JointStatus headStatus;

    curSkel.GetJointPosition(kinect_wrapper::KinectSkeleton::Head, &headPos, &headStatus);

    // Verify every joint is currently tracked
    if(leftHandStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || rightHandStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || rightHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED
       || centerHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || leftHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED){
         achievedSteps_[i] = 0;
         continue;
    }

    if(headStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED )
    {
      achievedSteps_[i] = 0;
      continue;
    }


    // Compute average hip and shoulder position and compare with each hand pos
    cv::Vec3f avgHipPos = (centerHipPos + rightHipPos + leftHipPos) / (3.0f);
    cv::Vec3f avgKneePos = (leftKneePos + rightKneePos) / (2.0f);

    float upperLimit = headPos.val[1];//((avgShouldPos[1] - avgHipPos[1]) * (2.0f/3.0f)) + avgHipPos[1];
    float lowerLimit = ((avgHipPos.val[1] - avgKneePos.val[1]) * (2.0f/3.0f)) + avgKneePos.val[1];

    bool isInLimits = lowerLimit < leftHandPos.val[1] && lowerLimit < rightHandPos.val[1] && leftHandPos.val[1] < upperLimit && rightHandPos.val[1] < upperLimit;

    switch(currentMove_)
    {
    case NO_MOVE_DETECTED:
      if(isInLimits)
      {
        currentMove_ = HANDS_PLACED;
        updateHandPositionX(rightHandPos[0], leftHandPos[0]);

        std::cout << "Move" << (int)currentMove_ << "detected"<< std::endl;
      }
      break;
    case HANDS_PLACED:
      if((abs(rightHandPos[0] - lastMoveHandXPosition_[0]) >= LATERAL_MOVEMENT || abs(leftHandPos[0] - lastMoveHandXPosition_[1]) >= LATERAL_MOVEMENT) && isInLimits)
      {
        elapsedTimeSinceLastMove_ = 0;
        currentMove_ = FIRST_LATERAL_MOVE;
        updateHandPositionX(rightHandPos[0], leftHandPos[0]);
        nbLateralMoves_++;
        std::cout << "Move" << (int)currentMove_ << "detected"<< std::endl;
      }
      else
      {
        elapsedTimeSinceLastMove_ += 1/FRAME_PER_SECONDS;
      }
      break;
    case FIRST_LATERAL_MOVE:
      if(nbLateralMoves_ + 1 == MOVES_NUMBER)
      {
        elapsedTimeSinceLastMove_ = 0;
        currentMove_ = NO_MOVE_DETECTED;
        std::cout << "Final Move" << "detected" << std::endl;
        return true;
      }
      bool hasMovedLaterally = (abs(rightHandPos[0] - lastMoveHandXPosition_[0]) >= LATERAL_MOVEMENT || abs(leftHandPos[0] - lastMoveHandXPosition_[1]) >= LATERAL_MOVEMENT);
      if(hasMovedLaterally && isInLimits)
      {
        nbLateralMoves_++;
        updateHandPositionX(rightHandPos[0], leftHandPos[0]);
        elapsedTimeSinceLastMove_ = 0;
        std::cout << "Move" << (int)currentMove_ << "detected"<< std::endl;
      }
      else
      {
        elapsedTimeSinceLastMove_ += 1/FRAME_PER_SECONDS;
      }
      break;
    }
  }

  if(elapsedTimeSinceLastMove_ > GESTURE_TIMEOUT)
  {
    currentMove_ = NO_MOVE_DETECTED;
    elapsedTimeSinceLastMove_ = 0;
  }
  return false;
}

void PianoGesture::updateHandPositionX( float rightHandPos, float leftHandPos )
{
  lastMoveHandXPosition_[0] = rightHandPos;
  lastMoveHandXPosition_[1] = leftHandPos;
}

