using UnityEngine;
using System.Collections;

public interface Instrument {

	void DefinirStatutNote(int indexNote, PartitionPiano.StatutNote statut);

	void ObtenirInfoNotePourCubesTombants(int index, out float positionHorizontale, out float largeur);

	PianoNote ObtenirNote(int index);
}
