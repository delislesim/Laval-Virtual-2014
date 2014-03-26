using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GuitareEtapeAssiste : EtapeTutorial {
	
	public GuitareEtapeAssiste(AudioClip son, AudioClip sonAnglais, AssistedModeControllerGuitar assistedModeController) {
		this.son = son;
		this.sonAnglais = sonAnglais;

		this.assistedModeController = assistedModeController;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Jouez une note lorsqu'une ligne bleue\ntouche le manche de la guitare.";
		else
			return "Play a note when a blue line touches the fretboard.";
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
		assistedModeController.StartSong (AssistedModeControllerGuitar.Chanson.TNT);
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		timer += Time.deltaTime;
		return timer > kTempsAffichage;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return false;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;

	// Voix lisant l'instruction en anglais.
	private AudioClip sonAnglais;
	
	// Mode assiste.
	private AssistedModeControllerGuitar assistedModeController;

	// Timer pour masquer le guidage.
	private float timer;
	
	// Temps d'affichage du guidage, en secondes.
	private const float kTempsAffichage = 3.0f;
	
}
