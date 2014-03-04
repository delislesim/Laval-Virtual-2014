﻿using UnityEngine;
using System.Collections;

public class FingerSphere : MonoBehaviour, HandJointSphereI {

	// Use this for initialization
	void Start () {
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsValid ()) {
			return;
		}

		// Trouver des objets en collision avec cette boule.
		Collider[] hitColliders = Physics.OverlapSphere (transform.position, rayon);
		for (int i = 0; i < hitColliders.Length; ++i) {
			Collider collider = hitColliders[i];
			PianoNote note = collider.GetComponent<PianoNote>();
			if (note != null) {
				note.ToucherAvecSphere(this);
			}
		}
	}

	public void Reset() {
		initialized = false;
	}

	public void SetTargetPosition(Vector3 targetPosition, bool valid) {
		// Gerer les etats invalides.
		this.valid = valid;
		if (!valid) {
			++compteurInvalide;
			if (compteurInvalide > compteurInvalideMax) {
				renderer.enabled = false;
			}
			return;
		}
		compteurInvalide = 0;
		renderer.enabled = true;
		
		// Mettre a jour la position.
		if (!initialized) {
			kalman.SetInitialObservation(new Vector4(targetPosition.x,
			                                         targetPosition.y,
			                                         targetPosition.z));
			initialized = true;
		} else {
			kalman.Update(new Vector4(targetPosition.x,
			                          targetPosition.y,
			                          targetPosition.z));
		}

		// Bouger selon le filtre de Kalman.
		Vector4 kalmanPosition = kalman.GetFilteredVector ();
		transform.localPosition = new Vector3 (kalmanPosition.x,
		                       		           kalmanPosition.y,
		                            	       kalmanPosition.z);
	}

	public bool IsValid() {
		return valid && compteurInvalide <= compteurInvalideMax;
	}
	
	private bool initialized = false;
	
	private Kalman kalman = new Kalman(1.0f);

	// Rayon de la sphere en coordonnes du monde.
	private float rayon;

	// Gerer les donnees invalides.
	private bool valid = false;
	private int compteurInvalide = 0;
	private const int compteurInvalideMax = 10;
}
