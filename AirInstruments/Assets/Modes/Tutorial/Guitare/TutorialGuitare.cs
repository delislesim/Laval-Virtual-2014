using UnityEngine;
using System.Collections;

public class TutorialGuitare {
	
	public TutorialGuitare(AudioClip sonLeverBras,
	                       AudioClip sonCordes,
	                       AudioClip sonAssiste,
	                       HandFollower handFollower,
	                       AssistedModeControllerGuitar assistedModeController) {
		this.tutorial = Tutorial.ObtenirInstance ();
		
		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new GuitareEtapeLeverBras (sonLeverBras));
		tutorial.AjouterEtape (new GuitareEtapeCordes (sonCordes, handFollower));
		tutorial.AjouterEtape (new GuitareEtapeAssiste (sonAssiste, assistedModeController));
	}
	
	public void Demarrer() {
		tutorial.Demarrer (kPositionGuidage);
	}
	
	public bool EstComplete() {
		return tutorial.EstComplete ();
	}
	
	// Tutorial.
	private Tutorial tutorial;
	
	// Position du guidage.
	private Vector2 kPositionGuidage = new Vector2(427.0f, 492.0f);
}
