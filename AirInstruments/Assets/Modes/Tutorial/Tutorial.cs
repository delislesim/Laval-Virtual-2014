using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {

	// Affichage du guidage du tutorial.
	public GuidageTutorial guidageTutorial;

	// Constructeur. Garde l'unique instance de cette classe dans
	// une variable statique.
	public Tutorial() {
		instance = this;
	}

	// Retourne l'unique instance de cette classe.
	public static Tutorial ObtenirInstance() {
		return instance;
	}

	// Efface toutes les etapes du tutorial.
	public void ReinitialiserEtapes() {
		etapes.Clear ();
	}

	// Ajoute une etape au tutorial.
	public void AjouterEtape(EtapeTutorial etape) {
		etapes.Add (etape);
	}

	// Demarre le tutorial.
	// @param positionGuidage : position a laquelle le guidage doit s'afficher.
	// @param tailleGuidage : taille d'affichage du guidage.
	public void Demarrer(Vector3 positionGuidage, Vector3 tailleGuidage) {
		// Positionner le guidage.
		guidageTutorial.transform.position = positionGuidage;
		guidageTutorial.transform.localScale = tailleGuidage;

		// Remettre a 0 le compteur indiquant a quelle etape on est rendu.
		indexEtape = 0;

		// Le tutorial n'a pas encore été complété.
		estComplete = false;

		// Activer le tutorial.
		gameObject.SetActive (true);
	}

	// Indique si le tutorial a été complété.
	public bool EstComplete() {
		return estComplete;
	}

	private void AccederProchaineEtape() {
		guidageTutorial.AfficherEtape (etapes[indexEtape]);
		etapes [indexEtape].Demarrer ();
		++indexEtape;
	}

	// Update is called once per frame
	void Update () {
		// Affichage de la premiere etape.
		if (indexEtape == 0) {
			AccederProchaineEtape();
		} else if (guidageTutorial.EstVisible() &&
				   !guidageTutorial.EstEnAnimation() &&
		           etapes[indexEtape - 1].EstCompletee()) {
			// L'etape est completee: Faire l'animation de sortie
			// du guidage courant.
			guidageTutorial.Masquer();
		} else if (!guidageTutorial.EstVisible()) {
			// Il n'y a aucune étape de visible.
			if (indexEtape < etapes.Count) {
				// S'il reste des étapes, on affiche la suivante.
				AccederProchaineEtape();
			} else {
				// S'il ne reste pas d'étapes, on se désactive et on indique
				// que le tutorial est complété.
				gameObject.SetActive (false);
			}
		}
	}

	// Unique instance du tutorial, pour le singleton.
	private static Tutorial instance;

	// Etapes du tutorial.
	private List<EtapeTutorial> etapes = new List<EtapeTutorial>();

	// Numero de la prochaine etape que le joueur doit realiser.
	private int indexEtape = 0;

	// Indique si le tutorial a été complété.
	private bool estComplete = false;
}
