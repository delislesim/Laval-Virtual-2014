using UnityEngine;
using System.Collections;

public class GuitareController : MonoBehaviour {

	// Controleur du mode assiste de la guitare.
	public AssistedModeControllerGuitar assistedModeController;

	// Controleur du squelette du joueur de guitare.
	public MoveJointsForGuitar moveJoints;

	// Joueur de guitare.
	public GuitarPlayer guitarPlayer;

	// Methode appelee quand l'instrument "guitare" est choisi.
	void OnEnable() {
		assistedModeController.gameObject.SetActive (true);
		moveJoints.gameObject.SetActive (true);
		guitarPlayer.gameObject.SetActive (true);
	}

	// Methode appelee quand l'instrument "guitare" n'est plus choisi.
	void OnDisable () {
		assistedModeController.gameObject.SetActive (false);
		moveJoints.gameObject.SetActive (false);
		guitarPlayer.gameObject.SetActive (false);
	}

	// Methode appelee a chaque frame quand la guitare est l'instrument courant.
	void Update () {
	}
}
