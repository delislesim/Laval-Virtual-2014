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
	private const float DIST_MAX_KINECT = 12.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...

	// Position par défaut de la tete, lorsqu'aucun squelette n'est visible.
	private Vector3 kPositionTeteDefaut = new Vector3(0, 1.88f, -10.88f);

	// Deplacement maximum de la tete par deltaTime.
	private float kDeplacementMaxTete = 10.0f;

	// Vitesse de rotation maximale de la caméra, en degres par deltaTime.
	private float kRotationMaxTete = 10.0f;

	// Camera principale du jeu.
	private Camera mainCamera;

	//Hand freeze info
	private bool right_hand_freez_speedY_positive;
	private bool left_hand_freez_speedY_positive;

	// Use this for initialization
	void Start () {
		targetRotationCamera.eulerAngles = new Vector3 (-10.0f, 0, 0);

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

		// Mettre la tete a la position par defaut, afin d'eviter les mouvements
		// brusques de camera quand on entre dans le mode drum.
		Head.transform.position = kPositionTeteDefaut;

		// Initialiser les filtres de Kalman.
		for (int i = 0; i < kalmanMains.Length; ++i) {
			kalmanMains[i] = new Kalman(20.0f);
			kalmanMains[i].SetInitialObservation(Vector4.zero);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Create valid skeleton with joints positions/rotations
		m_player_one = new Skeleton(0);
		moveJoints (m_player_one);
	}

	public void AssignerCamera(Camera mainCamera) {
		this.mainCamera = mainCamera;
	}

	bool SkeletonIsTrackedAndValid()
	{
		float distHipHead = Vector3.Distance(current_positions[(int)Skeleton.Joint.HipCenter], current_positions[(int)Skeleton.Joint.Head]);

		//Check if hip joint is at reasonable distance from kinect (drum)
		//Check if dist hip to head is reasonable (no mini ghost) skeleton
		return current_positions[(int)Skeleton.Joint.HipCenter].z < -7
			&& current_positions[(int)Skeleton.Joint.HipCenter].z > -11
			&& distHipHead > 1.8
			&& distHipHead < 5;
	}

	void moveJoints(Skeleton player)
	{
		const int jointsCount = (int)Skeleton.Joint.Count;

		// Obtenir toutes les positions courantes.
		for (int i = 0; i < jointsCount; ++i) {
			// Store last positions/rotations
			last_positions[i] = current_positions[i];
			last_rotations[i] = current_rotations[i];

			// Obtenir la nouvelle position de la Kinect.
			Vector3 posJoint;
			Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);

			if(jointStatus != Skeleton.JointStatus.NotTracked && player.Exists()) {
				current_positions[i] = WorldPositionFromKinectPosition(posJoint);
			} else {
				current_positions[i] = HIDING_POS;
			}
		}

		// Determiner si le squelette est valide.
		bool skeletonValid = SkeletonIsTrackedAndValid ();

		// Appliquer les positions aux articulations.
		for(int i = 0; i < jointsCount; i++) {

			if (joints[i] == null)  // TODO: Pourquoi ca arriverait?
				continue;

			if(i == (int)Skeleton.Joint.Head) {

				// Cas spécial de la tete.
				Vector3 targetPosition;
				if (skeletonValid && current_positions[i] != HIDING_POS) {
					targetPosition = current_positions[i];
				} else {
					targetPosition = kPositionTeteDefaut;
				}

				// Bouger la tete progressivement vers la position cible, afin d'eviter les
				// mouvements brusques de camera.
				joints[i].transform.position = Vector3.MoveTowards(joints[i].transform.position,
				                                                   targetPosition,
				                                                   kDeplacementMaxTete * Time.deltaTime);

				// Si on a une camera, faire la rotation.
				if (mainCamera != null) {
					Quaternion rotationCamera = mainCamera.transform.rotation;
					/*
					mainCamera.transform.rotation = Quaternion.RotateTowards(rotationCamera,
					                                                         targetRotationCamera,
					                                                         kRotationMaxTete * Time.deltaTime);*/
				}

				/* TODO: Face tracker.
				Quaternion faceRotation = player.GetFaceRotation();
				if(player.GetFaceTrackingStatus())
				{
					Debug.Log("Head rotation" + player.GetFaceRotation());
					joints[i].transform.localRotation = faceRotation;
				}
				*/
			} else {
				joints[i].transform.position = current_positions[i];

				if (i == (int)Skeleton.Joint.HandRight || i == (int)Skeleton.Joint.HandLeft) {
					moveHand(player, (Skeleton.Joint)i);
				}

				if (skeletonValid && current_positions[i] != HIDING_POS) {
					joints[i].renderer.enabled = true;
				} else {
					joints[i].renderer.enabled = false;
				}
			}

			//Store new current position/rotation
			current_positions[i] = joints[i].transform.position;
			current_rotations[i] = joints[i].transform.localRotation;
		}

		// Predict sounds
		manageMouvementsAndSounds(current_positions, last_positions);
	}

	Vector3 WorldPositionFromKinectPosition(Vector3 kinectPosition) {
		return new Vector3(kinectPosition.x*PLAYER_HIGHT,
		                   kinectPosition.y*PLAYER_HIGHT,
		                   -kinectPosition.z*PLAYER_HIGHT);
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

	private void moveHand(Skeleton player, Skeleton.Joint joint){
		// Gerer la rotation des mains.
		int index = joint == Skeleton.Joint.HandLeft ? 0 : 1;

		Vector3 rotation = player.GetBoneOrientation (joint).eulerAngles;
		Vector4 previousRotation = kalmanMains [index].GetFilteredVector ();
		for (int i = 0; i < 3; ++i) {
			if (rotation[i] > 300 && previousRotation[i] < 60) {
				rotation[i] -= 360;
			} else if (rotation[i] < 60 && previousRotation[i] > 300) {
				rotation[i] += 360;
			}
		}

		Vector4 smoothedRotation = kalmanMains [index].Update (new Vector4(rotation.x, rotation.y, rotation.z, 0));
		Quaternion smoothedRotationQuaternion = Quaternion.Euler (new Vector3 (smoothedRotation.x,
		                                                                       smoothedRotation.y,
		                                                                       smoothedRotation.z));
		joints [(int)joint].transform.localRotation = smoothedRotationQuaternion;

		for (int i = 0; i < 3; ++i) {
			while (smoothedRotation[i] > 360) {
				smoothedRotation[i] -= 360;
			}
			while (smoothedRotation[i] < 0) {
				smoothedRotation[i] += 360;
			}
		}
		kalmanMains [index].SetInitialObservation (smoothedRotation);
	}

	private Quaternion targetRotationCamera = Quaternion.identity;

	// Filtres de Kalman pour la rotation des mains.
	private Kalman[] kalmanMains = new Kalman[2];
}
