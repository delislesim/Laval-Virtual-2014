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
		tutorial.Demarrer (GuidageTutorial.Position.HAUT);
	}

	public bool EstComplete() {
		return tutorial.EstComplete ();
	}

	// Tutorial.
	private Tutorial tutorial;
}
