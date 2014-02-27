using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	// Definir l'instrument presentement actif.
	public void DefinirInstrument(Instrument instrument) {
		currentInstrument = instrument;
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
}
