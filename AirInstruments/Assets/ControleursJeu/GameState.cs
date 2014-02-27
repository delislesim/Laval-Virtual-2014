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
		} else if (currentInstrument == Instrument.Drum) {
			drumController.gameObject.SetActive (true);
		} else if (currentInstrument == Instrument.Guitar) {
			guitareController.gameObject.SetActive (true);
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
