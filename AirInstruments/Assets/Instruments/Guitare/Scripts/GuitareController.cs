using UnityEngine;
using System.Collections;

public class GuitareController : MonoBehaviour, InstrumentControllerInterface {

	// Controleur du mode assiste de la guitare.
	public AssistedModeControllerGuitar assistedModeController;

	// Controleur du squelette du joueur de guitare.
	public MoveJointsForGuitar moveJoints;

	// Joueur de guitare.
	public GuitarPlayer guitarPlayer;

	// joints unity
	public Joints jointsObject;

	//Objet de guitare
	public Transform GuitarContainer;

	private Vector3 DEFAULT_GUIT_POS;
	private Vector3 DEFAULT_GUIT_ROT;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
	}

	private void Start(){
		GuitarContainer.transform.position = DEFAULT_GUIT_POS;
		GuitarContainer.transform.rotation = Quaternion.Euler(DEFAULT_GUIT_ROT);
	}

	//Show/Hide joints
	//place guitar
	public void ShowJoints(bool show)
	{
		jointsObject.gameObject.SetActive(show);
		if(!show)
		{
			DEFAULT_GUIT_POS = new Vector3(-5.187511f, 4.332658f, 9.69f);
			DEFAULT_GUIT_ROT = new Vector3(0, 180, 90);
			GuitarContainer.transform.localPosition = DEFAULT_GUIT_POS;
			GuitarContainer.transform.rotation = Quaternion.Euler(DEFAULT_GUIT_ROT);
		}
	}

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
