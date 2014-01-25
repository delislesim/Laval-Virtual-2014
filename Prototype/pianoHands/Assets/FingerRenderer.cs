using UnityEngine;
using System.Collections.Generic;

public class FingerRenderer : MonoBehaviour {

	// Basic cylinder for fingers
	public static Transform fingerPrefab;
	public static Transform jointPrefab;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Finger : MonoBehaviour {

	// 3D model
	public Transform model;
	// Joints
	public List<Transform> joints;

	public Finger(List<Vector3> jointsPosList)
	{
		// Instantiate each joint
		for(int i = 0; i < jointsPosList.Count; i++) {
			joints.Add((Transform) Instantiate(FingerRenderer.jointPrefab, jointsPosList[i], Quaternion.identity));
		}
		// Instantiate finger
		// todo : right position
		model = (Transform)Instantiate (FingerRenderer.fingerPrefab);
	}
}