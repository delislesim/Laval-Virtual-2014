using UnityEngine;
using System.Collections;

public class DrumEtapePosition : EtapeTutorial {
	
	public DrumEtapePosition(AudioClip son,
	                         GameObject tipLeft,
	                         GameObject tipRight) {
		this.son = son;
		this.tipLeft = tipLeft;
		this.tipRight = tipRight;
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
	
	// Tip left.
	private GameObject tipLeft;

	// Tip right.
	private GameObject tipRight;
	
}
