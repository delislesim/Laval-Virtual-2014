﻿using KinectHelpers;

enum GestureConstants
{
	FRAME_PER_SECONDS = 30
}

enum Direction
{
	NO_DIRECTION, RIGHT, LEFT
};

public abstract class Gesture {

	public Gesture()
	{
		achievedSteps_ = 0;
	}

	public abstract bool trackGesture(Skeleton skeleton);

	// Indique de 0 a 1 a quel point le gesture est complete.
	public abstract float isPartiallyTracked();
	
	protected uint nbSteps_;
	protected uint achievedSteps_;
	protected GestureId gestureId_;

	public GestureId GestureId_
	{
		get
		{
			return gestureId_;
		}
	}

	public abstract void Reset();
};