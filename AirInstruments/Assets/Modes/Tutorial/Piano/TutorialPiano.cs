using UnityEngine;
using System.Collections;

public class TutorialPiano {

	public TutorialPiano(IntelHandController intelHandController,
	                     AssistedModeControllerPiano assistedModeController,
	                     AudioClip sonCapteur,
                         AudioClip sonPosition,
                         AudioClip sonGamme,
                         AudioClip sonAssiste,
	                     AudioClip sonAnglaisCapteur,
	                     AudioClip sonAnglaisPosition,
	                     AudioClip sonAnglaisGamme,
	                     AudioClip sonAnglaisAssiste) {
		this.tutorial = Tutorial.ObtenirInstance ();

		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new PianoEtapeCapteur (sonCapteur, sonAnglaisCapteur, intelHandController));
		tutorial.AjouterEtape (new PianoEtapePosition (sonPosition, sonAnglaisPosition, intelHandController));
		tutorial.AjouterEtape (new PianoEtapeGamme (sonGamme, sonAnglaisGamme, intelHandController, assistedModeController));
		tutorial.AjouterEtape (new PianoEtapeAssiste (sonAssiste, sonAnglaisAssiste, intelHandController, assistedModeController));
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
