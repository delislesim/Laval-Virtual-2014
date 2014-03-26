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
	public static Vector3 HIDING_POS = new Vector3(100, 100, 100);
	private const float PLAYER_HIGHT = 5.0f;
	private const float DELTA_CHECK_TIME = 5.0f;
	private float accumulated_time;
	private const float DIST_MAX_KINECT = 10.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...

	// Script qui affiche les cylindres.
	public SkeletonDrawer drawer;

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

		// Se "connecter" au squelette 0.
		m_player_one = new Skeleton(0);
	}

	void OnEnable () {
		kalmanMainGauche = new Kalman (15.0f);
		kalmanMainDroite = new Kalman (15.0f);
	}
	
	// Update is called once per frame
	void Update () {
		m_player_one.ReloadSkeleton ();
		if (m_player_one.IsDifferent() && SkeletonIsTrackedAndValid(m_player_one)) {
			moveJoints (m_player_one);
			handFollower.SignalerNouvellePosition();
			drawer.PlacerCylindres();
		}

		// Mettre le bon layer a la ligne verte.
		if (GuitareController.JoueurEstVisible ()) {
			ligneVerte.SetActive (true);
		} else {
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
				Vector3 posJoint;
				Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);
				if(jointStatus != Skeleton.JointStatus.NotTracked && isReliable)
				{
					/*
					// Ajustement de la position des mains -> c'est seulement un prolongement des bras.
					if (i == (int)Skeleton.Joint.HandLeft) {
						Vector3 positionCoude;
						player.GetJointPosition(Skeleton.Joint.ElbowLeft, out positionCoude);
						Vector3 positionPoignet;
						player.GetJointPosition(Skeleton.Joint.WristLeft, out positionPoignet);
						posJoint = PositionMainAjustee(positionCoude, positionPoignet);
					} else if (i == (int)Skeleton.Joint.HandRight) {
						Vector3 positionCoude;
						player.GetJointPosition(Skeleton.Joint.ElbowRight, out positionCoude);
						Vector3 positionPoignet;
						player.GetJointPosition(Skeleton.Joint.WristRight, out positionPoignet);
						posJoint = PositionMainAjustee(positionCoude, positionPoignet);
					}
					*/

					// Appliquer un filtre de Kalman au mains.
					if (i == (int)Skeleton.Joint.HandLeft) {
						Vector3 positionPoignet;
						player.GetJointPosition(Skeleton.Joint.WristLeft, out positionPoignet);
						posJoint = AppliquerKalmanMain(kalmanMainGauche, positionPoignet, posJoint);
					} else if (i == (int)Skeleton.Joint.HandRight) {
						Vector3 positionPoignet;
						player.GetJointPosition(Skeleton.Joint.WristRight, out positionPoignet);
						posJoint = AppliquerKalmanMain(kalmanMainDroite, positionPoignet, posJoint);
					}

					//POSITIONS
					joints[i].transform.position = 
						new Vector3(posJoint.x*PLAYER_HIGHT, posJoint.y*PLAYER_HIGHT+2.5f, -posJoint.z*PLAYER_HIGHT)
							+ kDeplacementJoueur;

					// // ROTATIONS
					//Apply head rotation
					if(i == (int)Skeleton.Joint.Head)
						joints[i].transform.localRotation = player.GetFaceRotation();

					joints[i].renderer.enabled = true;

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

	// Retourne une position ajustee pour la main.
	Vector3 PositionMainAjustee(Vector3 positionCoude, Vector3 positionPoignet) {
		if (positionCoude == positionPoignet)
			return positionPoignet;

		Vector3 avantBras = positionPoignet - positionCoude;
		return positionPoignet + avantBras.normalized * kLongueurPoignetMain;
	}

	Vector3 AppliquerKalmanMain(Kalman kalman, Vector3 posPoignet, Vector3 posMain) {
		Vector3 posRelative = posMain - posPoignet;
		Vector4 newPosRelative = kalman.Update(new Vector4(posRelative.x, posRelative.y, posRelative.z, 0));
		return posPoignet + new Vector3(newPosRelative.x, newPosRelative.y, newPosRelative.z);
	}

	void manageMouvementsAndSounds(Vector3[] currentPos, Vector3[] pastPos)
	{
		Vector3 hipPos = current_positions[(int)Skeleton.Joint.HipCenter];
		Vector3 lHandPos = current_positions[(int)Skeleton.Joint.HandLeft];

		// Tricher en montant le hip pos un peu.
		hipPos += new Vector3 (0.0f, 0.02f, 0.0f);

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

	// Longueur entre le poignet et la main.
	private const float kLongueurPoignetMain = 0.1f;

	// Kalman pour la main gauche (position relative au poignet).
	private Kalman kalmanMainGauche;

	// Kalman pour la main droite (position relative au poignet).
	private Kalman kalmanMainDroite;

}
