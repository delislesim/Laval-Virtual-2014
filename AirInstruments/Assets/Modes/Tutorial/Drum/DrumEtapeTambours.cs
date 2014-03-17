using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		components.Add (tom1);
		components.Add (tom2);
		components.Add (highHat);
		components.Add (ride);
		components.Add (tomBig);
		components.Add (crash);
		components.Add (snare);
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		return "Jouez les tambours et cymbales qui deviennent bleus.";
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
		if (prochainComposant >= components.Count)
			return true;

		if (prochainComposant == -1 || components [prochainComposant].AEteJoue()) {
			++prochainComposant;
			if (prochainComposant >= components.Count) {
				prochainComposant = components.Count;
				return true;
			} else {
				// Jouer le prochain composant.
				components [prochainComposant].DoitEtreJoue();
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

	// Liste de tambours a frapper.
	private List<ComponentInterface> components = new List<ComponentInterface>();
	
}
