using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FeuDrum : MonoBehaviour {

	public List<ParticleSystem> derriere;
	public List<ParticleSystem> crash;
	public List<ParticleSystem> tom1;
	public List<ParticleSystem> tom2;

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
		if (particle_list == null)
			return;

		for (int i = 0; i < (int)TypeEmission.COUNT; ++i) {
			compteurs[i] += Time.deltaTime;
			if (compteurs[i] >= kTempsDesactiver) {
				SetEmettre(false, particle_list[i]);
				SetActive(false, particle_list[i]);
				compteurs[i] = 0;
			} else if (compteurs[i] >= kTempsArreterEmettre) {
				SetEmettre(false, particle_list[i]);
			}
		}
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

	// Temps pour arreter d'emettre.
	private const float kTempsArreterEmettre = 0.75f;

	// Temps pour desactiver le feu.
	private const float kTempsDesactiver = 7.0f;

	// Compteurs pour fermer les bursts.
	private float[] compteurs = new float[(int)TypeEmission.COUNT];
}
