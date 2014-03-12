using UnityEngine;
using System.Collections;

public interface Instrument {

	void DefinirStatutNote(int indexNote, PartitionPiano.StatutNote statut);

	// Indique si la note est adjacente a une note qui doit etre jouee
	// selon la partition.
	void DefinirAdjacentAJouer(int indexNote, bool adjacentAJouer);

	void ObtenirInfoNotePourCubesTombants(int index, out float positionHorizontale, out float largeur);

	PianoNote ObtenirNote(int index);
}
