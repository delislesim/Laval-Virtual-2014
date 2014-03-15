using UnityEngine;
using System.Collections;

public class TutorialPiano {

	public TutorialPiano(IntelHandController intelHandController,
	                     AssistedModeControllerPiano assistedModeController,
	                     AudioClip sonCapteur,
                         AudioClip sonPosition,
                         AudioClip sonGamme,
                         AudioClip sonAssiste) {
		this.tutorial = Tutorial.ObtenirInstance ();

		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new PianoEtapeCapteur (sonCapteur, intelHandController));
		tutorial.AjouterEtape (new PianoEtapePosition (sonPosition, intelHandController));
		tutorial.AjouterEtape (new PianoEtapeGamme (sonGamme, intelHandController, assistedModeController));
		tutorial.AjouterEtape (new PianoEtapeAssiste (sonAssiste, intelHandController, assistedModeController));
	}

	public void Demarrer() {
		tutorial.Demarrer (kPositionGuidage,
		                   new Vector3(kTailleGuidage, kTailleGuidage, kTailleGuidage));
	}

	public bool EstComplete() {
		return tutorial.EstComplete ();
	}

	// Tutorial.
	private Tutorial tutorial;

	// Position du guidage.
	private Vector3 kPositionGuidage = new Vector3(-13.8f, 3.66f, -17.286f);

	// Taille du guidage.
	private const float kTailleGuidage = 0.15f;
}
