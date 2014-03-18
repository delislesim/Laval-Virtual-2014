using UnityEngine;
using System.Collections;

public class TutorialGuitare {
	
	public TutorialGuitare(AudioClip sonLeverBras,
	                       AudioClip sonCordes,
	                       AudioClip sonChangerNote,
	                       AudioClip sonAssiste,
	                       HandFollower handFollower,
	                       AssistedModeControllerGuitar assistedModeController) {
		this.tutorial = Tutorial.ObtenirInstance ();
		
		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new GuitareEtapeLeverBras (sonLeverBras));
		tutorial.AjouterEtape (new GuitareEtapeCordes (sonCordes, handFollower));
		tutorial.AjouterEtape (new GuitareEtapeChangerNote (sonChangerNote, handFollower));
		tutorial.AjouterEtape (new GuitareEtapeAssiste (sonAssiste, assistedModeController));
	}
	
	public void Demarrer() {
		tutorial.Demarrer (GuidageTutorial.Position.BAS);
	}
	
	public bool EstComplete() {
		return tutorial.EstComplete ();
	}
	
	// Tutorial.
	private Tutorial tutorial;
}
