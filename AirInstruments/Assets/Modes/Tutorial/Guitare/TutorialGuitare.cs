using UnityEngine;
using System.Collections;

public class TutorialGuitare {
	
	public TutorialGuitare(AudioClip sonLeverBras,
	                       AudioClip sonCordes,
	                       AudioClip sonChangerNote,
	                       AudioClip sonAssiste,
	                       AudioClip sonAnglaisLeverBras,
	                       AudioClip sonAnglaisCordes,
	                       AudioClip sonAnglaisChangerNote,
	                       AudioClip sonAnglaisAssiste,
	                       HandFollower handFollower,
	                       AssistedModeControllerGuitar assistedModeController) {
		this.tutorial = Tutorial.ObtenirInstance ();
		
		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new GuitareEtapeLeverBras (sonLeverBras, sonAnglaisLeverBras));
		tutorial.AjouterEtape (new GuitareEtapeCordes (sonCordes, sonAnglaisCordes, handFollower));
		tutorial.AjouterEtape (new GuitareEtapeChangerNote (sonChangerNote, sonAnglaisChangerNote, handFollower));
		tutorial.AjouterEtape (new GuitareEtapeAssiste (sonAssiste, sonAnglaisAssiste, assistedModeController));
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
