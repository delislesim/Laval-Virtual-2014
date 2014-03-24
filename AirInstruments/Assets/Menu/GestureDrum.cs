using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GestureDrum : Gesture {

	private bool gestureActivated_;
	private float[] handsVerticalSpeed_;
	private float elapsedTimeTimeout_;
	private float elapsedTimeGesture_;
	private float[] previousHandsPosition_;

	private const float gestureTimeout_ = 0.6f;
	private const float gestureTime_ = 1.5f;

	private const float minHandDepth_ = 0.12f;
	private const float minHandsSpeed_ = 0.13f;

	public GestureDrum()
	{
		gestureActivated_ = false;
		handsVerticalSpeed_ = new float[2]{0,0};
		elapsedTimeGesture_ = 0;
		elapsedTimeTimeout_ = 0;
		previousHandsPosition_ = new float[2]{0,0};
		gestureId_ = GestureId.GESTURE_DRUM;
	}

	public override void Reset() {
		elapsedTimeGesture_ = 0;
	}

	public override bool trackGesture (Skeleton skeleton)
	{		
		if(!skeleton.Exists())
		{
			Reset();
			return false;
		}

		// Gather relevant joint positions

		Vector3 leftHandPos, rightHandPos, leftHipPos, centerHipPos, rightHipPos;
		Skeleton.JointStatus leftHandStatus, rightHandStatus, leftHipStatus, centerHipStatus, rightHipStatus;

		Vector3 leftKneePos, rightKneePos, headPos;
		Skeleton.JointStatus leftKneeStatus, rightKneeStatus, headStatus;

		leftHandStatus = skeleton.GetJointPosition(Skeleton.Joint.HandLeft, out leftHandPos);
		rightHandStatus = skeleton.GetJointPosition(Skeleton.Joint.HandRight, out rightHandPos);
		leftHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipLeft, out leftHipPos);
		centerHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipCenter, out centerHipPos);
		rightHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipRight, out rightHipPos);
		rightKneeStatus = skeleton.GetJointPosition(Skeleton.Joint.KneeLeft, out leftKneePos);
		leftKneeStatus = skeleton.GetJointPosition(Skeleton.Joint.KneeRight, out rightKneePos);
		headStatus = skeleton.GetJointPosition(Skeleton.Joint.Head, out headPos);

		// Verify every joint is currently tracked
		if(leftHandStatus == Skeleton.JointStatus.NotTracked || rightHandStatus == Skeleton.JointStatus.NotTracked || rightHipStatus == Skeleton.JointStatus.NotTracked
		   || centerHipStatus == Skeleton.JointStatus.NotTracked || leftHipStatus == Skeleton.JointStatus.NotTracked
		   || rightKneeStatus == Skeleton.JointStatus.NotTracked || leftKneeStatus == Skeleton.JointStatus.NotTracked
		   || headStatus == Skeleton.JointStatus.NotTracked) {
			return false;
		}

		// Compute some averages for certain body parts
		Vector3 avgHipPos = (centerHipPos + rightHipPos + leftHipPos) / (3.0f);
		Vector3 avgKneePos = (leftKneePos + rightKneePos) / (2.0f);

		// Verify if hands are held at an average position and in front of the body
		float upperLimit = headPos[1];
		float lowerLimit = ((avgHipPos[1] - avgKneePos[1]) * (2.0f/3.0f)) + avgKneePos[1];
		
		bool isInLimits = lowerLimit < leftHandPos[1] && lowerLimit < rightHandPos[1] && leftHandPos[1] < upperLimit && rightHandPos[1] < upperLimit;
		bool handsFarEnough = (centerHipPos [2] - leftHandPos[2]) >= minHandDepth_ && (centerHipPos [2] - rightHandPos[2]) >= minHandDepth_;

		// If first time hands are in position, activate
		if(!gestureActivated_)
		{
			if(isInLimits && handsFarEnough)
			{
				gestureActivated_ = true;
				return false;
			}
			else
			{
				return false;
			}
		}

		// If activated verify that speed is high enough
		float leftHandSpeed = Mathf.Abs (leftHandPos [1] - previousHandsPosition_ [0]) / (Time.deltaTime);
		float rightHandSpeed = Mathf.Abs (rightHandPos [1] - previousHandsPosition_ [1]) / (Time.deltaTime);

		previousHandsPosition_[0] = leftHandPos[1];
		previousHandsPosition_[1] = rightHandPos[1];

		//Debug.Log(" Left hand speed : " + leftHandSpeed + "\n");
	    
		bool speedHighEnough = (leftHandSpeed >= minHandsSpeed_) && (rightHandSpeed >= minHandsSpeed_);

		//Debug.Log ("Speed high enough : " + speedHighEnough + "\n");

		// If any condition is not respected tick the failure timeout
		if(!speedHighEnough || !isInLimits || !handsFarEnough)
		{
			elapsedTimeTimeout_ += Time.deltaTime;
			return false;
		}
		// Else tick gesture time and reset failure timeout
		else
		{
			elapsedTimeGesture_ += Time.deltaTime;
			elapsedTimeTimeout_ = 0;
		}

		if(elapsedTimeTimeout_ >= gestureTimeout_)
		{
			elapsedTimeGesture_ = 0;
			gestureActivated_ = false;
			return false;
		}
		else 
		{
			if(elapsedTimeGesture_ >= gestureTime_)
			{
				//Debug.Log("VOICI LE SAUVEUR");
				elapsedTimeGesture_ = 0;
				elapsedTimeTimeout_ = 0;
				gestureActivated_ = false;
				return true;
			}
		}


		return false;
	}

	public override float isPartiallyTracked() {
		return elapsedTimeGesture_ / gestureTime_;
	}
}
