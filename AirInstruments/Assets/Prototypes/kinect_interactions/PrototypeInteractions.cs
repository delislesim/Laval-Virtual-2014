﻿using UnityEngine;
using System.Collections;

public class PrototypeInteractions : MonoBehaviour {

	public Texture handTexture;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		KinectPowerInterop.NuiHandPointerInfo[] hands = new KinectPowerInterop.NuiHandPointerInfo[2];
		if (KinectPowerInterop.GetHandsInteraction (0, hands)) {
			if ((hands[0].State & KinectPowerInterop.NuiHandpointerStateNotTracked) == 0) {
				left_hand_position.x = hands[0].X;
				left_hand_position.y = hands[0].Y;
				Debug.Log(left_hand_position);
				left_hand_visible = true;
			} else {
				left_hand_visible = false;
			}
			if ((hands[1].State & KinectPowerInterop.NuiHandpointerStateNotTracked) == 0) {
				right_hand_position.x = hands[1].X;
				right_hand_position.y = hands[1].Y;
				//right_hand_visible = true;
			} else {
				//right_hand_visible = false;
			}
		} else {
			left_hand_visible = false;
			//right_hand_visible = false;
		}
	}

	void OnGUI () {
		if (left_hand_visible)
			GUI.DrawTexture (new Rect (left_hand_position.x * 200 + 300, left_hand_position.y * 200 + 300, 50, 50), handTexture);
	}

	bool left_hand_visible = false;
	//bool right_hand_visible = false;

	Vector2 left_hand_position = new Vector2 ();
	Vector2 right_hand_position = new Vector2 ();
}
