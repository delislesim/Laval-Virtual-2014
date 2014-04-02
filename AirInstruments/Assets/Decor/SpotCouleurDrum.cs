using UnityEngine;
using System.Collections;

public class SpotCouleurDrum : MonoBehaviour {

	public Light subLight;

	// Use this for initialization
	void Start () {
		positionInitiale = transform.position;
		rotationInitiale = transform.rotation;

		centreRotation = positionInitiale;
		centreRotation.y -= 4.59f;
	}
	
	// Update is called once per frame
	void Update () {
		renderer.enabled = true;
		subLight.gameObject.SetActive(true);

		Color currentColor = renderer.material.GetColor ("_TintColor");

		// Animation de la couleur seulement quand on est a l'opacite max.
		if (opacite == kOpaciteMax) {
			currentColor = Color.Lerp(currentColor, targetColor, 1.0f * Time.deltaTime);
		}

		// Animations d'opacite.
		if (allume && !fermePourLeFun && opacite != kOpaciteMax) {
			// Animation d'allumage.
			opacite = Lerp.LerpFloat(opacite, kOpaciteMax, kVitesseOpacite * Time.deltaTime);
		} else if (!allume || fermePourLeFun) {
			// Animation de fermeture.
			opacite = Lerp.LerpFloat(opacite, !allume ? 0 : 0.25f, kVitesseOpacite * Time.deltaTime);
		}

		// Animation de l'angle.
		compteurAngle += Time.deltaTime;
		float proportionAngle = compteurAngle / kTempsAnimationAngle;
		if (proportionAngle >= 1.0f) {
			compteurAngle = 0;
			anglePrecedent = angleCible;
			angleCible = Random.Range(-10.0f, 10.0f);
		} else {
			float angleCourant = easeInOutCubic (anglePrecedent, angleCible, proportionAngle);
			transform.position = positionInitiale;
			transform.rotation = rotationInitiale;
			transform.RotateAround (centreRotation, Vector3.forward, angleCourant);
		}


		// Decider de se fermer pour le fun?
		compteurFermePourLeFun += Time.deltaTime;
		if (compteurFermePourLeFun >= kTempsFermePourLeFun) {
			ChoisirFermePourLeFun();
			compteurFermePourLeFun = 0;
		}

		// Appliquer l'opacite et la couleur.
		renderer.material.SetColor("_TintColor", new Color(currentColor.r,
						                                   currentColor.g,
						                                   currentColor.b,
						                                   opacite));
		subLight.color = new Color (currentColor.r, currentColor.g, currentColor.b, 1.0f);
		subLight.intensity = opacite * 16.0f;


		// Se desactive quand on est eteint et que l'opacite atteint zero.
		if (!allume && opacite == 0) {
			gameObject.SetActive(false);
		}
	}

	private float easeInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value + 2) + start;
	}

	void ChoisirFermePourLeFun() {
		double rand = Random.Range (0, 1.0f);
		if (fermePourLeFun) {
			if (rand > 0.1f)
				fermePourLeFun = false;
		} else {
			if (rand > 0.8f)
				fermePourLeFun = true;
		}
	}

	public void Allumer() {
		if (!allume) {
			kTempsAnimationAngle = 2.0f * Random.Range(0.75f, 1.25f);

			gameObject.SetActive(true);
			renderer.enabled = false;
			subLight.gameObject.SetActive(false);

			allume = true;
			compteurFermePourLeFun = 0;
			compteurAngle = kTempsAnimationAngle;
			fermePourLeFun = false;
			opacite = 0;
			anglePrecedent = 0;
			ChoisirFermePourLeFun();
		}
	}

	public void Fermer() {
		if (allume) {
			allume = false;
		}
	}

	public void ChangerCouleur(Color nouvelleCouleur) {
		targetColor = nouvelleCouleur;
	}

	// Position initiale.
	private Vector3 positionInitiale;

	// Position de rotation.
	private Vector3 centreRotation;

	// Angle cible.
	private float angleCible = 0;

	// Angle precedent.
	private float anglePrecedent = 0;

	// Compteur pour animer l'angle.
	private float compteurAngle = 0;

	// Temps pour animer un angle.
	private float kTempsAnimationAngle = 2.0f;

	// Rotation initiale.
	private Quaternion rotationInitiale;

	// Couleur cible.
	private Color targetColor;

	// Indique si on est suppose etre allume.
	private bool allume = false;

	// Indique si on a decide de se fermer pour le fun.
	private bool fermePourLeFun = false;

	// Opacite quand on est allume.
	private const float kOpaciteMax = 0.5f;

	// Opacite courante.
	private float opacite = 0;

	// Vitesse de changement de l'opacite.
	private const float kVitesseOpacite = 0.5f;

	// Compteur pour decider de s'eteindre pour le fun.
	private float compteurFermePourLeFun = 0;

	// Temps pour decider de se fermer pour le fun.
	private float kTempsFermePourLeFun = 3.0f;

}
