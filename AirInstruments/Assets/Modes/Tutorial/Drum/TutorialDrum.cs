using UnityEngine;
using System.Collections;

public class TutorialDrum {
	
	public TutorialDrum(GameObject tipLeft,
	                    GameObject tipRight,
						DrumComponent crash,
	                    HighHatComponent highHat,
	                    DrumComponent ride,
	                    DrumComponent snare,
	                    DrumComponent tom1,
	                    DrumComponent tom2,
	                    DrumComponent tomBig,
	                    AudioClip sonPosition,
	                    AudioClip sonTambours) {
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
