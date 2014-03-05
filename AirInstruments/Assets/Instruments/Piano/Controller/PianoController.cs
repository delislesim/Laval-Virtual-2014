using UnityEngine;
using System.Collections;

public class PianoController : MonoBehaviour, InstrumentControllerInterface {

	// Englobe tout ce qui doit etre active/desactive quand le piano est active/desactive.
	public GameObject pianoWrapper;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
	}

	public void PrepareToStop() {
	}

	// Methode appelee quand l'instrument "piano" est choisi.
	void OnEnable() {
		Debug.Log ("start active");
		pianoWrapper.SetActive (true);
		Debug.Log ("stop active");
	}
	
	// Methode appelee quand l'instrument "piano" n'est plus choisi.
	void OnDisable () {
		pianoWrapper.SetActive (false);
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
