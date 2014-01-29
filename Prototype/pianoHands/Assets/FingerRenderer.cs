using UnityEngine;
using System.Collections.Generic;

public class FingerRenderer : MonoBehaviour {

	// Basic cylinder for fingers
	public static Transform fingerPrefab;
	public static Transform jointPrefab;

	// 3D models for linking joints
	public List<Transform> models;
	// Joints
	public List<Transform> joints;


	// Use this for initialization
	void Start () {
	}

	void instantiateHand(List<Vector3> jointsPosList){
		// Instantiate each joint
		for(int i = 0; i < jointsPosList.Count; i++) {
			joints.Add((Transform) Instantiate(FingerRenderer.jointPrefab, jointsPosList[i], Quaternion.identity));
		}
		
		for (int i = 1; i < jointsPosList.Count-1; i++) {
			models.Add((Transform) Instantiate(FingerRenderer.fingerPrefab, (jointsPosList[i] - jointsPosList[i-1])/2, Quaternion.LookRotation(jointsPosList[i] - jointsPosList[i-1])));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}