using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GestureMenu : Gesture {

	private const float distanceBetweenHands_ = 0.18f;
	private const float handsToHead_ = -0.10f;
	private const float gestureTime_ = 2.0f;
	private float elapsedTime_ = 0;

	public GestureMenu()
	{
		nbSteps_ = 0;
		achievedSteps_ = 0;
		gestureId_ = GestureId.GESTURE_MENU;
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

		Vector3 rightHandPos, leftHandPos, headPos;
		Skeleton.JointStatus rightHandStatus, leftHandStatus, headStatus;

		rightHandStatus = skeleton.GetJointPosition (Skeleton.Joint.HandRight, out rightHandPos);
		leftHandStatus = skeleton.GetJointPosition (Skeleton.Joint.HandLeft, out leftHandPos);
		headStatus = skeleton.GetJointPosition (Skeleton.Joint.Head, out headPos);

		if(rightHandStatus == Skeleton.JointStatus.NotTracked || leftHandStatus == Skeleton.JointStatus.NotTracked
		   || headStatus == Skeleton.JointStatus.NotTracked) {
			return false;
		}

		bool isHighEnough = (rightHandPos[1] - headPos[1]) >= handsToHead_ && (leftHandPos[1] - headPos[1]) >= handsToHead_;
		bool handsDistantEnough = ( rightHandPos [0] - leftHandPos [0]) >= distanceBetweenHands_;

		//Debug.Log ("Right : " + rightHandPos [1] + " Left : " + leftHandPos [1] + "\n");
		//Debug.Log ("Head : " + headPos [1] + "\n");

		//Debug.Log ("Right : " + rightHandPos [0] + " Left : " + leftHandPos [0] + "\n");
		//Debug.Log ("Hands high enough : " + isHighEnough + "\n");
		//Debug.Log ("Hands wide enough : " + handsDistantEnough + "\n");

		if (isHighEnough && handsDistantEnough) {
			//Debug.Log ("BRAVO GESTE DETECTE");
			elapsedTime_ += Time.deltaTime;
		}
		else
			elapsedTime_ = 0;

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
}
