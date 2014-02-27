using UnityEngine;
using System;

public class HandJointSphere : MonoBehaviour, HandJointSphereI
{
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

	private Kalman kalman = new Kalman();

	// Gerer les donnees invalides.
	private bool valid = false;
	private int compteurInvalide = 0;
	private const int compteurInvalideMax = 10;
}
