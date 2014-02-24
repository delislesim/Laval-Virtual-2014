using UnityEngine;
using System;

public class HandJointSphere : MonoBehaviour, HandJointSphereI
{
	public void SetTargetPosition(Vector3 targetPosition) {
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
		transform.position = new Vector3 (kalmanPosition.x,
		                                  kalmanPosition.y,
		                                  kalmanPosition.z);
	}
	
	
	private bool initialized = false;
	
	private Kalman kalman = new Kalman();
}
