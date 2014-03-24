using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrumEtapeMitrailler : EtapeTutorial {

	// Liste de composants a faire apparaitre un a la fois.
	private List<DrumDecoration> decoration = new List<DrumDecoration>();

	public DrumEtapeMitrailler(AudioClip son,
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
	                           DrumDecoration tomBigDecoration) {
		this.son = son;

		decoration.Add (snareDecoration);
		decoration.Add (bassDecoration);
		decoration.Add (crashDecoration);
		decoration.Add (highHatDecoration);
		decoration.Add (rideDecoration);
		decoration.Add (tom1Decoration);
		decoration.Add (tom2Decoration);
		decoration.Add (tomBigDecoration);
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		return "Tapez rapidement sur le tambour qui se trouvent devant vous.";
	}
	
	// Retourne la voix lisant l'instruction.
	public AudioClip ObtenirAudio() {
		return son;
	}
	
	// Retourne le nom de l'animation a jouer.
	public string ObtenirAnimation() {
		return "piano-etape-assiste.png";
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {
		for (int i = 0; i < decoration.Count; ++i) {
			decoration[i].Cacher();
		}
		decoration [0].Afficher ();
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {

		
		return false;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}

	// Voix lisant l'instruction.
	private AudioClip son;

}
