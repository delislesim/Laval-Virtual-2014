using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FeuGuitare : MonoBehaviour {

	public GameObject lanceFlammes;

	public List<ParticleSystem> thrower;
	public List<ParticleSystem> smoke;
	public List<ParticleSystem> thrower_child;
	public List<ParticleSystem> sparks;

	// Use this for initialization
	void Start () {
		SetEmit (false);
	}

	void SetEmit(bool emit) {
		for (int i = 0; i < thrower.Count; ++i) {
			thrower[i].enableEmission = emit;
		}
		for (int i = 0; i < smoke.Count; ++i) {
			smoke[i].enableEmission = emit;
		}
		for (int i = 0; i < thrower_child.Count; ++i) {
			thrower_child[i].enableEmission = emit;
		}
		for (int i = 0; i < sparks.Count; ++i) {
			sparks[i].enableEmission = emit;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Solo = afficher le feu.
		if (AssistedModeControllerGuitar.EstSolo ()) {
			if (action != ActionCourante.MONTER && !lanceFlammes.activeSelf) {
				action = ActionCourante.MONTER;
				timer = 0;

				lanceFlammes.SetActive(true);
				SetEmit(true);
			}
		} else {
			if (action != ActionCourante.DESCENDRE && lanceFlammes.activeSelf) {
				action = ActionCourante.DESCENDRE;
				timer = 0;

				SetEmit(false);
			}
		}

		// Afficher le feu.
		if (action == ActionCourante.MONTER) {
			timer += Time.deltaTime;
			if (timer >= kTempsAnimationEntree) {
				action = ActionCourante.RIEN;
				//SetEmit(false);
				//lanceFlammes.SetActive(false);
			}
		}
		// Masquer le feu.
		else if (action == ActionCourante.DESCENDRE) {
			timer += Time.deltaTime;
			if (timer >= kTempsAnimationSortie) {
				action = ActionCourante.RIEN;
				//SetEmit(false);
				lanceFlammes.SetActive(false);
			}
		}

	}

	enum ActionCourante {
		MONTER,
		DESCENDRE,
		RIEN
	}
	private static ActionCourante action = ActionCourante.RIEN;

	// Position en y visible.
	private const float kYVisible = 4.52f;

	// Position en x visible.
	private const float kYCache = -8.5f;

	// Timer pour l'animation courante.
	private float timer = 0;

	// Temps d'une animation d'entree.
	private const float kTempsAnimationEntree = 1.0f;

	// Temps d'une animation de sortie.
	private const float kTempsAnimationSortie = 6.0f;
}
