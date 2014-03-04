using System.Collections;
using KinectHelpers;
using UnityEngine;

enum PianoMove
{
	NO_MOVE_DETECTED, HANDS_PLACED, FIRST_LATERAL_MOVE, SUBSEQUENT_LATERAL_MOVES
}

public class GesturePiano : Gesture {
	
	public GesturePiano()
	{
		elapsedTimeSinceLastMove_ = 0;
		currentMove_ = PianoMove.NO_MOVE_DETECTED;
		nbLateralMoves_ = 0;
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
		   || centerHipStatus == Skeleton.JointStatus.NotTracked || leftHipStatus == Skeleton.JointStatus.NotTracked){
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

		switch(currentMove_)
		{
		case PianoMove.NO_MOVE_DETECTED:
			if(isInLimits)
			{
				currentMove_ = PianoMove.HANDS_PLACED;
				updateHandPositionX(rightHandPos[0], leftHandPos[0]);

				//Debug.Log("Move {0} detected\n" + (int)currentMove_);
			}
			break;
		case PianoMove.HANDS_PLACED:
			if((Mathf.Abs(rightHandPos[0] - lastMoveHandXPosition_[0]) >= LATERAL_MOVEMENT || Mathf.Abs(leftHandPos[0] - lastMoveHandXPosition_[1]) >= LATERAL_MOVEMENT) && isInLimits)
			{
				elapsedTimeSinceLastMove_ = 0;
				currentMove_ = PianoMove.FIRST_LATERAL_MOVE;
				updateHandPositionX(rightHandPos[0], leftHandPos[0]);
				nbLateralMoves_++;
				//Debug.Log("Move {0} detected\n" + (int)currentMove_);
			}
			else
			{
				elapsedTimeSinceLastMove_ += 1.0f/Time.deltaTime;
			}
			break;
		case PianoMove.FIRST_LATERAL_MOVE:
			if(nbLateralMoves_ + 1 == MOVES_NUMBER)
			{
				elapsedTimeSinceLastMove_ = 0;
				currentMove_ = PianoMove.NO_MOVE_DETECTED;
				//Debug.Log("Final Move {0} detected\n" +  (int)currentMove_);
				return true;
			}
			bool hasMovedLaterally = (Mathf.Abs(rightHandPos[0] - lastMoveHandXPosition_[0]) >= LATERAL_MOVEMENT || Mathf.Abs(leftHandPos[0] - lastMoveHandXPosition_[1]) >= LATERAL_MOVEMENT);
			if(hasMovedLaterally && isInLimits)
			{
				nbLateralMoves_++;
				updateHandPositionX(rightHandPos[0], leftHandPos[0]);
				elapsedTimeSinceLastMove_ = 0;
				//Debug.Log("Move {0} detected\n" + (int)currentMove_);
			}
			else
			{
				elapsedTimeSinceLastMove_ += 1.0f/Time.deltaTime;
			}
			break;
		}

		
		if(elapsedTimeSinceLastMove_ > GESTURE_TIMEOUT)
		{
			currentMove_ = PianoMove.NO_MOVE_DETECTED;
			elapsedTimeSinceLastMove_ = 0;
		}
		return false;
	}
	
	private void updateHandPositionX(float rightHandPos, float leftHandPos )
	{
		lastMoveHandXPosition_[0] = rightHandPos;
		lastMoveHandXPosition_[1] = leftHandPos;
	}
	
	private float elapsedTimeSinceLastMove_;
	
	// Id of last detected move in the sequence
	private PianoMove currentMove_;
	
	// Number of lateral moves to activate the piano
	private uint nbLateralMoves_;
	
	// Registered position of each hand for the last move
	private float[] lastMoveHandXPosition_;

	// Constants
	private const float LATERAL_MOVEMENT = 0.3f;
	private const float GESTURE_TIMEOUT = 3.0f;
	private const uint MOVES_NUMBER = 3;

}
