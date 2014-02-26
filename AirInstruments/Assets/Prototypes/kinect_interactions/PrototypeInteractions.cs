using UnityEngine;
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
			if ((hands[0].State & KinectPowerInterop.NuiHandPointerStatePrimaryForUser) != 0) {
				active_hand_position.x = hands[0].X;
				active_hand_position.y = hands[0].Y;
				active_hand_visible = true;
			} else if ((hands[1].State & KinectPowerInterop.NuiHandPointerStatePrimaryForUser) != 0) {
				active_hand_position.x = hands[1].X;
				active_hand_position.y = hands[1].Y;
				active_hand_visible = true;
			} else {
				active_hand_visible = false;
				Debug.Log(hands[0].State);
			}
		} else {
			active_hand_visible = false;
		}
	}

	void OnGUI () {
		if (active_hand_visible)
			GUI.DrawTexture (new Rect (active_hand_position.x * 200 + 300, active_hand_position.y * 200 + 300, 50, 50), handTexture);
	}

	bool active_hand_visible = false;

	Vector2 active_hand_position = new Vector2 ();
}
