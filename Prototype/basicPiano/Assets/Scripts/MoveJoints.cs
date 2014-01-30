using UnityEngine;
using System.Collections;
using KinectHelpers;

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
	private Vector3 pos_initial_offset = Vector3.zero;
	private bool initial_position_initialized = false;

	// Use this for initialization
	void Start () {
		joints = new GameObject[(int)Skeleton.Joint.Count] {
			Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right
		};
	}
	
	// Update is called once per frame
	void Update () {

		//Get joints positions
		Skeleton skeleton = new Skeleton(0);
		//Set offset as soon as we see hip_center
		if(initial_position_initialized == false && skeleton.Exists() )
		{
			Vector3 hipPos = Vector3.zero;
			Skeleton.JointStatus status = skeleton.GetJointPosition(Skeleton.Joint.HipCenter, out hipPos);

			if(status != Skeleton.JointStatus.NotTracked)
			{
				pos_initial_offset = Vector3.zero - hipPos;
				//Debug.Log ("Offset : " + posInitialOffset);
				initial_position_initialized = true;
			}
		}

		// update the local positions of the bones
		int jointsCount = (int)Skeleton.Joint.Count;

		for(int i = 0; i < jointsCount; i++) 
		{
			if(joints[i] != null)
			{
				//int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i): i;
				Vector3 posJoint = Vector3.zero;
				//TODO get joint rotations
				Skeleton.JointStatus jointStatus = skeleton.GetJointPosition((Skeleton.Joint)i, out posJoint);
				
				joints[i].transform.position = new Vector3(posJoint.x*5, posJoint.y*5, -5*posJoint.z) + pos_initial_offset;

				if(i == (int)Skeleton.Joint.Head)
					joints[i].transform.localRotation = skeleton.GetNeckOrientation();
			}
		}	
	}
}
