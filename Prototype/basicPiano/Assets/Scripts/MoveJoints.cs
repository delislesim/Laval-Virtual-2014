using UnityEngine;
using System.Collections;

public class MoveJoints : MonoBehaviour {

	//Public
	public GameObject Hip_Center;
	public GameObject Spine;
	public GameObject Shoulder_Center;
	public GameObject Head;
	public GameObject Shoulder_Left;
	public GameObject Elbow_Left;
	public GameObject Wrist_Left;
	public GameObject Hand_Left;
	public GameObject Shoulder_Right;
	public GameObject Elbow_Right;
	public GameObject Wrist_Right;
	public GameObject Hand_Right;
	public GameObject Hip_Left;
	public GameObject Knee_Left;
	public GameObject Ankle_Left;
	public GameObject Foot_Left;
	public GameObject Hip_Right;
	public GameObject Knee_Right;
	public GameObject Ankle_Right;
	public GameObject Foot_Right;

	//Public
	private GameObject[] joints; 
	private Vector3 posInitialOffset = Vector3.zero;
	private bool initialPosInitialized = false;
	private float[] jointPositions;

	// Use this for initialization
	void Start () {
		joints = new GameObject[(int)KinectPowerInterop.NuiSkeletonPositionIndex.Count] {
			Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right
		};

		jointPositions = new float[(int)KinectPowerInterop.NuiSkeletonPositionIndex.Count * 3] ;
	}
	
	// Update is called once per frame
	void Update () {

		//Get joints positions
		bool bonesTracked = KinectPowerInterop.GetJointsPosition(0, jointPositions);
		if(bonesTracked)
			Debug.Log("skelette détecté");
		else
			Debug.Log("En attente d'un skelette");

		//Set offset as soon as we see hip_center
		if(initialPosInitialized == false && bonesTracked )
		{
			Vector3 hipPos = new Vector3(jointPositions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HipCenter],
			                             jointPositions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HipCenter+1],
			                             jointPositions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HipCenter+2]);

			if(hipPos == Vector3.zero)
			{
				posInitialOffset = Vector3.zero - hipPos;
				initialPosInitialized = true;
			}
		}

		// update the local positions of the bones
		int jointsCount = (int)KinectPowerInterop.NuiSkeletonPositionIndex.Count;

		for(int i = 0; i < jointsCount; i++) 
		{
			if(joints[i] != null)
			{
				//int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i): i;
				Vector3 posJoint = new Vector3(jointPositions[i],
				                               jointPositions[i+1],
				                               jointPositions[i+2]);
				//posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;
				//Quaternion rotJoint = KinectManager.Instance.GetJointOrientation(playerID, joint, !MirroredMovement);
				
				//posJoint -= posPointMan;
				//posJoint.z = -posJoint.z;
				
				//if(MirroredMovement)
				//{
				//	posJoint.x = -posJoint.x;
				//}
				
				joints[i].transform.position = posJoint + posInitialOffset;
				//joints[i].transform.localRotation = rotJoint;
			}
		}	
/*	
		Vector3 mdPos = new Vector3(positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight],
		                                      positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight+1],
		                                      positions[(int)KinectPowerInterop.NuiSkeletonPositionIndex.HandRight+2]);
*/
		//Hand_Right.transform.position = mdPos*100;
	}
}
