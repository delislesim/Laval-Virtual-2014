using UnityEngine;
using System.Collections;

public class DrumDecoration : MonoBehaviour {

	public enum DirectionSource {
		HAUT,
		BAS,
		GAUCHE,
		DROITE
	}

	// Direction de laquelle le drum doit provenir lors des animations.
	public DirectionSource directionSource;

	void Start () {
		// Enregistrer la position initiale;
		positionInitiale = transform.position;

		// Calculer la position pour se cacher.
		positionCachee = positionInitiale;

		switch (directionSource) {
		case DirectionSource.HAUT:
			positionCachee.y = positionInitiale.y + kDistanceCacher;
			break;
		case DirectionSource.BAS:
			positionCachee.y = positionInitiale.y - kDistanceCacher;
			break;
		case DirectionSource.GAUCHE:
			positionCachee.x = positionInitiale.x - kDistanceCacher;
			break;
		case DirectionSource.DROITE:
			positionCachee.x = positionInitiale.x + kDistanceCacher;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Quand le tutorial n'est pas actif, tous les composants doivent etre visibles.
		if (!DrumController.TutorialActif ()) {
			Afficher();
		}

		// Augmenter le compteur.
		compteur += Time.deltaTime;
		float proportion = compteur / kTempsAnimation;
		if (proportion > 1.0f)
			proportion = 1.0f;

		// Faire l'animation.
		if (actionCourante == ActionCourante.AFFICHER) {
			Debug.Log(proportion);
			transform.position = new Vector3(spring(positionCachee.x, positionInitiale.x, proportion),
			                                 spring(positionCachee.y, positionInitiale.y, proportion),
			                                 spring(positionCachee.z, positionInitiale.z, proportion));
		} else if (actionCourante == ActionCourante.CACHER) {
			transform.position = new Vector3(spring(positionInitiale.x, positionCachee.x, proportion),
			                                 spring(positionInitiale.y, positionCachee.y, proportion),
			                                 spring(positionInitiale.z, positionCachee.z, proportion));
		}

		// Quand l'animation est terminee...
		if (proportion == 1.0f)
			actionCourante = ActionCourante.RIEN;
	}

	public void Afficher() {
		if (actionCourante == ActionCourante.AFFICHER)
			return;
		if (transform.position == positionInitiale) {
			actionCourante = ActionCourante.RIEN;
			return;
		}

		compteur = 0;
		actionCourante = ActionCourante.AFFICHER;
	}

	public void Cacher() {
		if (actionCourante == ActionCourante.CACHER || transform.position == positionCachee)
			return;
		if (transform.position == positionCachee) {
			actionCourante = ActionCourante.RIEN;
			return;
		}

		compteur = 0;
		actionCourante = ActionCourante.CACHER;
	}

	/* GFX47 MOD START */
	private float easeInBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d-value) + start;
	}
	/* GFX47 MOD END */
	
	/* GFX47 MOD START */
	//private float bounce(float start, float end, float value){
	private float easeOutBounce(float start, float end, float value){
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}
	/* GFX47 MOD END */

	private float easeInOutBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		if (value < d/2) return easeInBounce(0, end, value*2) * 0.5f + start;
		else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}

	private float spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	// Position initiale.
	private Vector3 positionInitiale;

	// Position cachée.
	private Vector3 positionCachee;

	// Distance pour se cacher.
	private const float kDistanceCacher = 12.0f;

	// Action courante.
	enum ActionCourante {
		AFFICHER,
		CACHER,
		RIEN
	}
	private ActionCourante actionCourante = ActionCourante.RIEN;

	// Timer pour l'animation en cours.
	private float compteur = 0;

	// Temps pour chaque animation.
	private const float kTempsAnimation = 3.0f;
}
