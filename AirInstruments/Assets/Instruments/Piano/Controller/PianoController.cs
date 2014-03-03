using UnityEngine;
using System.Collections;

public class PianoController : MonoBehaviour, InstrumentControllerInterface {

	// Controleur des mains du squelette a partir des donnees de la camera d'Intel.
	public IntelHandController intelHandController;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
	}

	// Methode appelee quand l'instrument "piano" est choisi.
	void OnEnable() {
		intelHandController.gameObject.SetActive (true);
	}
	
	// Methode appelee quand l'instrument "piano" n'est plus choisi.
	void OnDisable () {
		intelHandController.gameObject.SetActive (false);
	}

	// Methode appelee a chaque frame quand le piano est l'instrument courant.
	void Update () {
		// Verifier si le mode assiste est demande.
		if (Input.GetButtonDown ("MenuAssiste")) {
			MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();

			// Positionner le menu du mode assiste.
			menuAssiste.transform.position = new Vector3(-11.15239f, 1.369231f, -5.25142f);
			menuAssiste.transform.eulerAngles = new Vector3(323.81f, 180f, 0);
			menuAssiste.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

			// Activer le menu du mode assiste.
			menuAssiste.gameObject.SetActive (true);
		}
	}
}
