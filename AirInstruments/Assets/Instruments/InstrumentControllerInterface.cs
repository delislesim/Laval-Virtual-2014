using UnityEngine;
using System.Collections;

public interface InstrumentControllerInterface {

	// Appele au debut du mouvement de camera pour se rendre a l'instrument.
	void Prepare();

	void PrepareToStop();

}
