using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	// Controleur du piano.
	public PianoController pianoController;

	// Controleur de la guitare.
	public GuitareController guitareController;

	// Controleur du drum.
	public DrumController drumController;

	// Controleur de la camera.
	public CameraController cameraController;

	// Retourne l'unique instance de la classe GameState.
	public static GameState ObtenirInstance() {
		return instance;
	}

	void Start () {
		instance = this;
		AccederEtat (State.ChooseInstrument);
	}

	void Update() {
		// Changements d'etat a l'aide du clavier.
		if (Input.GetButtonDown("Piano")) {
			AccederEtat (State.Piano);
		} else if (Input.GetButtonDown("Drum")) {
			AccederEtat (State.Drum);
		} else if (Input.GetButtonDown("Guitare")) {
			AccederEtat (State.Guitar);
		}


	}

	// Acceder a un nouvel etat.
	private void AccederEtat(State state) {
		if (state == currentState) {
			return;
		}

		// Arreter l'etat presentement actif.
		pianoController.gameObject.SetActive (false);
		drumController.gameObject.SetActive (false);
		guitareController.gameObject.SetActive (false);

		// Activer le nouvel etat.
		currentState = state;
		if (currentState == State.ChooseInstrument) {

			Debug.Log("Aller au choix d'instrument.");

		} else if (currentState == State.Piano) {

			cameraController.AccederEtat(State.ChooseInstrument, State.Piano);
			//pianoController.gameObject.SetActive (true);
			Debug.Log("Piano choisi.");

		} else if (currentState == State.Drum) {

			cameraController.AccederEtat(State.ChooseInstrument, State.Drum);
			//drumController.gameObject.SetActive (true);
			Debug.Log("Drum choisi.");

		} else if (currentState == State.Guitar) {

			cameraController.AccederEtat(State.ChooseInstrument, State.Guitar);
			//guitareController.gameObject.SetActive (true);
			Debug.Log("Guitare choisie.");

		}
		transitionTerminee = false;
	}

	// Etats possibles du jeu.
	public enum State {
		Unknown,
		ChooseInstrument,
		Guitar,
		Piano,
		Drum
	}

	// Etat presentement actif.
	private State currentState = State.Unknown;

	// Indique si la transition vers l'etat courant est terminee.
	private bool transitionTerminee = false;

	// Unique instance de la classe GameState.
	private static GameState instance;

}
