using UnityEngine;
using System.Collections;

public class PianoExperimentationController : MonoBehaviour {

	public Transform joint0;
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
	public Transform joint18;
	public Transform joint19;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		num_known = 0;

		int[] positions = new int[30 * 3];
		byte[] known = new byte[30];
		KinectPowerInterop.GetPianoHands (positions, known);

		SetJointPosition (positions, known, 0, joint0);
		SetJointPosition (positions, known, 1, joint1);
		SetJointPosition (positions, known, 2, joint2);
		SetJointPosition (positions, known, 3, joint3);
		SetJointPosition (positions, known, 4, joint4);
		SetJointPosition (positions, known, 5, joint5);
		SetJointPosition (positions, known, 6, joint6);
		SetJointPosition (positions, known, 7, joint7);
		SetJointPosition (positions, known, 8, joint8);
		SetJointPosition (positions, known, 9, joint9);
		SetJointPosition (positions, known, 10, joint10);
		SetJointPosition (positions, known, 11, joint11);
		SetJointPosition (positions, known, 12, joint12);
		SetJointPosition (positions, known, 13, joint13);
		SetJointPosition (positions, known, 14, joint14);
		SetJointPosition (positions, known, 15, joint15);
		SetJointPosition (positions, known, 16, joint16);
		SetJointPosition (positions, known, 17, joint17);
		SetJointPosition (positions, known, 18, joint18);
		SetJointPosition (positions, known, 19, joint19);

		Debug.Log (num_known);
	}

	void SetJointPosition(int[] positions, byte[] known, int index, Transform joint) {
		if (known [index] == 1) {
			double x = positions[index * 3 + 0];
			double y = positions[index * 3 + 1];
			double z = positions[index * 3 + 2];
			x = (x - 320.0) * 15.0 / 320.0;
			y = -((y - 240.0) * 15.0 / 320.0);
			z = -(z - 750.0) / 6.0;

			joint.position = new Vector3 ((float)x, (float)y, (float)z);
			Debug.Log("joint " + index + ": " + joint.position);

			++num_known;
		} else {
			joint.position = new Vector3 (-100, -100, 0);
		}
	}

	private int num_known = 0;
}
