using UnityEngine;
using System.Collections;

public interface Instrument {

	void PlayNote(int index);
	void StopNote(int index);

	void GetNoteInfo(int index, out float positionHorizontale, out float largeur);
}
