using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Position de la camera pour l'affichage du menu de choix d'instrument.
	public GameObject pointDepart;



	// Camera du jeu.
	public Camera mainCamera;
	
	void Start () {
		// Toujours commencer a la position initiale (choix d'instrument).
		mainCamera.transform.position = pointDepart.transform.position;
		mainCamera.transform.LookAt (Vector3.zero);
	}

	// Update is called once per frame.
	void Update () {

	}

	// Deplace la camera entre les etats specifies.
	public void AccederEtat (GameState.State from, GameState.State to) {
		// Aller vers le drum.
		if (from == GameState.State.ChooseInstrument &&
		    to == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersDrum").Play();
		}
		// Aller vers le piano.


	}
}
