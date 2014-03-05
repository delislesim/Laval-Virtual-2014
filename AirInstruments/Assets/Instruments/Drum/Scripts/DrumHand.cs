﻿using UnityEngine;
using System.Collections;

public class DrumHand : MonoBehaviour {

	// Bout de la baguette.
	public Collider tip;

	// Baguette.
	public Collider baguette;

	// Use this for initialization
	void Start () {
		positionInitiale = transform.position;
		rotationInitiale = transform.rotation;

		// Ignore les collisions entre les elements d'une meme main.
		Physics.IgnoreCollision (collider, baguette);

		// Initialiser Kalman.
		kalman.SetInitialObservation (Vector4.zero);
	}

	void OnDisable () {
		// Remettre les baguettes a leur position initiale pour que
		// le joint soit bien connecte au redemarrage.
		transform.position = positionInitiale;
		transform.rotation = rotationInitiale;
	}

	public void MettreAJour(Vector3 position, Vector3 rotation) {
		MettreAJourPosition (position);
		MettreAJourRotation (rotation);
	}

	private void MettreAJourRotation(Vector3 rotation) {
		Vector4 previousRotation = kalman.GetFilteredVector ();
		for (int i = 0; i < 3; ++i) {
			if (rotation[i] > 300 && previousRotation[i] < 60) {
				rotation[i] -= 360;
			} else if (rotation[i] < 60 && previousRotation[i] > 300) {
				rotation[i] += 360;
			}
		}
		
		Vector4 smoothedRotation = kalman.Update (new Vector4(rotation.x, rotation.y, rotation.z, 0));
		Quaternion smoothedRotationQuaternion = Quaternion.Euler (new Vector3 (smoothedRotation.x,
		                                                                       smoothedRotation.y,
		                                                                       smoothedRotation.z));
		transform.localRotation = smoothedRotationQuaternion;
		
		for (int i = 0; i < 3; ++i) {
			while (smoothedRotation[i] > 360) {
				smoothedRotation[i] -= 360;
			}
			while (smoothedRotation[i] < 0) {
				smoothedRotation[i] += 360;
			}
		}
		kalman.SetInitialObservation (smoothedRotation);
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
	private Kalman kalman = new Kalman(20.0f);

	// Distance maximale pour une teleportation.
	private const float kDistanceTeleportation = 2.0f;

	// Position initiale.
	private Vector3 positionInitiale;
	
	// Rotation initiale.
	private Quaternion rotationInitiale;
}
