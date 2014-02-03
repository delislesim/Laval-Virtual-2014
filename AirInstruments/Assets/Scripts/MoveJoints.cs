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

	public DrumComponent Bass_Kick; 

	//Public
	private GameObject[] joints;
	private Transform[] current_transforms;
	private Transform[] last_transforms;
	private bool high_hat_opened;

	// Use this for initialization
	void Start () {
		joints = new GameObject[(int)Skeleton.Joint.Count] {
			Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right
		};

		last_transforms = new Transform[(int)Skeleton.Joint.Count];
		current_transforms = new Transform[(int)Skeleton.Joint.Count];
		high_hat_opened = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Create valid skeleton with joints positions/rotations
		Skeleton playerOne = new Skeleton(0);
		moveJoints (playerOne);

	}

	void moveJoints(Skeleton player)
	{
		// update the local positions of the bones
		int jointsCount = (int)Skeleton.Joint.Count;

		for(int i = 0; i < jointsCount; i++) 
		{
			//Store last positions/rotations
			last_transforms[i] = current_transforms[i];

			if(joints[i] != null)
			{
				Vector3 posJoint = Vector3.zero;
				Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);
				if(jointStatus != Skeleton.JointStatus.NotTracked)
				{
					joints[i].transform.position = new Vector3(posJoint.x*5, posJoint.y*5, -5*posJoint.z);
					//Apply head rotation
					if(i == (int)Skeleton.Joint.Head)
						joints[i].transform.localRotation = player.GetNeckOrientation();
				}
				//If not tracked, hide!
				else
					joints[i].transform.position = new Vector3(0,-10,0);

				//Store new current position/rotation
				current_transforms[i] = joints[i].transform;
			}
		}

		//Predict sounds
		manageMouvementsAndSounds(current_transforms, last_transforms);
	}

	void manageMouvementsAndSounds(Transform[] currentPos, Transform[] pastPos)
	{
		//Play bass kick
		//if(pastPos[(int)Skeleton.Joint.KneeRight] != null){
		//	if(Mathf.Abs(pastPos[(int)Skeleton.Joint.KneeRight].position.y - currentPos[(int)Skeleton.Joint.KneeRight].position.y) > (0.002 * Time.deltaTime))
		//	{
		//		Bass_Kick.PlaySound();
				//Debug.Log ("Delta Y : " + Mathf.Abs(pastPos[(int)Skeleton.Joint.KneeRight].position.y - currentPos[(int)Skeleton.Joint.KneeRight].position.y)* Time.deltaTime);
		//	}

		//


	}

}
