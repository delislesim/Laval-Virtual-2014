using UnityEngine;
using System.Collections;

public class SpotlightControl : MonoBehaviour {

	public void SetTargetIntensity(float intensity, float speed) {
		targetIntensity = intensity;
		speedIntensity = speed;
	}

	void Start () {
		targetIntensity = light.intensity;
	}

	// Update is called once per frame
	void Update () {
		// Animer l'intensite.
		float intensity = light.intensity;
		AnimateToTarget (targetIntensity, speedIntensity, intensity, out intensity);
		light.intensity = intensity;
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
}
