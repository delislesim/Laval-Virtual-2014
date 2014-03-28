using UnityEngine;
using System.Collections;
using KinectHelpers;

public class MoveJoints : MonoBehaviour {
	public GameObject Spine_Base;
	public GameObject Spine_Mid;
	public GameObject Neck;
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
	public GameObject Spine_Shoulder;
	public GameObject Hand_Tip_Left;
	public GameObject Thumb_Left;
	public GameObject Hand_Tip_Right;
	public GameObject Thumb_Right;

	public DrumComponent Bass_Kick; 

	// Script qui affiche les cylindres.
	public SkeletonDrawer drawer;

	//Private
	private Skeleton m_player_one;
	private GameObject[] joints;
	private Vector3[] current_positions;
	private Vector3[] last_positions;
	private Quaternion[] current_rotations;
	private Quaternion[] last_rotations;
	private const float KICK_SPEED = 1.0f;
	private const float HH_SPEED = 1.0f;
	private Vector3 HIDING_POS = new Vector3(0,-150,-150);
	private bool kick_ready;
	private bool hit_hat_ready;
	private const float PLAYER_HIGHT = 5.0f;
	private const float DELTA_CHECK_TIME = 5.0f;
	private const float DIST_MAX_KINECT = 12.0f; //2m
	private const float DIST_MIN_KINECT = 2.0f; //dist min...

	// Position par défaut de la tete, lorsqu'aucun squelette n'est visible.
	public static Vector3 kPositionTeteDefaut = new Vector3(0.1f, 3.1f, -10.3f);

	// Deplacement maximum de la tete par deltaTime.
	private float kDeplacementMaxTete = 10.0f;

	//Hand freeze info
	private bool right_hand_freez_speedY_positive;
	private bool left_hand_freez_speedY_positive;

	public static MoveJoints ObtenirInstance() {
		return instance;
	}

	// Use this for initialization
	void Start () {
		instance = this;

		joints = new GameObject[] {
			Spine_Base, Spine_Mid, Neck, Head, Shoulder_Left, Elbow_Left,
			Wrist_Left, Hand_Left, Shoulder_Right, Elbow_Right, Wrist_Right,
			Hand_Right, Hip_Left, Knee_Left, Ankle_Left, Foot_Left, Hip_Right,
			Knee_Right, Ankle_Right, Foot_Right, Spine_Shoulder, Hand_Tip_Left,
			Thumb_Left, Hand_Tip_Right, Thumb_Right
		};

		last_positions = new Vector3[(int)Skeleton.Joint.Count];
		current_positions = new Vector3[(int)Skeleton.Joint.Count];

		last_rotations = new Quaternion[(int)Skeleton.Joint.Count];
		current_rotations = new Quaternion[(int)Skeleton.Joint.Count];
		kick_ready = true;

		// Mettre la tete a la position par defaut, afin d'eviter les mouvements
		// brusques de camera quand on entre dans le mode drum.
		Head.transform.position = kPositionTeteDefaut;

		// Layer des colliders de drum components.
		drumComponentLayer = 1 << LayerMask.NameToLayer ("DrumComponent");

		// Charger le squelette.
		m_player_one = new Skeleton(0);

		// Valeurs initiales pour les filtres de Kalman.
		kalman_hand_left.SetInitialObservation (Vector4.zero);
		kalman_hand_right.SetInitialObservation (Vector4.zero);
		kalman_thumb_left.SetInitialObservation (Vector4.zero);
		kalman_thumb_right.SetInitialObservation (Vector4.zero);
	}
	
	// Update is called once per frame
	void Update () {
		m_player_one.ReloadSkeleton ();

		if (m_player_one.IsDifferent()) {
			moveJoints (m_player_one);
			drawer.PlacerCylindres();
		}
	}

	void moveJoints(Skeleton player)
	{
		const int jointsCount = (int)Skeleton.Joint.Count;

		// Obtenir toutes les positions courantes.
		for (int i = 0; i < jointsCount; ++i) {
			// Store last positions/rotations
			last_positions[i] = current_positions[i];
			last_rotations[i] = current_rotations[i];

			// Get new position.
			Vector3 posJoint;
			Skeleton.JointStatus jointStatus = player.GetJointPosition((Skeleton.Joint)i, out posJoint);

			if(jointStatus != Skeleton.JointStatus.NotTracked && player.Exists()) {
				current_positions[i] = WorldPositionFromKinectPosition(posJoint);
			} else {
				current_positions[i] = HIDING_POS;
			}
		}

		// Fixer la position du squelette.
		FixerSquelette ();

		bool IsReliable = player.IsSkeletonReliable ();

		// Appliquer les positions aux articulations.
		for(int i = 0; i < jointsCount; i++) {

			if (joints[i] == null)  // TODO: Pourquoi ca arriverait? Aucune idée honnetement. Après réflexion, toujours aucune idée
				continue;

			if(i == (int)Skeleton.Joint.Head) {

				// Cas spécial de la tete.
				Vector3 targetPosition;
				if (current_positions[i] != HIDING_POS) {
					targetPosition = current_positions[i];
//					Debug.Log (current_positions[i]);
				} else {
					targetPosition = kPositionTeteDefaut;
				}

				// Bouger la tete progressivement vers la position cible, afin d'eviter les
				// mouvements brusques de camera.
				joints[i].transform.position = Vector3.MoveTowards(joints[i].transform.position,
				                                                   targetPosition,
				                                                   kDeplacementMaxTete * Time.deltaTime);
			} else {
				if (i == (int)Skeleton.Joint.HandTipLeft) {
					current_positions[i] = KalmanRelatif(current_positions[(int)Skeleton.Joint.HandTipLeft],
					                                     current_positions[(int)Skeleton.Joint.HandLeft],
					                                     kalman_hand_left);
				} else if (i == (int)Skeleton.Joint.HandTipRight) {
					current_positions[i] = KalmanRelatif(current_positions[(int)Skeleton.Joint.HandTipRight],
					                                     current_positions[(int)Skeleton.Joint.HandRight],
					                                     kalman_hand_right);
				} else if (i == (int)Skeleton.Joint.ThumbLeft) {
					current_positions[i] = KalmanRelatif(current_positions[(int)Skeleton.Joint.ThumbLeft],
					                                     current_positions[(int)Skeleton.Joint.WristLeft],
					                                     kalman_thumb_left);
				} else if (i == (int)Skeleton.Joint.ThumbRight) {
					current_positions[i] = KalmanRelatif(current_positions[(int)Skeleton.Joint.ThumbRight],
					                                     current_positions[(int)Skeleton.Joint.WristRight],
					                                     kalman_thumb_right);
				}

				if (i == (int)Skeleton.Joint.HandRight || i == (int)Skeleton.Joint.HandLeft) {
					if (current_positions[i] != HIDING_POS) {
						DrumHand drumHand = joints [i].GetComponent<DrumHand>();
						drumHand.MettreAJour(current_positions[i]);
					}
				} else {
					joints[i].transform.position = current_positions[i];
				}

				if (current_positions[i] == HIDING_POS || !IsReliable ||
				    i == (int)Skeleton.Joint.HandTipLeft ||
				    i == (int)Skeleton.Joint.HandTipRight ||
				    i == (int)Skeleton.Joint.ThumbLeft ||
				    i == (int)Skeleton.Joint.ThumbRight) {
					joints[i].renderer.enabled = false;
				} else {
					joints[i].renderer.enabled = true;
				}
			}

			//Store new current position/rotation
			current_positions[i] = joints[i].transform.position;
			current_rotations[i] = joints[i].transform.localRotation;
		}

		// Rotation des mains.
		joints[(int)Skeleton.Joint.HandLeft].transform.rotation =
			VectorConversions.RotationDroite(current_positions[(int)Skeleton.Joint.WristLeft],
		                                     current_positions[(int)Skeleton.Joint.HandTipLeft]);
		joints[(int)Skeleton.Joint.HandRight].transform.rotation =
			VectorConversions.RotationDroite(current_positions[(int)Skeleton.Joint.WristRight],
			                                 current_positions[(int)Skeleton.Joint.HandTipRight]);
	}

	Vector3 KalmanRelatif(Vector3 joint, Vector3 parent, Kalman kalman) {
		Vector3 difference = joint - parent;
		Vector4 smoothed_difference = kalman.Update (new Vector4 (difference.x, difference.y, difference.z, 0));
		return parent + new Vector3 (smoothed_difference.x,
		                             smoothed_difference.y,
		                             smoothed_difference.z);
	}

	void FixerSquelette() {
		/*
		// Mesurer les bras.
		Vector3 positionEpaule = current_positions [(int)Skeleton.Joint.ShoulderLeft];
		Vector3 positionCoude = current_positions [(int)Skeleton.Joint.ElbowLeft];
		Vector3 positionMain = current_positions [(int)Skeleton.Joint.HandLeft];
		float longueurBras = Vector3.Distance (positionEpaule, positionCoude) +
						Vector3.Distance (positionCoude, positionMain);
		*/

		// Le centre des épaules doit etre pret de la position fixee.
		Vector3 positionCentreEpaules = current_positions [(int)Skeleton.Joint.Neck];
		if (positionCentreEpaules == HIDING_POS)
			return;

		Vector3 positionCentreEpaulesMax = kCibleEpaules + kToleranceCibleEpaules;
		Vector3 positionCentreEpaulesMin = kCibleEpaules - kToleranceCibleEpaules;
		Vector3 ajustement = Vector3.zero;

		for (int i = 0; i < 3; ++i) {
			if (positionCentreEpaules[i] < positionCentreEpaulesMin[i]) {
				ajustement[i] = positionCentreEpaulesMin[i] - positionCentreEpaules[i];
			} else if (positionCentreEpaules[i] > positionCentreEpaulesMax[i]) {
				ajustement[i] = positionCentreEpaulesMax[i] - positionCentreEpaules[i];
			}
		}

		for (int i = 0; i < (int)Skeleton.Joint.Count; ++i) {
			if (current_positions[i] != HIDING_POS) {
				current_positions[i] += ajustement;
			}
		}
	}
	
	Vector3 WorldPositionFromKinectPosition(Vector3 kinectPosition) {
		return new Vector3(kinectPosition.x*PLAYER_HIGHT,
		                   kinectPosition.y*PLAYER_HIGHT,
		                   -kinectPosition.z*PLAYER_HIGHT);
	}
	
	// Unique instance de cette classe.
	private static MoveJoints instance;

	// Layer des colliders de drum components.
	private int drumComponentLayer;

	// Position cible des épaules.
	private Vector3 kCibleEpaules = new Vector3(0.2f, 2.0f, -10.4f);

	// Tolérance pour la position cible des épaules.
	private Vector3 kToleranceCibleEpaules = new Vector3 (0.2f, 0.001f, 0.1f);

	// Force des filtres de Kalman.
	private const float kKalmanForce = 5.0f;

	// Kalman pour le pouce gauche par rapport au poignet.
	private Kalman kalman_thumb_left = new Kalman(kKalmanForce);

	// Kalman pour le pouce droit par rapport au poignet.
	private Kalman kalman_thumb_right = new Kalman(kKalmanForce);

	// Kalman pour les doigts gauche par rapport a la main.
	private Kalman kalman_hand_left = new Kalman(kKalmanForce);

	// Kalman pour les doigts droits par rapport a la main.
	private Kalman kalman_hand_right = new Kalman(kKalmanForce);


}

