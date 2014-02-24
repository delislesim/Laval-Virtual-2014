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
	public HighHatComponent High_Hat;
	public TipCollider tip_left;
	public TipCollider tip_right;

	//Private
	private Skeleton m_player_one;
	private GameObject[] joints;
	private Vector3[] current_positions;
	private Vector3[] last_positions;
	private Quaternion[] current_rotations;
	private Quaternion[] last_rotations;
	private const float KICK_SPEED = 1.0f;
	private const float HH_SPEED = 1.0f;
	private Vector3 HIDING_POS = new Vector3(0,-10,0);
	private bool kick_ready;
	private bool hit_hat_ready;
	private const float PLAYER_HIGHT = 5.0f;
	private const float DELTA_CHECK_TIME = 5.0f;
	//private float accumulated_time;
	private const float DIST_MAX_KINECT = 12.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...

	//Hand freeze info
	private float right_hand_freez_posy;
	private float left_hand_freez_posy;
	private bool right_hand_freez_speedY_positive;
	private bool left_hand_freez_speedY_positive;
	private bool left_hand_is_frozen;
	private bool right_hand_is_frozen;


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

		last_rotations = new Quaternion[(int)Skeleton.Joint.Count];
		current_rotations = new Quaternion[(int)Skeleton.Joint.Count];
		kick_ready = true;

		right_hand_freez_posy = 0;
	    left_hand_freez_posy= 0;
		left_hand_is_frozen = false;
		right_hand_is_frozen = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Create valid skeleton with joints positions/rotations
		m_player_one = new Skeleton(0);
		//if (SkeletonIsTrackedAndValid(m_player_one))
			moveJoints (m_player_one);
	}

	bool SkeletonIsTrackedAndValid()
	{
		float distHipHead = Vector3.Distance(current_positions[(int)Skeleton.Joint.HipCenter], current_positions[(int)Skeleton.Joint.Head]);
		//Debug.Log ("Hip pos : z" + current_positions[(int)Skeleton.Joint.HipCenter].z);
		//Debug.Log ("Dist Hip-Head : " + distHipHead);

		//Check if hip joint is at reasonable distance from kinect (drum)
		//Check if dist hip to head is reasonable (no mini ghost) skeleton
			return current_positions[(int)Skeleton.Joint.HipCenter].z < -7
				&& current_positions[(int)Skeleton.Joint.HipCenter].z > -11
				&& distHipHead > 1.8
				&& distHipHead < 5;
			
	}

	void moveJoints(Skeleton player)
	{
		// update the local positions of the bones
		int jointsCount = (int)Skeleton.Joint.Count;

		for(int i = 0; i < jointsCount; i++) 
		{
			//Store last positions/rotations
			last_positions[i] = current_positions[i];
			last_rotations[i] = current_rotations[i];

			if(joints[i] != null)
			{
				Vector3 posJoint = Vector3.zero;
				Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);
				if(jointStatus != Skeleton.JointStatus.NotTracked)
				{
					// // POSITIONS
					//*****************************Freeze hands! *************************************************\\\
					if ( i == (int)Skeleton.Joint.HandRight && tip_right.IsCollided() )
					{
						if(!right_hand_is_frozen)
						{
							right_hand_is_frozen = true;
							right_hand_freez_posy = last_positions[i].y;
							joints[i].transform.position = last_positions[i];
						}//Frozen
						else
						{
							float posY = posJoint.y*PLAYER_HIGHT;
							if((posY > right_hand_freez_posy)){
								right_hand_is_frozen = false;
								joints[i].transform.position = new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT, -posJoint.z*PLAYER_HIGHT);
							}
						}//Not frozen
					}//if collided

					else if ( i == (int)Skeleton.Joint.HandLeft && tip_left.IsCollided() )
					{
						if(!left_hand_is_frozen)
						{
							left_hand_is_frozen = true;
							left_hand_freez_posy = last_positions[i].y;
							joints[i].transform.position = last_positions[i];
						}
						else
						{
							float posY = posJoint.y*PLAYER_HIGHT;
							if((posY > left_hand_freez_posy)){
								left_hand_is_frozen = false;
								joints[i].transform.position = new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT, -posJoint.z*PLAYER_HIGHT);
							}
						}
					}
					//*********************************************************************************\\\
				
					else {
						joints[i].transform.position = new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT, -posJoint.z*PLAYER_HIGHT);
					}

					// // ROTATIONS
					//Apply head rotation
					if(i == (int)Skeleton.Joint.Head)
						if(player.GetFaceTrackingStatus())
						{
							Debug.Log("Head rotation" + player.GetFaceRotation());
							joints[i].transform.localRotation = player.GetFaceRotation();
						}

					//Apply hand rotation if needed
					if(i == (int)Skeleton.Joint.HandRight)
					{
						if(tip_right.IsCollided())
							joints[i].transform.localRotation = last_rotations[i];
						else
							joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandRight);
					}
					if(i == (int)Skeleton.Joint.HandLeft)
					{
						if(tip_left.IsCollided())
							joints[i].transform.localRotation = last_rotations[i];
						else
							joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandLeft);
					}
				}
				//If not tracked, hide!
				else
					joints[i].transform.position = HIDING_POS;

				//Store new current position/rotation
				current_positions[i] = joints[i].transform.position;
				current_rotations[i] = joints[i].transform.localRotation;
			}
		}

		//Check if these positions are worth rendering -> Valid Skeleton
		//accumulated_time += Time.deltaTime;
		//if(accumulated_time > DELTA_CHECK_TIME)
		//{
		if(!SkeletonIsTrackedAndValid()){
			//Hide everything
			for(int i = 0; i < jointsCount; i++) 
			{
				joints[i].transform.position = HIDING_POS;
			}
			//TODO place camera so we don't see shit
		}
		//}
		//Predict sounds
		manageMouvementsAndSounds(current_positions, last_positions);
	}

	void manageMouvementsAndSounds(Vector3[] currentPos, Vector3[] pastPos)
	{
		//Play bass kick
		if(pastPos[(int)Skeleton.Joint.KneeRight] != HIDING_POS){
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

		//Manage High-Hat state
		if(pastPos[(int)Skeleton.Joint.KneeLeft] != HIDING_POS){
			if(pastPos[(int)Skeleton.Joint.KneeLeft].y - currentPos[(int)Skeleton.Joint.KneeLeft].y > (HH_SPEED * Time.deltaTime)
			   && hit_hat_ready == true){
				High_Hat.opened = false;
				hit_hat_ready = false;
			}
			
			if(pastPos[(int)Skeleton.Joint.KneeLeft].y - currentPos[(int)Skeleton.Joint.KneeLeft].y < (-HH_SPEED * Time.deltaTime)
			   && hit_hat_ready == false){
				High_Hat.opened = true;
				hit_hat_ready = true;
			}
		}
		           
	}

}
