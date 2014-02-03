using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {

	public Transform joint1;
	public Transform joint2;
	public Transform joint3;
	public Transform joint4;
	public Transform joint5;
	public Transform joint6;
	public Transform joint7;
	public Transform joint8;
	public Transform joint9;
	public Transform joint10;
	public Transform joint11;
	public Transform joint12;
	public Transform joint13;
	public Transform joint14;
	public Transform joint15;
	public Transform joint16;
	public Transform joint17;

	// Use this for initialization
	void Start () {
		KinectPowerInterop.InitializeHandTracker ();
	}
	
	// Update is called once per frame
	void Update () {
		float[] positions = new float[17 * 2 * 3];
		float[] error = new float[17 * 2];
		KinectPowerInterop.GetHandsSkeletons (positions, error);

		SetJointPosition (positions, 0, joint1);
		SetJointPosition (positions, 1, joint2);
		SetJointPosition (positions, 2, joint3);
		SetJointPosition (positions, 3, joint4);
		SetJointPosition (positions, 4, joint5);
		SetJointPosition (positions, 5, joint6);
		SetJointPosition (positions, 6, joint7);
		SetJointPosition (positions, 7, joint8);
		SetJointPosition (positions, 8, joint9);
		SetJointPosition (positions, 9, joint10);
		SetJointPosition (positions, 10, joint11);
		SetJointPosition (positions, 11, joint12);
		SetJointPosition (positions, 12, joint13);
		SetJointPosition (positions, 13, joint14);
		SetJointPosition (positions, 14, joint15);
		SetJointPosition (positions, 15, joint16);
		SetJointPosition (positions, 16, joint17);
	}

	void SetJointPosition(float[] positions, int index, Transform joint) {
		joint.position = new Vector3 (-positions [index * 3 + 0] * 30,
		                              -positions [index * 3 + 1] * 30 + 3,
		                              -positions [index * 3 + 2] * 30 + 12);
	}
}
