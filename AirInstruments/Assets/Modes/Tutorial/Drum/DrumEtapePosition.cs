using UnityEngine;
using System.Collections;

public class DrumEtapePosition : EtapeTutorial {
	
	public DrumEtapePosition(AudioClip son,
	                         GameObject tipLeft,
	                         GameObject tipRight) {
		this.son = son;
		this.tipLeft = tipLeft;
		this.tipRight = tipRight;
	}
	
	// Retourne le texte d'instruction qui doit etre affiche lors de
	// l'execution de cette etape du tutorial.
	public string ObtenirTexte() {
		if(!Langue.isEnglish)
			return "Positionnez le bout de vos baguettes au-dessus de la batterie.";
		else
			return "Position the tip of your drumsticks over the drum set.";
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
		// Ne rien faire.
	}
	
	// Indique si l'etape a ete completee avec success par le joueur.
	public bool EstCompletee() {
		return RespecteLimites (tipLeft) &&
			   RespecteLimites (tipRight);
	}

	private bool RespecteLimites(GameObject tip) {
		return tip.transform.position.z > kCibleZ &&
			   tip.transform.position.x > kLimiteMinX &&
			   tip.transform.position.x < kLimiteMaxX;
	}
	
	// Indique si on doit feliciter le joueur (vrai) ou simplement
	// passer a l'etape suivante (faux).
	public bool DoitFeliciter() {
		return true;
	}
	
	// Voix lisant l'instruction.
	private AudioClip son;
	
	// Tip left.
	private GameObject tipLeft;

	// Tip right.
	private GameObject tipRight;

	// Cible en z.
	private const float kCibleZ = -7.4f;

	// Limites en x.
	private const float kLimiteMinX = -2.0f;
	private const float kLimiteMaxX = 2.2f;
	
}
