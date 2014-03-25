using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GuitareEtapeChangerNote : EtapeTutorial {
	
	public GuitareEtapeChangerNote(AudioClip son, HandFollower handFollower) {
		this.son = son;
		this.handFollower = handFollower;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Déplacez votre main sur le manche de\nla guitare pour changer la note.";
		else
			return "Move your hand over over the fretboard to change the played note.";
	}
	
	// Retourne la voix lisant l'instruction.
	public AudioClip ObtenirAudio() {
		return son;
	}
	
	// Retourne le nom de l'animation a jouer.
	public Texture[] ObtenirAnimation() {
		Texture[] textureChangerNote = new Texture[3];
		for (int i = 0; i < textureChangerNote.Length; i++) {
			int index = i + 1;
			textureChangerNote [i] = (Texture)Resources.Load ("TutorielGuitare/ChangerNote/" + index);
		}
		return textureChangerNote;
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {

	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		timer += Time.deltaTime;
		return timer > kTempsAffichage;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;

	// Timer.
	private float timer = 0;

	// Hand follower.
	private HandFollower handFollower;

	// Temps d'affichage de cette etape.
	private const float kTempsAffichage = 2.5f;
	
}
