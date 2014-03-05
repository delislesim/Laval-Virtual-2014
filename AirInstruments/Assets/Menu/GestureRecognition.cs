using UnityEngine;
using System.Collections;
using KinectHelpers;
using System.Collections.Generic;

public enum GestureId
{
	NO_GESTURE = -1, 
	GESTURE_PIANO = 0,
	GESTURE_MENU = 1,
	GESTURE_DRUM = 2
}

public class GestureRecognition : MonoBehaviour {

	public GestureRecognition() {
		// Conserver une reference a l'instance unique de reconnaisseur
		// de gestes.
		instance = this;
	}

	// Retourne l'instance unique du reconnaisseur de gestes.
	public static GestureRecognition ObtenirInstance() {
		return instance;
	}

	// Retourne le geste effectue a l'image courante.
	public GestureId GetCurrentGesture() {
		return currentId;
	}

	// Use this for initialization
	void Start () {
		AddGesture (new GesturePiano ());
		AddGesture (new GestureMenu ());
		AddGesture (new GestureDrum ());
	}
	
	// Update is called once per frame
	void Update () {
		skeleton_.ReloadSkeleton ();

		currentId = GestureId.NO_GESTURE;
		// Poll gesture controller to see if a gesture has been detected
		foreach(Gesture gesture in gestureList)
		{
		   if(gesture.trackGesture(skeleton_)) {
				currentId = gesture.GestureId_;
				Debug.Log ("Gesture: " + currentId);
			}
		}
		//print(currentId);
		//print(gestureId[0]);
	}

	void AddGesture(Gesture gesture)
	{
		gestureList.Add(gesture);
	}

	private Skeleton skeleton_ = new Skeleton(0);

	private List<Gesture> gestureList = new List<Gesture>();

	private GestureId currentId = GestureId.NO_GESTURE;

	private static GestureRecognition instance;

}
