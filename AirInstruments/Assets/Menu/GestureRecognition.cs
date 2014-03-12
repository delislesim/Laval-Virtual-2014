using UnityEngine;
using System.Collections;
using KinectHelpers;
using System.Collections.Generic;

public enum GestureId
{
	NO_GESTURE = -1, 
	GESTURE_PIANO = 0,
	GESTURE_MENU = 1,
	GESTURE_DRUM = 2,
	GESTURE_GUITAR = 3
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
		AddGesture (new GestureDrum ());
		AddGesture (new GestureGuitar ());
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
		DetectGesture (currentId);
		//print(currentId);
		//print(gestureId[0]);
	}

	private void DetectGesture(GestureId gestureId)
	{
		switch(gestureId)
		{
		case GestureId.NO_GESTURE:
			break;
		case GestureId.GESTURE_DRUM:
			GameState.ObtenirInstance().AccederEtat(GameState.State.Drum);
			SwitchToInstrument();
			break;
		case GestureId.GESTURE_GUITAR:
			GameState.ObtenirInstance().AccederEtat(GameState.State.Guitar);
			SwitchToInstrument();
			break;
		case GestureId.GESTURE_PIANO:
			GameState.ObtenirInstance().AccederEtat(GameState.State.Piano);
			SwitchToInstrument();
			break;
		case GestureId.GESTURE_MENU:
			GameState.ObtenirInstance().AccederEtat(GameState.State.ChooseInstrument);
			break;
		}
	}

	private void SwitchToInstrument()
	{
		DeleteGesture (GestureId.GESTURE_DRUM);
		DeleteGesture (GestureId.GESTURE_PIANO);
		DeleteGesture (GestureId.GESTURE_GUITAR);
		AddGesture (new GestureMenu ());
	}

	private void AddGesture(Gesture gesture)
	{
		gestureList.Add(gesture);
	}

	private void DeleteGesture(GestureId gestureId)
	{
		gestureList.RemoveAll (x => x.GestureId_ == gestureId);
	}

	private Skeleton skeleton_ = new Skeleton(0);

	private List<Gesture> gestureList = new List<Gesture>();

	private GestureId currentId = GestureId.NO_GESTURE;

	private static GestureRecognition instance;

}
