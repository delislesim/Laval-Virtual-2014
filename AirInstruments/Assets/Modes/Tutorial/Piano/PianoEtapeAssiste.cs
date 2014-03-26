using UnityEngine;
using System.Collections;

public class PianoEtapeAssiste : EtapeTutorial {
	
	public PianoEtapeAssiste(AudioClip son,
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
			return "Continuez d'appuyer sur les notes bleues pour jouer un air connu.";
		else
			return "Keep on pressing the blue keys in order\nto play a popular song.";
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
		assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\valse.txt", 4.0f);
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
	
	// Controleur de mains, permettant de savoir si l'etape est completee.
	private IntelHandController handController;
	
	// Controleur du mode assiste.
	private AssistedModeControllerPiano assistedModeController;

	// Timer pour masquer le guidage.
	private float timer;

	// Temps d'affichage du guidage, en secondes.
	private const float kTempsAffichage = 3.0f;
	
}
