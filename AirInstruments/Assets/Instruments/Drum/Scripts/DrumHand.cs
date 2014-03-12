using UnityEngine;
using System.Collections;
using System;

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

	public void MettreAJour(Vector3 position, Vector3 rotation) {
		MettreAJourPosition (position);
		MettreAJourRotation (rotation);
	}

	private void MettreAJourRotation(Vector3 rotation) {
		return;
		/*
		Vector4 previousRotation = kalman.GetFilteredVector ();
		// x, y, z
		for (int i = 0; i < 3; ++i) {
			float diffRotationDepasseHaut = Mathf.Abs(rotation[i] + 360 - previousRotation[i]);
			float diffRotationDepasseBas = Mathf.Abs(rotation[i] - 360 - previousRotation[i]);
			float diffRotation = Math.Abs(rotation[i] - previousRotation[i]);

			if (diffRotationDepasseBas < diffRotation && diffRotationDepasseBas < diffRotationDepasseHaut) {
				rotation[i] -= 360;
			} else if (diffRotationDepasseHaut < diffRotation && diffRotationDepasseHaut < diffRotationDepasseBas) {
				rotation[i] += 360;
			}
		}

		//if (rotation.z < 10 || rotation.z > 70) {
		//	rotation = anglesCorrects;
		//}

		Vector4 smoothedRotation = kalman.Update (new Vector4(rotation.x, rotation.y, rotation.z, 0));
		for (int i = 0; i < 3; ++i) {
			while (smoothedRotation[i] > 360) {
				smoothedRotation[i] -= 360;
			}
			while (smoothedRotation[i] < 0) {
				smoothedRotation[i] += 360;
			}


			float max = anglesCorrects[i] + kAnglesCorrectsTolerance[i];
			float min = anglesCorrects[i] - kAnglesCorrectsTolerance[i];
			if (smoothedRotation[i] > max || smoothedRotation[i] <  min) {
				float diffMax = DifferenceAngle(smoothedRotation[i], max);
				float diffMin = DifferenceAngle(smoothedRotation[i], min);
				if (diffMax < diffMin) {
					smoothedRotation[i] = max;
				}
				else {
					smoothedRotation[i] = min;
				}
			}

		}

		Quaternion smoothedRotationQuaternion = Quaternion.Euler (new Vector3 (smoothedRotation.x,
		                                                                       smoothedRotation.y,
		                                                                       smoothedRotation.z));
		//transform.localRotation = Quaternion.identity;
		//smoothedRotationQuaternion;
		kalman.SetInitialObservation (smoothedRotation);
		
		*/
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
