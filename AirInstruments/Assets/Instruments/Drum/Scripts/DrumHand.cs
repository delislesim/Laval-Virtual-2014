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

	private void MettreAJourPosition(Vector3 position) {
		/*
		Vector3 motion = position - transform.position;

		CharacterController characterController = GetComponent<CharacterController> ();
		characterController.Move (motion);
*/
		transform.position = position;
		/*
		// Si la main est trop loin de sa position actuelle selon Kinect, faire
		// une teleportation.
		if ((transform.position - position).magnitude > kDistanceTeleportation) {
			if (!Physics.CheckSphere(position, 0.8f)) {
				transform.position = position;
			}
		}
		*/
	}

	// Distance maximale pour une teleportation.
	private const float kDistanceTeleportation = 2.0f;

	// Position initiale.
	private Vector3 positionInitiale;
	
	// Rotation initiale.
	private Quaternion rotationInitiale;
}
