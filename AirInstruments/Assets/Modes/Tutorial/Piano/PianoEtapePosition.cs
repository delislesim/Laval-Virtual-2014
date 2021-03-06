﻿using UnityEngine;
using System.Collections;

public class PianoEtapePosition : EtapeTutorial {

	public PianoEtapePosition(AudioClip son,
	                          AudioClip sonAnglais,
	                          IntelHandController handController) {
		this.son = son;
		this.sonAnglais = sonAnglais;

		this.handController = handController;
	}

	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Suivez les fleches pour approcher vos mains du piano.";
		else
			return "Follow the arrows to place your hands correctly.";
	}
	
	// Retourne la voix lisant l'instruction.
	public AudioClip ObtenirAudio() {
		if (Langue.isEnglish)
			return sonAnglais;
		else
			return son;
	}
	
	// Retourne le nom de l'animation a jouer.
	public Texture[] ObtenirAnimation() {
		return null;
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

	// Voix lisant l'instruction.
	private AudioClip son;

	// Voix lisant l'instruction en anglais.
	private AudioClip sonAnglais;

	// Controleur de mains, permettant de savoir si l'etape est completee.
	private IntelHandController handController;
	
}
