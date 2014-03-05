using UnityEngine;
using System.Collections;

public class GuitareController : MonoBehaviour, InstrumentControllerInterface {

	// Wrapper de tout ce qui doit etre active pour la guitare.
	public GameObject guitareWrapper;

	// joints unity
	public Joints jointsObject;

	//Objet de guitare
	public Transform GuitarContainer;

	// Giga spot a allumer pour switcher la guitare.
	public Light gigaSpotGuitare;

	// Guitare decorative, pour le menu de choix d'instrument.
	public GameObject guitareDecorative;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (10);
		tempsPreparation = 0;
		aFaitSwitch = false;
		estEnTrainDeQuitter = false;
		gameObject.SetActive (true);
	}

	public void PrepareToStop() {
		tempsPreparation = -4.0f;
		aFaitSwitch = false;
		estEnTrainDeQuitter = true;
	}

	public void Update() {
		// Retour au menu de choix d'instrument a l'aide d'un geste.
		if (GestureRecognition.ObtenirInstance ().GetCurrentGesture () == GestureId.GESTURE_MENU) {
			GameState.ObtenirInstance().AccederEtat (GameState.State.ChooseInstrument);
			return;
		}

		// Faire l'animation du gigaspot qui permet de switcher la guitare subtilement.
		tempsPreparation += Time.deltaTime;
		if (tempsPreparation > 0) {
			if (tempsPreparation < 0.5f) {
				gigaSpotGuitare.intensity += 16.0f * Time.deltaTime;
			} else if (tempsPreparation < 2.0f) {
				gigaSpotGuitare.intensity -= 5.33f * Time.deltaTime;
			} else if (gigaSpotGuitare.intensity != 0) {
				gigaSpotGuitare.intensity = 0;
			}

			// Switcher la guitare decorative / controlee par le joueur.
			if (tempsPreparation > 0.5f && !aFaitSwitch) {
				if (estEnTrainDeQuitter) {
					guitareWrapper.SetActive (false);
					guitareDecorative.SetActive (true);
				} else {
					guitareWrapper.SetActive (true);
					guitareDecorative.SetActive (false);
				}
				aFaitSwitch = true;
			}
		}
	}

	// Methode appelee quand l'instrument "guitare" est choisi.
	void OnEnable() {
		// L'initialisation se fait dans "Prepare".
	}

	// Methode appelee quand l'instrument "guitare" n'est plus choisi.
	void OnDisable () {
		// Faire le switch de guitare au cas ou il n'a pas deja ete fait.
		guitareWrapper.SetActive (false);
		guitareDecorative.SetActive (true);
	}

	// Temps depuis que le mode a commencé a se préparer.
	private float tempsPreparation;

	// Indique si on a fait le switch de guitare.
	private bool aFaitSwitch = false;

	// Indique si on est en train de quitter la scene.
	private bool estEnTrainDeQuitter = false;

}
