﻿using System.Collections;
using KinectHelpers;
using UnityEngine;

public class GesturePiano : Gesture {
	
	public GesturePiano()
	{
		elapsedTime_ = 0;
		nbSteps_ = 30;
		lastMoveHandXPosition_ = new float[2];
		lastMoveHandXPosition_[0] = 0;
		lastMoveHandXPosition_[1] = 0;
		gestureId_ = GestureId.GESTURE_PIANO;
	}
	
	public override bool trackGesture(Skeleton skeleton)
	{
		// Track the gesture
		if(!skeleton.Exists())
		{
			return false;
		}
		
		// Get relevant joint positions for lower bound
		Vector3 leftHandPos, rightHandPos, leftHipPos, centerHipPos, rightHipPos;
		Skeleton.JointStatus leftHandStatus, rightHandStatus, leftHipStatus, centerHipStatus, rightHipStatus;

		leftHandStatus = skeleton.GetJointPosition(Skeleton.Joint.HandLeft, out leftHandPos);
		rightHandStatus = skeleton.GetJointPosition(Skeleton.Joint.HandRight, out rightHandPos);
		leftHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipLeft, out leftHipPos);
		centerHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipCenter, out centerHipPos);
		rightHipStatus = skeleton.GetJointPosition(Skeleton.Joint.HipRight, out rightHipPos);
		
		Vector3 rightKneePos, leftKneePos;
		Skeleton.JointStatus rightKneeStatus, leftKneeStatus;
		
		rightKneeStatus = skeleton.GetJointPosition(Skeleton.Joint.KneeLeft, out leftKneePos);
		leftKneeStatus = skeleton.GetJointPosition(Skeleton.Joint.KneeRight, out rightKneePos);
		
		// Upper limit to hand position
		Vector3 headPos;
		Skeleton.JointStatus headStatus;
		
		headStatus = skeleton.GetJointPosition(Skeleton.Joint.Head, out headPos);
		
		// Verify every joint is currently tracked
		if(leftHandStatus == Skeleton.JointStatus.NotTracked || rightHandStatus == Skeleton.JointStatus.NotTracked || rightHipStatus == Skeleton.JointStatus.NotTracked
		   || centerHipStatus == Skeleton.JointStatus.NotTracked || leftHipStatus == Skeleton.JointStatus.NotTracked
		   || rightKneeStatus == Skeleton.JointStatus.NotTracked || leftKneeStatus == Skeleton.JointStatus.NotTracked) {
			return false;
		}
		
		if(headStatus == Skeleton.JointStatus.NotTracked)
		{
			return false;
		}
		
		
		// Compute average hip and shoulder position and compare with each hand pos
		Vector3 avgHipPos = (centerHipPos + rightHipPos + leftHipPos) / (3.0f);
		Vector3 avgKneePos = (leftKneePos + rightKneePos) / (2.0f);
		
		float upperLimit = headPos[1];//((avgShouldPos[1] - avgHipPos[1]) * (2.0f/3.0f)) + avgHipPos[1];
		float lowerLimit = ((avgHipPos[1] - avgKneePos[1]) * (2.0f/3.0f)) + avgKneePos[1];
		
		bool isInLimits = lowerLimit < leftHandPos[1] && lowerLimit < rightHandPos[1] && leftHandPos[1] < upperLimit && rightHandPos[1] < upperLimit;

		//Debug.Log ("Left hand : " + leftHandPos [0] + " " + leftHandPos [1] + "\n");
		//Debug.Log ("Right hand : " + rightHandPos [0] + " " + rightHandPos [1] + "\n");

		//Debug.Log ("Hands depth : " + leftHandPos [2] + " right : " + rightHandPos [2] + "\n");

		// Verify that current hand depth is within bounds
		bool handsFarEnough = (centerHipPos [2] - leftHandPos[2]) >= minHandDepth_ && (centerHipPos [2] - rightHandPos[2]) >= minHandDepth_;

		if(handsFarEnough && isInLimits)
		{
			elapsedTime_ += Time.deltaTime;
		}
		else
		{
			elapsedTime_ = 0;
			return false;
		}

		if(elapsedTime_ >= gestureTime_)
		{
			elapsedTime_ = 0;
			return true;
		}
		else
			return false;
	}

	public override float isPartiallyTracked() {
		return elapsedTime_ / gestureTime_;
	}
	
	private void updateHandPositionX(float rightHandPos, float leftHandPos )
	{
		lastMoveHandXPosition_[0] = rightHandPos;
		lastMoveHandXPosition_[1] = leftHandPos;
	}
	
	private float elapsedTime_;
	
	// Registered position of each hand for the last move
	private float[] lastMoveHandXPosition_;

	// Constants
	private const float LATERAL_MOVEMENT = 0.3f;
	private const float gestureTime_ = 2.0f;
	private const uint MOVES_NUMBER = 3;
	private const float minHandDepth_ = 0.18f;

}
