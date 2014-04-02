using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GestureGuitar : Gesture {

	private const float minHeightLeftFromHead_ = -0.1f;
	private const float minHeightRightFromHead_ = -0.25f;
	private const float maxHeightFromHead = -0.03f;
	private const float minDistanceToHip_ = 0.25f;
	private const float gestureTime_ = 2.0f;
	private const float maxHandsDepth_ = 0.33f;
	private const float gestureTimeout_ = 0.5f;
	private const float minRightHandSpeed_ = 0.3f;

	private float elapsedTime_ = 0;
	private float elapsedTimeout_ = 0;

	// Registered position of each hand for the last move
	private float[] previousHandsPosition_;

	public GestureGuitar()
	{
		gestureId_ = GestureId.GESTURE_GUITAR;
		previousHandsPosition_ = new float[2]{0,0};
	}

	public override void Reset() {
		elapsedTime_ = 0;
	}

	public override bool trackGesture (Skeleton skeleton)
	{
		if(!skeleton.Exists())
		{
			Reset();
			return false;
		}

		// Gather joint positions
		Vector3 rightHandPos, leftHandPos, hipLeftPos, headPos;
		Skeleton.JointStatus rightHandStatus, leftHandStatus, hipLeftStatus, headStatus;
		
		rightHandStatus = skeleton.GetJointPosition (Skeleton.Joint.HandRight, out rightHandPos);
		leftHandStatus = skeleton.GetJointPosition (Skeleton.Joint.HandLeft, out leftHandPos);
		hipLeftStatus = skeleton.GetJointPosition (Skeleton.Joint.HipLeft, out hipLeftPos);
		headStatus = skeleton.GetJointPosition (Skeleton.Joint.Head, out headPos);

		if(rightHandStatus == Skeleton.JointStatus.NotTracked || leftHandStatus == Skeleton.JointStatus.NotTracked
		   || hipLeftStatus == Skeleton.JointStatus.NotTracked) {
			return false;
		}

		// Compute current speed
		float[] speed = getSpeed(leftHandPos, rightHandPos);

		//bool isHighEnough = (rightHandPos [1] - headPos [1]) >= minHeightRightFromHead_ && (leftHandPos [1] - headPos [1]) >= minHeightLeftFromHead_;
		bool handsNotTooFar = hipLeftPos[2] - rightHandPos[2] <= maxHandsDepth_ && hipLeftPos[2] - leftHandPos[2] <= maxHandsDepth_;
		bool leftHandFarEnough = Mathf.Abs(leftHandPos [0] - hipLeftPos [0]) >= minDistanceToHip_;
		bool rightHandMoving = speed[1] >= minRightHandSpeed_;
		bool isLowEnough = (rightHandPos[1] - headPos[1]) <= maxHeightFromHead;
		//Debug.Log ("hands not too far : " + handsNotTooFar + "\nleft hand far enough : " + leftHandFarEnough + "\n");
		//Debug.Log ((rightHandPos [0] - leftHandPos [0]) + "\n");
		//Debug.Log ((hipLeftPos[2] - rightHandPos[2]) + " " + (hipLeftPos[2] - leftHandPos[2]) + "\n");
		//Debug.Log ((leftHandPos [0] - hipLeftPos [0]) + "\n");

		previousHandsPosition_ [0] = leftHandPos.x;
		previousHandsPosition_ [1] = rightHandPos.x;

		if (handsNotTooFar && leftHandFarEnough && rightHandMoving && isLowEnough) {
			elapsedTime_ += GestureRecognition.deltaTime;
			elapsedTimeout_ = 0;
		}
		else
		{
			elapsedTimeout_ += GestureRecognition.deltaTime;
		}

		if(elapsedTimeout_ >= gestureTimeout_)
		{
			elapsedTime_ = 0;
			elapsedTimeout_ = 0;
		}
		else
		{
			if(elapsedTime_ >= gestureTime_)
			{
				elapsedTime_ = 0;
				elapsedTimeout_ = 0;
				return true;
			}
		}

		return false;

	}

	private float[] getSpeed(Vector3 posActuelleGauche, Vector3 posActuelleDroite)
	{
		float leftHandSpeed = Mathf.Abs (posActuelleGauche.x - previousHandsPosition_ [0]) / (GestureRecognition.deltaTime);
		float rightHandSpeed = Mathf.Abs (posActuelleDroite.x - previousHandsPosition_ [1]) / (GestureRecognition.deltaTime);
		
		float[] speeds = new float[2]{leftHandSpeed, rightHandSpeed};
		return speeds;
	}

	public override float isPartiallyTracked() {
		return elapsedTime_ / gestureTime_;
	}
}
