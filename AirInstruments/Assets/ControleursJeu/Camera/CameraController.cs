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
		// Aller de unknown (au debut) vers le menu de choix d'instrument.
		if (from == GameState.State.Unknown &&
		    to == GameState.State.ChooseInstrument) {
			GameState.ObtenirInstance().OnCompleteTransition();
		}
		// Aller vers le drum.
		else if (from == GameState.State.ChooseInstrument &&
		    to == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersDrum").Play();
		}
		// Aller vers le piano.
		else if (from == GameState.State.ChooseInstrument &&
		         to == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersPiano").Play();
		}
		// Aller vers la guitare.
		else if (from == GameState.State.ChooseInstrument &&
		    	 to == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersGuitare").Play();
		}
		// Quitter le drum.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterDrum").Play();
		}
		// Quitter le piano.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterPiano").Play();
		}
		// Quitter la guitare.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterGuitare").Play();
		}

	}
}
