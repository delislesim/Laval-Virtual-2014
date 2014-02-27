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

	// Retourne l'unique instance de la classe GameState.
	public static GameState ObtenirInstance() {
		return instance;
	}

	void Start () {
		instance = this;
		DefinirInstrument (Instrument.None);
	}

	void Update() {
		if (Input.GetButtonDown("Piano")) {
			DefinirInstrument (Instrument.Piano);
		} else if (Input.GetButtonDown("Drum")) {
			DefinirInstrument (Instrument.Drum);
		} else if (Input.GetButtonDown("Guitare")) {
			DefinirInstrument (Instrument.Guitar);
		}
	}

	// Definir l'instrument presentement actif.
	public void DefinirInstrument(Instrument instrument) {
		// Arreter la composante presentement active.
		pianoController.gameObject.SetActive (false);
		drumController.gameObject.SetActive (false);
		guitareController.gameObject.SetActive (false);

		// Activer la nouvelle composante.
		currentInstrument = instrument;
		if (currentInstrument == Instrument.Piano) {
			pianoController.gameObject.SetActive (true);
			Debug.Log("Piano choisi.");
		} else if (currentInstrument == Instrument.Drum) {
			drumController.gameObject.SetActive (true);
			Debug.Log("Drum choisi.");
		} else if (currentInstrument == Instrument.Guitar) {
			guitareController.gameObject.SetActive (true);
			Debug.Log("Guitare choisie.");
		}
	}

	// Retourne l'instrument actif.
	public Instrument ObtenirInstrument() {
		return currentInstrument;
	}
	
	public enum Instrument {
		None,
		Guitar,
		Piano,
		Drum
	}

	// Instrument presentement actif.
	private Instrument currentInstrument = Instrument.None;

	// Unique instance de la classe GameState.
	private static GameState instance;

}
