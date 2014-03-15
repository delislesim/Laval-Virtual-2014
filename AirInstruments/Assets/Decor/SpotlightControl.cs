using UnityEngine;
using System.Collections;

public class SpotlightControl : MonoBehaviour {

	public void SetTargetIntensity(float intensity, float speed) {
		targetIntensity = intensity;
		speedIntensity = speed;
	}

	public void SetLookAt(Vector3 lookAtPoint, float speed) {
		Vector3 newDirection = lookAtPoint - transform.position;
		targetRotation.SetFromToRotation (Vector3.forward, newDirection);
		speedRotation = speed;
	}

	public void SetAngle(float angle, float speed) {
		targetAngle = angle;
		speedAngle = speed;
	}

	public void Reinitialiser() {
		targetIntensity = initialIntensity;
		targetRotation = initialRotation;
		targetAngle = initialAngle;
	}

	void Start () {
		// Parametres initiaux.
		initialIntensity = light.intensity;
		initialRotation = transform.rotation;
		initialAngle = light.spotAngle;

		Reinitialiser ();
	}

	// Update is called once per frame
	void Update () {
		// Animer l'intensite.
		float intensity = light.intensity;
		AnimateToTarget (targetIntensity, speedIntensity, intensity, out intensity);
		light.intensity = intensity;

		// Animer la rotation.
		transform.rotation = Quaternion.RotateTowards (transform.rotation,
		                                               targetRotation,
		                                               speedRotation * Time.deltaTime);

		// Animer l'angle.
		float angle = light.spotAngle;
		AnimateToTarget (targetAngle, speedAngle, angle, out angle);
		light.spotAngle = angle;
	}

	private void AnimateToTarget(float target, float speed, float initial, out float current) {
		current = initial;

		if (current < target) {
			current += speed * Time.deltaTime;
			if (current > target) {
				current = target;
			}
		} else if (current > target) {
			current -= speed * Time.deltaTime;
			if (current < target) {
				current = target;
			}
		}
	}

	// Intensité cible.
	private float targetIntensity;

	// Vitesse de changement de l'intensité.
	private float speedIntensity = 0;

	// Rotation cible.
	private Quaternion targetRotation;

	// Vitesse de rotation.
	private float speedRotation = 0;

	// Angle cible.
	private float targetAngle = 0;

	// Vitesse de changement de l'angle.
	private float speedAngle = 0;

	// Intensité initiale.
	private float initialIntensity;

	// Rotation initiale.
	private Quaternion initialRotation;

	// Angle initial.
	private float initialAngle;
}
