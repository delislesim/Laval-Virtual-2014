using UnityEngine;
using System.Collections;

public interface EtapeTutorial {

	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	string ObtenirTexte();

	// Retourne le fichier audio a jouer.
	AudioClip ObtenirAudio();

	// Retourne le nom de l'animation a jouer.
	string ObtenirAnimation();

	// Appeler lorsque cette etape du tutorial debute.
	void Demarrer();

	// Indique si l'etape a ete completee avec success par le joueur.
	bool EstCompletee();

	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	bool DoitFeliciter();
}
