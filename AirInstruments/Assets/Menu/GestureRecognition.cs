using UnityEngine;
using System.Collections;
using KinectHelpers;
using System.Collections.Generic;

public enum GestureId
{
	NO_GESTURE = -1, 
	GESTURE_PIANO = 0,
	GESTURE_MENU = 1
}

public class GestureRecognition : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		AddGesture (new GesturePiano ());
		AddGesture (new GestureMenu ());
	}
	
	// Update is called once per frame
	void Update () {
		skeleton_.ReloadSkeleton ();
		GestureId currentId = GestureId.NO_GESTURE;
		// Poll gesture controller to see if a gesture has been detected
		foreach(Gesture gesture in gestureList)
		{
		   if(gesture.trackGesture(skeleton_))
				currentId = gesture.GestureId_;
		}
		//print(currentId);
		//print(gestureId[0]);
	}

	void AddGesture(Gesture gesture)
	{
		gestureList.Add(gesture);
	}

	Skeleton skeleton_ = new Skeleton(0);

	List<Gesture> gestureList = new List<Gesture>();

}
