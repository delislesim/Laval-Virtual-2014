using UnityEngine;
using System.Collections;

public class FeuDrum : MonoBehaviour {

	public ParticleSystem wall;
	public ParticleSystem smoke;
	public ParticleSystem small;
	public ParticleSystem sparks;

	// Envoyer une shot de feu.
	public void Burst() {
		SetEmettre (true);
		SetActive (true);
		compteur = 0;
	}

	// Use this for initialization
	void Start () {
		SetEmettre (false);
		SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		compteur += Time.deltaTime;
		if (compteur >= kTempsDesactiver) {
			SetActive(false);
			SetEmettre(false);
			compteur = 0;
		} else if (compteur >= kTempsArreterEmettre) {
			SetEmettre(false);
		}
	}

	void SetEmettre(bool emettre) {
		wall.enableEmission = emettre;
		smoke.enableEmission = emettre;
		small.enableEmission = emettre;
		sparks.enableEmission = emettre;
	}

	void SetActive(bool active) {
		wall.gameObject.SetActive (active);
		smoke.gameObject.SetActive (active);
		small.gameObject.SetActive (active);
		sparks.gameObject.SetActive (active);
	}

	// Temps pour arreter d'emettre.
	private const float kTempsArreterEmettre = 0.75f;

	// Temps pour desactiver le feu.
	private const float kTempsDesactiver = 7.0f;

	// Temps depuis le dernier burst de feu.
	private float compteur = 0;
}
