using UnityEngine;
using System.Collections;
using System;

public class DrumHand : MonoBehaviour {

	// Bout de la baguette.
	public Collider tip;

	// Baguette.
	public Collider baguette;
	public Collider autreBaguette;
	public Collider autreMain;

	// Use this for initialization
	void Start () {
		positionInitiale = transform.position;
		rotationInitiale = transform.rotation;

		// Ignore les collisions entre les elements d'une meme main.
		Physics.IgnoreCollision (collider, baguette);
		Physics.IgnoreCollision (collider, autreBaguette);
		Physics.IgnoreCollision (collider, autreMain);

		// Initialiser Kalman.
		kalman.SetInitialObservation (Vector4.zero);

		if (name == "HandLeft")
			anglesCorrects = kAnglesCorrectsGauche;
		else
			anglesCorrects = kAnglesCorrectsDroite;
	}

	void OnDisable () {
		// Remettre les baguettes a leur position initiale pour que
		// le joint soit bien connecte au redemarrage.
		transform.position = positionInitiale;
		transform.rotation = rotationInitiale;
	}

	public void MettreAJour(Vector3 position) {
		MettreAJourPosition (position);
		//MettreAJourRotation (rotation);
	}

	float DifferenceAngle(float angle, float target) {
		float diffExact = Mathf.Abs(angle - target);
		float diffPlus = Mathf.Abs(angle + 360 - target);
		float diffMoins = Mathf.Abs(angle - 360 - target);
		return Math.Min(Math.Min(diffPlus, diffMoins), diffExact);
	}

	private void MettreAJourPosition(Vector3 position) {
		Vector3 motion = position - transform.position;

		CharacterController characterController = GetComponent<CharacterController> ();
		characterController.Move (motion);

		// Si la main est trop loin de sa position actuelle selon Kinect, faire
		// une teleportation.
		if ((transform.position - position).magnitude > kDistanceTeleportation) {
			if (!Physics.CheckSphere(position, 0.8f)) {
				transform.position = position;
			}
		}
	}

	// Filtre de Kalman pour smoother la rotation des mains.
	private Kalman kalman = new Kalman(30.0f);

	// Distance maximale pour une teleportation.
	private const float kDistanceTeleportation = 2.0f;

	private Vector3 anglesCorrects;
	private Vector3 kAnglesCorrectsGauche = new Vector3(300.0f, 205.0f, 40.0f);
	private Vector3 kAnglesCorrectsDroite = new Vector3(300.0f, 205.0f, 40.0f);
	private Vector3 kAnglesCorrectsTolerance = new Vector3(58.0f, 70.0f, 0); 

	// Position initiale.
	private Vector3 positionInitiale;
	
	// Rotation initiale.
	private Quaternion rotationInitiale;
}
