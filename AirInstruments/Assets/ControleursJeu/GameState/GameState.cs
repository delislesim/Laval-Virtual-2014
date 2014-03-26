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

	// Assistance.
	public GameObject assistance;

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
			return;
		} else if (Input.GetButtonDown("Piano")) {
			AccederEtat (State.Piano);
			return;
		} else if (Input.GetButtonDown("Drum")) {
			AccederEtat (State.Drum);
			return;
		} else if (Input.GetButtonDown("Guitare")) {
			AccederEtat (State.Guitar);
			return;
		} else if (Input.GetKey (KeyCode.Escape)) {
				Application.Quit ();
				return;
		}

		// La guitare a besoin de continuer a animer son spot apres la
		// fin de l'animation de camera.
		if (transitionTerminee && previousState == State.Guitar) {
			guitareController.Update();
		}
	}

	// Acceder a un nouvel etat.
	public void AccederEtat(State state) {
		if (state == currentState || !transitionTerminee) {
			return;
		}

		KinectPower.SetAffichageSqueletteActive (false);

		transitionTerminee = false;
		previousState = currentState;

		// Voler la camera a la tete du drummer.
		cameraController.ReprendreCamera ();

		// Arreter l'etat presentement actif.
		if (previousState == State.Drum) {
			drumController.PrepareToStop();
		} else if (previousState == State.Piano) {
			pianoController.PrepareToStop();
		} else if (previousState == State.Guitar) {
			guitareController.PrepareToStop();			
		} else if (previousState == State.ChooseInstrument) {
			choixInstrumentControleur.PrepareToStop();
		}

		// Effacer tous les gestes.
		GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
		gestureRecognition.ClearGestures ();

		// Effacer tous les guidages.
		GuidageController.ObtenirInstance ().changerGuidage (typeGuidage.AUCUN);

		// Activer le nouvel etat.
		currentState = state;
		if (currentState == State.ChooseInstrument) {

			cameraController.AccederEtat(previousState, State.ChooseInstrument);
			assistance.SetActive (true);
			choixInstrumentControleur.Prepare();

		} else if (currentState == State.Piano) {

			cameraController.AccederEtat(previousState, State.Piano);
			pianoController.Prepare();

		} else if (currentState == State.Drum) {

			cameraController.AccederEtat(previousState, State.Drum);
			drumController.Prepare();

		} else if (currentState == State.Guitar) {

			cameraController.AccederEtat(previousState, State.Guitar);
			guitareController.Prepare();

		}
	}

	// Appele lorsque l'animation de camera permettant de se rendre a l'etat
	// suivant est terminee.
	public void OnCompleteTransition() {
		// Arreter l'etat presentement actif.
		if (previousState == State.Drum) {
			drumController.gameObject.SetActive (false);
		} else if (previousState == State.Piano) {
			pianoController.gameObject.SetActive (false);
		} else if (previousState == State.Guitar) {
			guitareController.gameObject.SetActive (false);		
		} else if (previousState == State.ChooseInstrument) {
			choixInstrumentControleur.gameObject.SetActive (false);
		}

		cameraController.AjusterRotation ();

		if (currentState == State.ChooseInstrument) {

			choixInstrumentControleur.gameObject.SetActive (true);

		} else if (currentState == State.Piano) {

			pianoController.gameObject.SetActive (true);
			
		} else if (currentState == State.Drum) {

			drumController.gameObject.SetActive (true);
			
		} else if (currentState == State.Guitar) {

			guitareController.gameObject.SetActive (true);
			guitareController.AnimationTerminee();
			
		}

		// Desactiver l'assistance pour tous les etats sauf le choix d'instrument.
		if (currentState != State.ChooseInstrument) {
			assistance.SetActive (false);
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

	// Etat precedent.
	private GameState.State previousState = State.Unknown;

	// Indique si la transition vers l'etat courant est terminee.
	private bool transitionTerminee = true;

	// Unique instance de la classe GameState.
	private static GameState instance;

}
