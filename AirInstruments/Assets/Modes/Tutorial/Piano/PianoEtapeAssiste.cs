using UnityEngine;
using System.Collections;

public class PianoEtapeAssiste : EtapeTutorial {
	
	public PianoEtapeAssiste(AudioClip son,
	                         IntelHandController handController,
	                         AssistedModeControllerPiano assistedModeController) {
		this.son = son;
		this.handController = handController;
		this.assistedModeController = assistedModeController;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Continuez d'appuyer sur les notes bleues pour jouer un air connu.";
		else
			return "Keep on pressing the blue keys in order to play a popular melody.";
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
	
	// Controleur de mains, permettant de savoir si l'etape est completee.
	private IntelHandController handController;
	
	// Controleur du mode assiste.
	private AssistedModeControllerPiano assistedModeController;

	// Timer pour masquer le guidage.
	private float timer;

	// Temps d'affichage du guidage, en secondes.
	private const float kTempsAffichage = 3.0f;
	
}
