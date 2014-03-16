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

	//Assisted Controller
	public DrumAssistedController assistedcontroller;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (4);
	}

	public void PrepareToStop() {
		gameObject.SetActive (false);
	}

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
		dude.gameObject.SetActive (true);
		//assistedcontroller.gameObject.SetActive(true);

		// Mettre la tete du drummer a la position de la camera.
		teteDrummer.transform.position = mainCamera.transform.position;
		Quaternion rotationCamera = mainCamera.transform.rotation;

		// Prendre le controle de la camera.
		mainCamera.transform.parent = teteDrummer.transform;
		mainCamera.transform.localPosition = Vector3.zero;
		mainCamera.transform.rotation = rotationCamera;

		// Activation du guidage
		GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.INSTRUMENTS);

		// Activation de la reconnaissance du geste de menu.
		GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
		gestureRecognition.AddGesture (new GestureMenu());
	}
	
	// Methode appelee quand l'instrument "drum" n'est plus choisi.
	void OnDisable () {
		dude.gameObject.SetActive (false);
		assistedcontroller.gameObject.SetActive(false);

	}
	
	// Methode appelee a chaque frame quand le drum est l'instrument courant.
	void Update () {
		// Retour au menu de choix d'instrument a l'aide d'un geste.
		if (GestureRecognition.ObtenirInstance ().GetCurrentGesture () == GestureId.GESTURE_MENU) {
			GameState.ObtenirInstance().AccederEtat (GameState.State.ChooseInstrument);
			return;
		}
	}
}
