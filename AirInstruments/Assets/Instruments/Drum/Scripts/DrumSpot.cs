using UnityEngine;
using System.Collections;

public class DrumSpot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!definiACetteFrame) {
			targetLuminosite = 0;
		}
		definiACetteFrame = false;

		if (light.intensity < targetLuminosite) {
			light.intensity += kVitesseAugmenter * Time.deltaTime;
			if (light.intensity > targetLuminosite) {
				light.intensity = targetLuminosite;
			}
		} else if (light.intensity > targetLuminosite) {
			light.intensity -= kVitesseDiminuer * Time.deltaTime;
			if (light.intensity < targetLuminosite) {
				light.intensity = targetLuminosite;
			}
		}

		// Animer la couleur.
		/*
		Color color = light.color;
		for (int i = 0; i < 3; ++i) {
			if (color[i] < 1.0f) {
				color[i] += kVitesseCouleur * Time.deltaTime;
				if (color[i] > 1.0f) {
					color[i] = 1.0f;
				}
			}
		}
		light.color = color;
		*/

		if (play) {
			light.color = Color.cyan;
			light.intensity = 4.0f;
		}
		play = false;
	}

	public void Play() {
		play = true;
	}

	public void DefinirLuminosite (float luminosite) {
		targetLuminosite = luminosite;
		definiACetteFrame = true;
	}

	private float targetLuminosite = 0f;

	private bool play = false;

	private bool definiACetteFrame = false;
	private const float kVitesseAugmenter = 5.0f;
	private const float kVitesseDiminuer = 4f;

	private const float kVitesseCouleur = 2.0f;
}
