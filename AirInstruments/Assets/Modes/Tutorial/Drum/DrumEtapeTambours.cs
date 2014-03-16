using UnityEngine;
using System.Collections;

public class DrumEtapeTambours : EtapeTutorial {
	
	public DrumEtapeTambours(AudioClip son,
	                         DrumComponent crash,
	                         HighHatComponent highHat,
	                         DrumComponent ride,
	                         DrumComponent snare,
	                         DrumComponent tom1,
	                         DrumComponent tom2,
	                         DrumComponent tomBig) {
		this.son = son;

		this.crash = crash;
		this.highHat = highHat;
		this.ride = ride;
		this.snare = snare;
		this.tom1 = tom1;
		this.tom2 = tom2;
		this.tomBig = tomBig;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		return "Positionnez le bout de vos\nbaguettes au-dessus de la\nbatterie.";
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
		// Ne rien faire.
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		return true;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return false;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;
	
	// Composants du drum.
	DrumComponent crash;
	HighHatComponent highHat;
	DrumComponent ride;
	DrumComponent snare;
	DrumComponent tom1;
	DrumComponent tom2;
	DrumComponent tomBig;
	
}
