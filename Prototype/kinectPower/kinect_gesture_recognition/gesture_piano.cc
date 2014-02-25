#include "gesture_piano.h"

PianoGesture::PianoGesture()
{
  unsigned int nbSteps[2] = {5,5};
  setNbSteps(nbSteps);
}

bool PianoGesture::trackGesture( const kinect_wrapper::KinectSkeletonFrame& frame, const kinect_wrapper::KinectSensorData& data )
{
  // Track the gesture for both skeletons if necessary
  for(int i=0; i < 2; i++)
  {
    kinect_wrapper::KinectSkeleton* curSkel;
    if(!frame.GetTrackedSkeleton(0, curSkel))
      continue;

    // Get relevant joint positions
    cv::Vec3f leftHandPos, rightHandPos, leftHipPos, centerHipPos, rightHipPos;
    kinect_wrapper::KinectSkeleton::JointStatus leftHandStatus, rightHandStatus, leftHipStatus, centerHipStatus, rightHipStatus;

    curSkel->GetJointPosition(kinect_wrapper::KinectSkeleton::HandLeft, &leftHandPos, &leftHandStatus);
    curSkel->GetJointPosition(kinect_wrapper::KinectSkeleton::HandRight, &rightHandPos, &rightHandStatus);
    curSkel->GetJointPosition(kinect_wrapper::KinectSkeleton::HipLeft, &leftHipPos, &leftHipStatus);
    curSkel->GetJointPosition(kinect_wrapper::KinectSkeleton::HipCenter, &centerHipPos, &centerHipStatus);
    curSkel->GetJointPosition(kinect_wrapper::KinectSkeleton::HipRight, &rightHipPos, &rightHipStatus);

    if(leftHandStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || rightHandStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || rightHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED
       || centerHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED || leftHipStatus == kinect_wrapper::KinectSkeleton::NOT_TRACKED){
       continue;
    }


    // Compute average hip position and compare with each hand pos
    cv::Vec3f avgHipPos = (centerHipPos + rightHipPos + leftHipPos) / (3.0f);

    if(avgHipPos[1] > leftHandPos[1] || avgHipPos[1] > rightHandPos[1])
      continue;

    achievedSteps_[i]++;

    if(achievedSteps_[i] == nbSteps_[i])
      return true;
    else
      return false;
  }
}
