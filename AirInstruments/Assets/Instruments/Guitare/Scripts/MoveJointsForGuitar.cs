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

	// Hand follower.
	public HandFollower handFollower;

	//public GuitarPlayer GuitPlayer;
	public Transform GuitarContainer;

	// Guitare.
	public GameObject guitare;

	// Ligne verte sur la guitare.
	public GameObject ligneVerte;

	//Private
	private Skeleton m_player_one;
	private GameObject[] joints;
	private Vector3[] current_positions;
	private Vector3[] last_positions;
	private Quaternion[] current_rotations;
	private Quaternion[] last_rotations;
	private const float STRUMMING_HAND_SPEED = 2.0f;
	private Vector3 HIDING_POS = new Vector3(100, 100, 100);
	private const float PLAYER_HIGHT = 5.0f;
	private const float DELTA_CHECK_TIME = 5.0f;
	private float accumulated_time;
	private const float DIST_MAX_KINECT = 10.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...

	// Layer d'affichage prioritaire.
	private int kLayerPrioritaire;

	// Layer d'affichage prioritaire extra.
	private int kLayerPrioritaireExtra;
	
	// Layer d'affichage par défaut.
	private int kLayerDefault;

	// Use this for initialization
	void Start () {
		kLayerPrioritaire = LayerMask.NameToLayer ("AffichagePrioritaireExtra");
		kLayerPrioritaireExtra = LayerMask.NameToLayer ("AffichagePrioritaireExtra");
		kLayerDefault = LayerMask.NameToLayer ("Default");

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

		// Se "connecter" au squelette 0.
		m_player_one = new Skeleton(0);
	}
	
	// Update is called once per frame
	void Update () {
		m_player_one.ReloadSkeleton ();
		if (m_player_one.IsDifferent() && SkeletonIsTrackedAndValid(m_player_one)) {
			moveJoints (m_player_one);
			handFollower.SignalerNouvellePosition();
		}

		// Mettre le bon layer a la guitare et a sa ligne verte.
		if (GuitareController.JoueurEstVisible ()) {
			guitare.layer = kLayerPrioritaire;
			ligneVerte.layer = kLayerPrioritaire;
		} else {
			guitare.layer = kLayerDefault;
			ligneVerte.SetActive (false);
		}
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

		bool isReliable = player.IsSkeletonReliable ();

		for(int i = 0; i < jointsCount; i++) 
		{
			//Store last positions/rotations
			last_positions[i] = current_positions[i];
			last_rotations[i] = current_rotations[i];

			if(joints[i] != null)
			{
				Vector3 posJoint = Vector3.zero;
				Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);
				if(jointStatus != Skeleton.JointStatus.NotTracked && isReliable)
				{
					//POSITIONS
					joints[i].transform.position = 
						new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT+2.5f, -posJoint.z*PLAYER_HIGHT)
							+ kDeplacementJoueur;

					// // ROTATIONS
					//Apply head rotation
					if(i == (int)Skeleton.Joint.Head)
						joints[i].transform.localRotation = player.GetFaceRotation();

					joints[i].renderer.enabled = true;

					// Mettre sur le bon layer.
					if (GuitareController.JoueurEstVisible() && EstIndexBras(i)) {
						joints[i].layer = kLayerPrioritaireExtra;
					} else {
						joints[i].layer = kLayerDefault;
					}

				}
				//If not tracked, hide!
				else {
					joints[i].transform.position = HIDING_POS;
					joints[i].renderer.enabled = false;
				}

				//Store new current position/rotation
				current_positions[i] = joints[i].transform.position;
				current_rotations[i] = joints[i].transform.localRotation;
			}
		}

		//Predict sounds?
		manageMouvementsAndSounds(current_positions, last_positions);
	}

	void manageMouvementsAndSounds(Vector3[] currentPos, Vector3[] pastPos)
	{
		Vector3 hipPos = current_positions[(int)Skeleton.Joint.HipCenter];
		Vector3 lHandPos = current_positions[(int)Skeleton.Joint.HandLeft];

		//Guit position
		GuitarContainer.position = hipPos;

		//Guit rotation
		Vector3 hipPosXZ = new Vector3(hipPos.x, 0, hipPos.z);
		Vector3 lHandPosXZ = new Vector3(lHandPos.x, 0, lHandPos.z);
		float AngleRotY = Vector3.Angle(new Vector3(0,0,1), lHandPosXZ-hipPosXZ);

		Vector3 hipPosXY = new Vector3(hipPos.x, hipPos.y, 0);
		Vector3 lHandPosXY = new Vector3(lHandPos.x, lHandPos.y, 0);
		float AngleRotZ = Vector3.Angle(new Vector3(0,1,0), hipPosXY-lHandPosXY);

		Quaternion GuitarRotation = Quaternion.Euler (0, -AngleRotY-90, AngleRotZ);
		GuitarContainer.rotation = GuitarRotation;
	}

	private bool EstIndexBras(int i) {
		return i == (int)Skeleton.Joint.ElbowLeft ||
				i == (int)Skeleton.Joint.ElbowRight ||
				i == (int)Skeleton.Joint.ShoulderLeft ||
				i == (int)Skeleton.Joint.ShoulderRight ||
				i == (int)Skeleton.Joint.ShoulderCenter ||
				i == (int)Skeleton.Joint.WristLeft ||
				i == (int)Skeleton.Joint.WristRight ||
				i == (int)Skeleton.Joint.HandLeft ||
				i == (int)Skeleton.Joint.HandRight;
	}

	// --- Constantes ---

	// Deplacement du joueur par rapport au centre de la scene.
	// Sert a ne pas etre au meme endroit que le drummer.
	private Vector3 kDeplacementJoueur = new Vector3(13.25f, 0, 0);


}
