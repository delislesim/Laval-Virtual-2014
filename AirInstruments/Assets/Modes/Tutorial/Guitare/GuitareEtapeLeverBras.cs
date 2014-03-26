using UnityEngine;
using System.Collections;
using KinectHelpers;

public class GuitareEtapeLeverBras : EtapeTutorial {
	
	public GuitareEtapeLeverBras(AudioClip son, AudioClip sonAnglais) {
		this.son = son;
		this.sonAnglais = sonAnglais;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Levez le bras gauche pour tenir la guitare.";
		else
			return "Hold up your left arm to hold up the guitar.";
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
		Texture[] textureBrasGauche = new Texture[1];
		for (int i = 0; i < textureBrasGauche.Length; i++) {
			int index = i + 1;
			textureBrasGauche [i] = (Texture)Resources.Load ("TutorielGuitare/BrasGauche/" + index);
		}
		return textureBrasGauche;
	}
	
	// Appeler lorsque cette etape du tutorial debute.
	public void Demarrer() {
		// Ne rien faire.
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		// Chargement du squelette.
		skeleton.ReloadSkeleton ();
		if (!skeleton.Exists() || !skeleton.IsDifferent())
			return false;

		// Obtenir toutes les articulations du bras gauche.
		Vector3 positionEpaule;
		if (skeleton.GetJointPosition (Skeleton.Joint.ShoulderLeft, out positionEpaule) == Skeleton.JointStatus.NotTracked)
			return false;
		Vector3 positionCoude;
		if (skeleton.GetJointPosition (Skeleton.Joint.ElbowLeft, out positionCoude) == Skeleton.JointStatus.NotTracked)
			return false;
		Vector3 positionMain;
		if (skeleton.GetJointPosition (Skeleton.Joint.HandLeft, out positionMain) == Skeleton.JointStatus.NotTracked)
			return false;

		// Mesurer la longueur du bras gauche.
		float longueurBras = Vector3.Distance(positionEpaule, positionCoude) +
			Vector3.Distance(positionCoude, positionMain);

		// On veut avoir une demi-longueur de bras vers la gauche.
		float differenceX = positionEpaule.x - positionMain.x;
		bool ok = differenceX > longueurBras / 2.5f;

		return ok;
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
	
	// Squelette.
	private Skeleton skeleton = new Skeleton(0);
	
}
