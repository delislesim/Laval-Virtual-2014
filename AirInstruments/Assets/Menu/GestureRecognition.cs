using UnityEngine;
using System.Collections;

public class GestureRecognition : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Poll gesture controller to see if a gesture has been detected
		int[] gestureId = {-1};
		KinectPowerInterop.GetGestureStatus(gestureId);
		print(gestureId[0]);
	}

	void OnGUI () {
		if (left_hand_visible)
			GUI.DrawTexture (new Rect (left_hand_position.x * 200 + 300, left_hand_position.y * 200 + 300, 50, 50), handTexture);
	}

	public Texture handTexture;

	bool left_hand_visible = false;
	//bool right_hand_visible = false;
	
	Vector2 left_hand_position = new Vector2 ();
	Vector2 right_hand_position = new Vector2 ();
}
