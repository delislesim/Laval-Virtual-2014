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

	// Controleur du menu de choix d'instrument.
	public ChoixInstrumentController choixInstrumentControleur;

	// Controleur de la camera.
	public CameraController cameraController;

	// Retourne l'unique instance de la classe GameState.
	public static GameState ObtenirInstance() {
		return instance;
	}

	public GameState() {
		instance = this;
	}

	void Start () {
		// Activer l'etat initial de choix d'instrument.
		AccederEtat (State.ChooseInstrument);
	}

	void Update() {
		// Changements d'etat a l'aide du clavier.
		if (Input.GetButtonDown("ChoixInstrument")) {
			AccederEtat (State.ChooseInstrument);
		} else if (Input.GetButtonDown("Piano")) {
			AccederEtat (State.Piano);
		} else if (Input.GetButtonDown("Drum")) {
			AccederEtat (State.Drum);
		} else if (Input.GetButtonDown("Guitare")) {
			AccederEtat (State.Guitar);
		}
	}

	// Acceder a un nouvel etat.
	public void AccederEtat(State state) {
		if (state == currentState || !transitionTerminee) {
			return;
		}

		GameState.State previousState = currentState;

		// Voler la camera.
		cameraController.ReprendreCamera ();

		// Arreter l'etat presentement actif.
		choixInstrumentControleur.gameObject.SetActive (false);
		pianoController.gameObject.SetActive (false);
		
		drumController.gameObject.SetActive (false);

		guitareController.ShowJoints(false);
		guitareController.gameObject.SetActive (false);

		transitionTerminee = false;

		// Activer le nouvel etat.
		currentState = state;
		if (currentState == State.ChooseInstrument) {

			cameraController.AccederEtat(previousState, State.ChooseInstrument);
			choixInstrumentControleur.Prepare();
			Debug.Log("Aller au choix d'instrument.");

		} else if (currentState == State.Piano) {

			cameraController.AccederEtat(previousState, State.Piano);
			pianoController.Prepare();
			Debug.Log("Piano choisi.");

		} else if (currentState == State.Drum) {

			cameraController.AccederEtat(previousState, State.Drum);
			drumController.Prepare();
			Debug.Log("Drum choisi.");

		} else if (currentState == State.Guitar) {

			cameraController.AccederEtat(previousState, State.Guitar);
			guitareController.Prepare();
			Debug.Log("Guitare choisie.");

		}
	}

	// Appele lorsque l'animation de camera permettant de se rendre a l'etat
	// suivant est terminee.
	public void OnCompleteTransition() {
		if (currentState == State.ChooseInstrument) {

			choixInstrumentControleur.gameObject.SetActive (true);

		} else if (currentState == State.Piano) {

			pianoController.gameObject.SetActive (true);
			
		} else if (currentState == State.Drum) {

			drumController.gameObject.SetActive (true);
			
		} else if (currentState == State.Guitar) {

			guitareController.gameObject.SetActive (true);
			guitareController.ShowJoints(true);
			
		}

		transitionTerminee = true;
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
	private bool transitionTerminee = true;

	// Unique instance de la classe GameState.
	private static GameState instance;

}
