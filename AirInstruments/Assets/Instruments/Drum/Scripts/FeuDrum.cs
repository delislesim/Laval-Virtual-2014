using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FeuDrum : MonoBehaviour {

	public List<ParticleSystem> derriere;
	public List<ParticleSystem> crash;
	public List<ParticleSystem> tom1;
	public List<ParticleSystem> tom2;

	// Effets de lumiere et de fumee du drum.
	public SpotCouleurDrumMaster spotCouleurDrum;

	// Listes de particle systems.
	private List<List<ParticleSystem>> particle_list;

	public enum TypeEmission
	{
		DERRIERE,
		CRASH,
		TOM1,
		TOM2,
		COUNT
	}

	public static void BurstCrash() {
		burstCrash = true;
	}

	// Envoyer une shot de feu.
	public void Burst(TypeEmission typeEmission) {
		switch (typeEmission) {
		case TypeEmission.DERRIERE:
			SetEmettre(true, derriere);
			SetActive(true, derriere);
		break;
		case TypeEmission.CRASH:
			SetEmettre(true, crash);
			SetActive(true, crash);
		break;
		case TypeEmission.TOM1:
			SetEmettre(true, tom1);
			SetActive(true, tom1);
		break;
		case TypeEmission.TOM2:
			SetEmettre(true, tom2);
			SetActive(true, tom2);
		break;
		}
		compteurs [(int)typeEmission] = 0;
		fireOn [(int)typeEmission] = true;
	}

	// Use this for initialization
	void Start () {
		particle_list = new List<List<ParticleSystem>> ();
		particle_list.Add (derriere);
		particle_list.Add (crash);
		particle_list.Add (tom1);
		particle_list.Add (tom2);

		for (int i = 0; i < particle_list.Count; ++i) {
			SetEmettre(false, particle_list[i]);
			SetActive(false, particle_list[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (burstCrash) {
			Burst(TypeEmission.CRASH);
			burstCrash = false;
		}

		if (particle_list == null)
			return;

		for (int i = 0; i < (int)TypeEmission.COUNT; ++i) {
			compteurs[i] += Time.deltaTime;
			if (compteurs[i] >= kTempsDesactiver[i]) {
				SetEmettre(false, particle_list[i]);
				SetActive(false, particle_list[i]);
				compteurs[i] = 0;
				fireOn [i] = false;
			} else if (compteurs[i] >= kTempsArreterEmettre[i]) {
				SetEmettre(false, particle_list[i]);
			}
		}

		bool some_fire = false;
		for (int i = 0; i < (int)TypeEmission.COUNT; ++i) {
			if (fireOn[i])
				some_fire = true;
		}
		spotCouleurDrum.SetFermePourFeu (some_fire);
	}

	void SetEmettre(bool emettre, List<ParticleSystem> particle) {
		for (int i = 0; i < particle.Count; ++i) {
			particle[i].enableEmission = emettre;
		}
	}

	void SetActive(bool active, List<ParticleSystem> particle) {
		for (int i = 0; i < particle.Count; ++i) {
			particle[i].gameObject.SetActive(active);
		}
	}

	// Temps pour arreter d'emettre pour chaque compteur.
	private float[] kTempsArreterEmettre = {2.0f, 0.3f, 1.0f, 1.0f};

	// Temps pour desactiver le feu.
	private float[] kTempsDesactiver = {4.0f, 1.0f, 3.0f, 3.0f};

	// Compteurs pour fermer les bursts.
	private float[] compteurs = new float[(int)TypeEmission.COUNT];

	// Indiquer quels feux sont allumes.
	private bool[] fireOn = {false, false, false, false};

	// Mettre un burst sur le crash.
	private static bool burstCrash = false;
}
