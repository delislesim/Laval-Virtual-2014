using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrumEtapeMitraillerRide : EtapeTutorial {
	
	// Liste de composants a faire apparaitre un a la fois.
	private List<DrumDecoration> decoration = new List<DrumDecoration>();

	// Liste de tambours a frapper.
	private List<ComponentInterface> components = new List<ComponentInterface>();

	public DrumEtapeMitraillerRide(AudioClip son,
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
		
		decoration.Add (snareDecoration);
		decoration.Add (rideDecoration);
		decoration.Add (crashDecoration);
		decoration.Add (highHatDecoration);
		decoration.Add (tom1Decoration);
		decoration.Add (tom2Decoration);
		decoration.Add (tomBigDecoration);
		decoration.Add (bassDecoration);

		components.Add (snare);
		components.Add (ride);
		components.Add (crash);
		components.Add (highHat);
		components.Add (tom1);
		components.Add (tom2);
		components.Add (tomBig);

	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Jouez les 2 instruments en même temps.";
		else
			return "Play both drums at the same time.";
	}
	
	// Retourne la voix lisant l'instruction.
	public AudioClip ObtenirAudio() {
		return son;
	}
	
	// Retourne le nom de l'animation a jouer.
	public Texture[] ObtenirAnimation() {
		return null;
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {
		decoration [1].Afficher ();
		components [1].DoitEtreJoue (false);
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		if (!components[1].AEteJoue())
			return false;

		timer += Time.deltaTime;
		if (timer > kTempsIndividuel) {
			for (int i = 0; i < decoration.Count; ++i)
				decoration[i].Afficher();
			return true;
		}		
		return false;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;
	
	// Timer pour afficher des composants.
	private float timer = 0;
	
	// Temps pendant lequel un composant est affiche individuellement.
	private const float kTempsIndividuel = 4.0f;
	
}
