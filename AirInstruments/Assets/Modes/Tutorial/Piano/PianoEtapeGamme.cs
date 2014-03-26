using UnityEngine;
using System.Collections;

public class PianoEtapeGamme : EtapeTutorial {

	public PianoEtapeGamme(AudioClip son,
	                       AudioClip sonAnglais,
	                       IntelHandController handController,
	                       AssistedModeControllerPiano assistedModeController) {
		this.son = son;
		this.sonAnglais = sonAnglais;

		this.handController = handController;
		this.assistedModeController = assistedModeController;
	}

	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Appuyez sur les notes bleues.";
		else
			return "Press the blue keys.";
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
		assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\gamme.txt", 4.0f);
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		return !AssistedModeControllerPiano.EstActive ();
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

	// Controleur du mode assiste.
	private AssistedModeControllerPiano assistedModeController;
	
}
