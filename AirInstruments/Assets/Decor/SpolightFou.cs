using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpolightFou : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Se rappeler de l'intensite initiale.
		kIntensity = light.intensity;

		// Couleurs possibles.
		kCouleursPossibles.Add (Color.blue);
		kCouleursPossibles.Add (Color.cyan);
		kCouleursPossibles.Add (Color.green);
		kCouleursPossibles.Add (Color.magenta);
		kCouleursPossibles.Add (Color.red);
		kCouleursPossibles.Add (Color.yellow);

		// Choisir une nouvelle couleur.
		ChoisirNouvelleCouleurCible ();

		// Angle cible initial.
		angleCible = Quaternion.Euler (kAngleBase);
	}
	
	// Update is called once per frame
	void Update () {
		// Animer vers l'angle cible.
		transform.rotation = Quaternion.RotateTowards (transform.rotation, angleCible, kVitesseRotation * Time.deltaTime);

		// Si on a atteint la cible, choisir une nouvelle cible.
		if (transform.rotation == angleCible) {
			Vector3 angleCibleEuler = kAngleBase + new Vector3(Random.Range(-kAngleMax.x, kAngleMax.x),	
			                                                   Random.Range(-kAngleMax.y, kAngleMax.y),	
			                                                   Random.Range(-kAngleMax.z, kAngleMax.z));
			angleCible = Quaternion.Euler(angleCibleEuler);
		}

		// Animer vers la couleur cible.
		timerCouleur += kVitesseCouleur * Time.deltaTime;
		if (timerCouleur > 1) {
			ChoisirNouvelleCouleurCible();
		} else {
			light.color = Color.Lerp(couleurActuelle, couleurCible, timerCouleur);
		}

		// Animer l'intensité selon que les spots sont activés ou désactivés.
		float newIntensity = light.intensity;
		if (active) {
			newIntensity += kIntensity * Time.deltaTime;
		} else if (!active) {
			newIntensity -= kIntensity * Time.deltaTime;
		}
		if (newIntensity > kIntensity) {
			newIntensity = kIntensity;
		} else if (newIntensity < 0) {
			newIntensity = 0;
		}
		light.intensity = newIntensity;
	}

	// Choisir une nouvelle couleur cible.
	void ChoisirNouvelleCouleurCible () {
		timerCouleur = 0;
		couleurActuelle = light.color;
		couleurCible = kCouleursPossibles [Random.Range (0, kCouleursPossibles.Count)];
	}

	public static void SetActive(bool estActive) {
		active = estActive;
	}

	// Intensité.
	private static float kIntensity;

	// Indique si les spots fous sont activés.
	private static bool active;

	// Couleurs possibles.
	private List<Color> kCouleursPossibles = new List<Color> ();

	// Couleur actuelle.
	private Color couleurActuelle;

	// Couleur cible.
	private Color couleurCible;

	// Timer de couleur.
	private float timerCouleur = 0;

	// Vitesse de changement de la couleur.
	private const float kVitesseCouleur = 0.25f;

	// Angle cible.
	private Quaternion angleCible;

	// Vitesse du spot.
	private const float kVitesseRotation = 15;

	// Angle de base.
	private Vector3 kAngleBase = new Vector3(90, 0, 0);

	// Mouvements permis pour les angles.
	private Vector3 kAngleMax = new Vector3(45, 45, 45);

}
