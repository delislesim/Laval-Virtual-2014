#include "gesture_piano.h"

PianoGesture::PianoGesture()
{
  nbSteps_ = new unsigned int[2];
  nbSteps_[0] = 30;
  nbSteps_[1] = 30;
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

    // Verify that hands are within allowed bounds 
    if(lowerLimit > leftHandPos.val[1] || lowerLimit > rightHandPos.val[1] || leftHandPos.val[1] > upperLimit || rightHandPos.val[1] > upperLimit)
    {
      achievedSteps_[i] = 0;
      continue;
    }

    achievedSteps_[i]++;

    if(achievedSteps_[i] == nbSteps_[i])
      return true;
    else
      return false;
  }
  return false;
}
