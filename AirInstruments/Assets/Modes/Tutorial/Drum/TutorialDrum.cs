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
	                    DrumDecoration bassDecoration,
	                    DrumDecoration crashDecoration,
                        DrumDecoration highHatDecoration,
                        DrumDecoration rideDecoration,
                        DrumDecoration snareDecoration,
                        DrumDecoration tom1Decoration,
                        DrumDecoration tom2Decoration,
                        DrumDecoration tomBigDecoration,
	                    AudioClip sonPosition,
	                    AudioClip sonTambours,
	                    AudioClip sonImprovisez,
	                    AudioClip sonMitrailler,
	                    AudioClip sonMitraillerRide,
	                    AudioClip sonAnglaisPosition,
	                    AudioClip sonAnglaisTambours,
	                    AudioClip sonAnglaisImprovisez,
	                    AudioClip sonAnglaisMitrailler,
	                    AudioClip sonAnglaisMitraillerRide) {
		this.tutorial = Tutorial.ObtenirInstance ();
		
		// Creer les etapes du tutorial.
		tutorial.ReinitialiserEtapes ();
		tutorial.AjouterEtape (new DrumEtapePosition(sonPosition, sonAnglaisPosition, tipLeft, tipRight));
		tutorial.AjouterEtape (new DrumEtapeTambours(sonTambours,
		                                             sonAnglaisTambours,
		                                             crash,
		                                             highHat,
		                                             ride,
		                                             snare,
		                                             tom1,
		                                             tom2,
		                                             tomBig,
		                                             bassDecoration,
		                                             crashDecoration,
		                                             highHatDecoration,
		                                             rideDecoration,
		                                             snareDecoration,
		                                             tom1Decoration,
		                                             tom2Decoration,
		                                             tomBigDecoration));
		tutorial.AjouterEtape (new DrumEtapeMitrailler(sonMitrailler,
		                                               sonAnglaisMitrailler,
		                                               crash,
		                                               highHat,
		                                               ride,
		                                               snare,
		                                               tom1,
		                                               tom2,
		                                               tomBig,
		                                               bassDecoration,
		                                               crashDecoration,
		                                               highHatDecoration,
		                                               rideDecoration,
		                                               snareDecoration,
		                                               tom1Decoration,
		                                               tom2Decoration,
		                                               tomBigDecoration));
		tutorial.AjouterEtape (new DrumEtapeMitraillerRide(sonMitraillerRide,
		                                                   sonAnglaisMitraillerRide,
		                                                   crash,
		                                                   highHat,
		                                                   ride,
		                                                   snare,
		                                                   tom1,
		                                                   tom2,
		                                                   tomBig,
		                                                   bassDecoration,
		                                                   crashDecoration,
		                                                   highHatDecoration,
		                                                   rideDecoration,
		                                                   snareDecoration,
		                                                   tom1Decoration,
		                                                   tom2Decoration,
		                                                   tomBigDecoration));	
		tutorial.AjouterEtape (new DrumEtapeImprovisez(sonImprovisez, sonAnglaisImprovisez));
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
