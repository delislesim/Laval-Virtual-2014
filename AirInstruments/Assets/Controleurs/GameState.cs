using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public Instrument initialInstrument = Instrument.Guitar;

	// Composantes qui doivent etre demarrees pour que le piano fonctionne,
	// et desactivees quand le piano n'est pas actif pour economiser les performances.
	public List<GameObject> composantesPiano = new List<GameObject> ();

	// Composantes qui doivent etre demarrees pour que le drum fonctionne,
	// et desactivees quand le drum n'est pas actif pour economiser les performances.
	public List<GameObject> composantesDrum = new List<GameObject> ();

	// Composantes qui doivent etre demarrees pour que la guitare fonctionne,
	// et desactivees quand la guitare n'est pas actif pour economiser les performances.
	public List<GameObject> composantesGuitar = new List<GameObject> ();

	// Retourne l'unique instance de la classe GameState.
	public static GameState ObtenirInstance() {
		return instance;
	}

	void Start () {
		instance = this;

		DefinirInstrument (initialInstrument);
	}

	// Definir l'instrument presentement actif.
	public void DefinirInstrument(Instrument instrument) {
		// Arreter la composante presentement active.
		DefinirComposantesPianoActives (false);
		DefinirComposantesDrumActives (false);
		DefinirComposantesGuitareActives (false);

		// Activer la nouvelle composante.
		currentInstrument = instrument;
		if (currentInstrument == Instrument.Piano) {
			DefinirComposantesPianoActives (true);
		} else if (currentInstrument == Instrument.Drum) {
			DefinirComposantesDrumActives (true);
		} else if (currentInstrument == Instrument.Guitar) {
			DefinirComposantesGuitareActives (true);
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

	// Active / desactive les composantes du piano.
	private void DefinirComposantesPianoActives(bool active) {
		for (int i = 0; i < composantesPiano.Count; ++i) {
			composantesPiano[i].SetActive(active);
		}
	}

	// Active / desactive les composantes du drum.
	private void DefinirComposantesDrumActives(bool active) {
		for (int i = 0; i < composantesDrum.Count; ++i) {
			composantesDrum[i].SetActive(active);
		}
	}

	// Active / desactive les composantes de la guitare.
	private void DefinirComposantesGuitareActives(bool active) {
		for (int i = 0; i < composantesGuitar.Count; ++i) {
			composantesGuitar[i].SetActive(active);
		}
	}

	// Instrument presentement actif.
	private Instrument currentInstrument = Instrument.None;

	// Unique instance de la classe GameState.
	private static GameState instance;

}
