using UnityEngine;
using System.Collections;
using KinectHelpers;

public class MoveJointsForGuitar : MonoBehaviour {
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

	//public GuitarPlayer GuitPlayer;
	public LineRenderer GuitarLine;
	public Transform GuitarContainer;

	//Private
	private Skeleton m_player_one;
	private GameObject[] joints;
	private Vector3[] current_positions;
	private Vector3[] last_positions;
	private Quaternion[] current_rotations;
	private Quaternion[] last_rotations;
	private const float STRUMMING_HAND_SPEED = 2.0f;
	private Vector3 HIDING_POS = new Vector3(0,-10,0);
	private const float PLAYER_HIGHT = 5.0f;
	private const float DELTA_CHECK_TIME = 5.0f;
	private float accumulated_time;
	private const float DIST_MAX_KINECT = 10.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...


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
		accumulated_time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//Create valid skeleton with joints positions/rotations
		m_player_one = new Skeleton(0);
		if (SkeletonIsTrackedAndValid(m_player_one))
			moveJoints (m_player_one);
	}

	bool SkeletonIsTrackedAndValid(Skeleton player)
	{
		accumulated_time += Time.deltaTime;
		if(accumulated_time > DELTA_CHECK_TIME)
		{
			//Check if hip joint is at reasonable distance from kinect (drum)

			//Check if dist hip to head is reasonable (no mini ghost) skeleton
		}
		return true;
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
					//POSITIONS
					joints[i].transform.position = new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT, -posJoint.z*PLAYER_HIGHT);

					// // ROTATIONS
					//Apply head rotation
					if(i == (int)Skeleton.Joint.Head)
						joints[i].transform.localRotation = player.GetFaceRotation();

					//Apply hand rotation if needed
					/*
					if(i == (int)Skeleton.Joint.HandRight)
					{
						joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandRight);
					}
					if(i == (int)Skeleton.Joint.HandLeft)
					{
						joints[i].transform.localRotation = player.GetBoneOrientation(Skeleton.Joint.HandLeft);
					}
					*/
				}
				//If not tracked, hide!
				else
					joints[i].transform.position = HIDING_POS;

				//Store new current position/rotation
				current_positions[i] = joints[i].transform.position;
				current_rotations[i] = joints[i].transform.localRotation;
			}
		}

		//Update line render
		//GuitarLine.SetPosition(0, current_positions[(int)Skeleton.Joint.HipCenter]);
		//GuitarLine.SetPosition(1, current_positions[(int)Skeleton.Joint.HandLeft]);

		//Predict sounds
		manageMouvementsAndSounds(current_positions, last_positions);
	}

	void manageMouvementsAndSounds(Vector3[] currentPos, Vector3[] pastPos)
	{
		//Play next note   
		//Calculer la droite entre hip_center et main gauche
		//Regarder les intersection entre cette droite et la main droite (dans le plan du joueur?)
		//if(intersects)
		//GuitarPlayer.PlayNextRandomNote();

		//http://answers.unity3d.com/questions/336755/line-renderer-collision-detection.html
		/*Ray ray = new Ray(current_positions[(int)Skeleton.Joint.HandLeft], 
		                 current_positions[(int)Skeleton.Joint.HipCenter] - current_positions[(int)Skeleton.Joint.HandLeft]);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if(hit.transform.gameObject.tag == "PlayHand")
			{
				GuitPlayer.PlayNextRandomNote();
			}
		}
		*/
		GuitarContainer.position = current_positions[(int)Skeleton.Joint.HipCenter];

	}

}
