using UnityEngine;
using System.Collections;

public class DrumController : MonoBehaviour, InstrumentControllerInterface {

	// Controleur du squelette du joueur de drum.
	public MoveJoints jointsController;

	//Dude 
	public Joints joints;
	public Joints DrumStickR;
	public Joints DrumStickL;

	// Tete du joueur de drum, utilise pour controler la camera.
	public GameObject teteDrummer;

	// Camera principale du jeu.
	public Camera mainCamera;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (4);
	}

	//Show/Hide joints
	public void ShowJoints(bool show)
	{
		joints.gameObject.SetActive(show);
		DrumStickR.gameObject.SetActive(show);
		DrumStickL.gameObject.SetActive(show);
	}

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
		jointsController.gameObject.SetActive (true);

		// Prendre le controle de la camera.
		mainCamera.transform.parent = teteDrummer.transform;
		mainCamera.transform.localPosition = Vector3.zero;

		// Fournir la camera au controleur du squelette.
		jointsController.AssignerCamera (mainCamera);
	}
	
	// Methode appelee quand l'instrument "drum" n'est plus choisi.
	void OnDisable () {
		jointsController.gameObject.SetActive (false);
	}
	
	// Methode appelee a chaque frame quand le drum est l'instrument courant.
	void Update () {
	}
}
