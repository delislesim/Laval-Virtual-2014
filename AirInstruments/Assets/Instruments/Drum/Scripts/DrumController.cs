using UnityEngine;
using System.Collections;

public class DrumController : MonoBehaviour, InstrumentControllerInterface {

	// Controleur du squelette du joueur de drum.
	public MoveJoints jointsController;

	//Dude 
	public GameObject dude;

	// Tete du joueur de drum, utilise pour controler la camera.
	public GameObject teteDrummer;

	// Camera principale du jeu.
	public Camera mainCamera;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (4);
	}

	public void PrepareToStop() {
	}

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
		dude.gameObject.SetActive (true);

		// Mettre la tete du drummer a la position de la camera.
		teteDrummer.transform.position = mainCamera.transform.position;

		// Prendre le controle de la camera.
		mainCamera.transform.parent = teteDrummer.transform;
		mainCamera.transform.localPosition = Vector3.zero;
	}
	
	// Methode appelee quand l'instrument "drum" n'est plus choisi.
	void OnDisable () {
		dude.gameObject.SetActive (false);
	}
	
	// Methode appelee a chaque frame quand le drum est l'instrument courant.
	void Update () {
	}
}
