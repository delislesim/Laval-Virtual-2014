﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrumEtapeTambours : EtapeTutorial {
	
	public DrumEtapeTambours(AudioClip son,
	                         AudioClip sonAnglais,
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
		this.sonAnglais = sonAnglais;

		components.Add (tom1);
		components.Add (tom2);
		components.Add (highHat);
		components.Add (ride);
		//components.Add (tomBig);
		//components.Add (crash);
		//components.Add (snare);
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Jouez les tambours et cymbales qui deviennent bleus.";
		else
			return "Play the drums and cymbals when they become blue.";
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
		if (prochainComposant >= components.Count)
			return true;

		if (prochainComposant == -1 || components [prochainComposant].AEteJoue()) {
			++prochainComposant;
			if (prochainComposant >= components.Count) {
				prochainComposant = components.Count;
				return true;
			} else {
				// Jouer le prochain composant.
				components [prochainComposant].DoitEtreJoue(true);
			}
		}

		return false;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}

	// Index du prochain composant a frapper.
	private int prochainComposant = -1;
	
	// Voix lisant l'instruction.
	private AudioClip son;

	// Voix lisant l'instruction en anglais.
	private AudioClip sonAnglais;

	// Liste de tambours a frapper.
	private List<ComponentInterface> components = new List<ComponentInterface>();
	
}
