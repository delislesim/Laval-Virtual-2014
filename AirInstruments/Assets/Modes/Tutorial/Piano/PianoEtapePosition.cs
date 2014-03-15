using UnityEngine;
using System.Collections;

public class PianoEtapePosition : EtapeTutorial {

	public PianoEtapePosition(IntelHandController handController) {
		this.handController = handController;
	}

	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		return "Suivez les fleches pour approcher vos mains du piano.";
	}
	
	// Retourne le nom du fichier audio a jouer.
	public string ObtenirAudio() {
		return "piano-etape-position.wav";
	}
	
	// Retourne le nom de l'animation a jouer.
	public string ObtenirAnimation() {
		return "piano-etape-position.png";
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {
		// Ne rien faire.
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		return handController.MainsSontPresDuPiano ();
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}

	// Controleur de mains, permettant de savoir si l'etape est completee.
	private IntelHandController handController;
	
}
