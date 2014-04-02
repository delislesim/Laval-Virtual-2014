using UnityEngine;
using System.Collections;

public class SpotCouleurDrumMaster : MonoBehaviour {

	// Liste des spotlights disponibles.
	public SpotCouleurDrum[] spots;

	// Systeme de particule de la fumee.
	public GameObject smoke;

	void Start() {
	}


	// Permet de fermer temporairement les effets de lumiere pendant le feu.
	public void SetFermePourFeu(bool fermePourFeu) {
		if (!allume)
			return;

		if (fermePourFeu && !this.fermePourFeu) {
			foreach (SpotCouleurDrum spot in spots) {
				spot.Fermer();
			}
			smoke.particleEmitter.emit = false;
		} else if (!fermePourFeu && this.fermePourFeu) {
			foreach (SpotCouleurDrum spot in spots) {
				spot.Allumer();
			}
			smoke.particleEmitter.emit = true;
		}
		this.fermePourFeu = fermePourFeu;
	}
	private bool fermePourFeu = false;


	public void Allumer() {
		fermePourFeu = false;

		allume = true;
		compteur = 0;

		foreach (SpotCouleurDrum spot in spots) {
			spot.Allumer();
		}
		smoke.SetActive (true);
		smoke.particleEmitter.emit = true;
	}

	public void Fermer() {
		allume = false;

		foreach (SpotCouleurDrum spot in spots) {
			spot.Fermer();
		}
		smoke.particleEmitter.emit = false;
	}

	void Update() {
		if (!allume)
			return;

		// Gerer les changements de couleur a l'unisson.
		compteur += Time.deltaTime;
		if (compteur >= kTempsChangementCouleur) {
			// Choix d'une nouvelle couleur.
			int indexCouleur = random.Next(0, colors.Length);
			Color color = colors[indexCouleur];

			foreach (SpotCouleurDrum spot in spots) {
				spot.ChangerCouleur(color);
			}

			compteur = 0;
		}
	}

	// Indique si les spots sont allumes.
	private bool allume = false;

	// Listes de couleurs possibles pour les spots.
	private Color[] colors = new Color[]{
		new Color32(149, 52, 234, 128),
		new Color32(66, 52, 234, 128),
		new Color32(52, 234, 128, 128),
		new Color32(234, 86, 52, 128)
	};

	// Generateur de nombres aleatoires.
	private System.Random random = new System.Random ();

	// Temps avant un changement de couleur.
	private const float kTempsChangementCouleur = 2.0f;

	// Compteur pour les changements couleur.
	private float compteur = 0;

}
