using UnityEngine;
using System.Collections;

public class DrumController : MonoBehaviour {

	// Controleur du squelette du joueur de drum.
	public MoveJoints jointsController;

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
		jointsController.gameObject.SetActive (true);
	}
	
	// Methode appelee quand l'instrument "drum" n'est plus choisi.
	void OnDisable () {
		jointsController.gameObject.SetActive (false);
	}
	
	// Methode appelee a chaque frame quand le drum est l'instrument courant.
	void Update () {
	}
}
