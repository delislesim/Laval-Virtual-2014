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
	private Vector3[] current_positions;
	private Vector3[] last_positions;
	private bool high_hat_opened;
	private const float KICK_SPEED = 0.35f;
	private bool kick_ready;

	// Use this for initialization
	void Start () {
		joints = new GameObject[(int)Skeleton.Joint.Count] {
			Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right
		};

		last_positions = new Vector3[(int)Skeleton.Joint.Count];
		current_positions = new Vector3[(int)Skeleton.Joint.Count];
		high_hat_opened = false;
		kick_ready = true;
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
			last_positions[i] = current_positions[i];
		}

		for(int i = 0; i < jointsCount; i++) 
		{
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

					//Apply hand rotation
					if(i == (int)Skeleton.Joint.HandRight)
						joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandRight);
					if(i == (int)Skeleton.Joint.HandLeft)
						joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandLeft);
				}
				//If not tracked, hide!
				else
					joints[i].transform.position = new Vector3(0,-10,0);

				//Store new current position/rotation
				current_positions[i] = joints[i].transform.position;
			}
		}

		//Predict sounds
		manageMouvementsAndSounds(current_positions, last_positions);
	}

	void manageMouvementsAndSounds(Vector3[] currentPos, Vector3[] pastPos)
	{
		//Play bass kick
		if(pastPos[(int)Skeleton.Joint.KneeRight] != null){
			if(pastPos[(int)Skeleton.Joint.KneeRight].y - currentPos[(int)Skeleton.Joint.KneeRight].y > (KICK_SPEED * Time.deltaTime)
			   && kick_ready == true){
				Bass_Kick.PlaySound();
				kick_ready = false;
			}

			if(pastPos[(int)Skeleton.Joint.KneeRight].y - currentPos[(int)Skeleton.Joint.KneeRight].y < (-KICK_SPEED/2 * Time.deltaTime)
			   && kick_ready == false){
				kick_ready = true;
			}
		}
		           
	}

}
