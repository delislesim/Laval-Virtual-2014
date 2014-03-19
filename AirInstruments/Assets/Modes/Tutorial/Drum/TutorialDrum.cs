using UnityEngine;
using System.Collections;

public class TutorialDrum {
	
	public TutorialDrum(GameObject tipLeft,
	                    GameObject tipRight,
						DrumComponent crash,
	                    DrumComponent highHat,
	                    DrumComponent ride,
	                    DrumComponent snare,
	                    DrumComponent tom1,
	                    DrumComponent tom2,
	                    DrumComponent tomBig,
	                    AudioClip sonPosition,
	                    AudioClip sonTambours,
	                    AudioClip sonImprovisez) {
		this.tutorial = Tutorial.ObtenirInstance ();
		
		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new DrumEtapePosition(sonPosition, tipLeft, tipRight));
		tutorial.AjouterEtape (new DrumEtapeTambours(sonTambours,
		                                             crash,
		                                             highHat,
		                                             ride,
		                                             snare,
		                                             tom1,
		                                             tom2,
		                                             tomBig));
		tutorial.AjouterEtape (new DrumEtapeImprovisez(sonImprovisez));
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
