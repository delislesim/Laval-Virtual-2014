using UnityEngine;
using System.Collections;

public class MoveJoints : MonoBehaviour {

	public GameObject mainDroite;
	public GameObject mainGauche;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float[] positions = new float[(int)KinectPowerInterop.NuiSkeletonPositionIndex.Count * 3] ;
		KinectPowerInterop.GetJointsPosition(0, positions);
	
		Vector3 mdPos = new Vector3(positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight],
		                                      positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight+1],
		                                      positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight+2]);

		mainDroite.transform.position = mdPos*100;
	}
}
