using UnityEngine;
using System.Collections;

public class FeuGuitare : MonoBehaviour {

	public ParticleSystem murDeFeu;
	public GameObject lanceFlammes;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Solo = afficher le feu.
		if (AssistedModeControllerGuitar.EstSolo ()) {
			if (action != ActionCourante.MONTER && !lanceFlammes.activeSelf) {
				action = ActionCourante.MONTER;
				timer = 0;

				murDeFeu.gameObject.SetActive(true);
				lanceFlammes.SetActive(true);
			}
		} else {
			if (action != ActionCourante.DESCENDRE && lanceFlammes.activeSelf) {
				action = ActionCourante.DESCENDRE;
				timer = 0;
			}
		}

		// Afficher le feu.
		if (action == ActionCourante.MONTER) {
			timer += Time.deltaTime;
			float proportion = timer / kTempsAnimation;
			if (proportion > 1.0f)
				proportion = 1.0f;
			float y = easeOutQuad(kYCache, kYVisible, proportion);
			Vector3 position = transform.localPosition;
			position.y = y;

			if (proportion == 1.0f) {
				action = ActionCourante.RIEN;
				position.y = kYVisible;
			}

			transform.localPosition = position;

		}
		// Masquer le feu.
		else if (action == ActionCourante.DESCENDRE) {
			timer += Time.deltaTime;
			float proportion = timer / kTempsAnimation;
			if (proportion > 1.0f)
				proportion = 1.0f;
			float y = easeInQuad(kYVisible, kYCache, proportion);
			Vector3 position = transform.localPosition;
			position.y = y;

			if (proportion == 1.0f) {
				action = ActionCourante.RIEN;
				position.y = kYCache;
				murDeFeu.gameObject.SetActive(false);
				lanceFlammes.SetActive(false);
			}

			transform.localPosition = position;
		}

	}

	private float easeInQuad(float start, float end, float value){
		end -= start;
		return end * value * value + start;
	}
	
	private float easeOutQuad(float start, float end, float value){
		end -= start;
		return -end * value * (value - 2) + start;
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

	// Temps d'une animation.
	private const float kTempsAnimation = 2.0f;
}
