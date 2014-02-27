using UnityEngine;
using System.Collections;

public class PianoController : MonoBehaviour {

	// Controleur des mains du squelette a partir des donnees de la camera d'Intel.
	public IntelHandController intelHandController;

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
	
	}
}
