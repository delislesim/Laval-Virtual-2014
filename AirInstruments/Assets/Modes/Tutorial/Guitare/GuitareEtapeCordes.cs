﻿using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GuitareEtapeCordes : EtapeTutorial {
	
	public GuitareEtapeCordes(AudioClip son, HandFollower handFollower) {
		this.son = son;
		this.handFollower = handFollower;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		return "Faites de grands mouvements avec la main droite\npour jouer des notes.";
	}
	
	// Retourne la voix lisant l'instruction.
	public AudioClip ObtenirAudio() {
		return son;
	}
	
	// Retourne le nom de l'animation a jouer.
	public string ObtenirAnimation() {
		return "";
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {
		handFollower.ReinitialiserMouvementsAmples ();
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		return handFollower.ObtenirNumMouvementsAmples () > 3;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;

	// Squelette.
	private Skeleton skeleton = new Skeleton(0);

	// Hand follower.
	private HandFollower handFollower;
	
}
