using UnityEngine;
using System.Collections;

public interface Instrument {

	// Joue la note de force (accompagnement).
	void PlayNoteOverride(int index);

	// Indique que le joueur doit jouer la note specifiee.
	void PlayNotePlayer(int index);

	// Indique que le joueur ne doit pas jouer la note specifiee.
	void DontPlayNotePlayer(int index);

	void GetNoteInfo(int index, out float positionHorizontale, out float largeur);
}
