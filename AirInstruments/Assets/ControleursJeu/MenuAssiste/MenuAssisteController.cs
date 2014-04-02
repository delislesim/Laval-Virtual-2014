using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuAssisteController : MonoBehaviour {

	// Boutons.
	public List<GameObject> boutons;

	public MenuAssisteController() {
		instance = this;
	}

	// Retourne l'unique instance de cette classe.
	public static MenuAssisteController ObtenirInstance() {
		return instance;
	}

	public bool EstVisible() {
		return gameObject.activeSelf;
	}

	public void Afficher() {
		GestureRecognition.ObtenirInstance().RemoveGesture(GestureId.GESTURE_MENU);
		numBoutonsDesactives = 0;
		estEnTrainDeCacher = false;
		gameObject.SetActive (true);
		for (int i = 0; i < boutons.Count; ++i) {
			boutons[i].GetComponent<BoutonMusique>().Afficher();
		}
	}

	public void Cacher() {
		GestureRecognition.ObtenirInstance ().AddGesture (new GestureMenu ());
		numBoutonsDesactives = 0;
		estEnTrainDeCacher = true;
		for (int i = 0; i < boutons.Count; ++i) {
			if (!boutons[i].GetComponent<BoutonMusique>().EstDesactive()) {
				boutons[i].GetComponent<BoutonMusique>().Cacher();
			} else {
				SignalerBoutonDesactive();
			}
		}

		// Desactiver le pointeur.
		Pointeur.obtenirInstance ().gameObject.SetActive (false);
	}

	public void SignalerBoutonDesactive() {
		if (!estEnTrainDeCacher)
			return;

		++numBoutonsDesactives;
		if (numBoutonsDesactives == boutons.Count) {
			gameObject.SetActive (false);
		}
	}

	public void DesactiverTousBoutons() {
		for (int i = 0; i < boutons.Count; ++i) {
			DesactiverBouton(i);
		}
	}

	// Assigne un texte a un bouton du menu.
	public void AssignerTexte(int indexBouton, string texteHaut, string texteBas) {
		boutons [indexBouton].GetComponent<BoutonMusique> ().AssignerTexte (texteHaut, texteBas);
	}

	// Desactiver un bouton.
	private void DesactiverBouton(int indexBouton) {
		boutons [indexBouton].GetComponent<BoutonMusique> ().DefinirDesactive (true);
	}

	// Obtient l'index du bouton presse. -1 si aucun bouton n'est presse.
	public int ObtenirBoutonPresse() {
		return Pointeur.obtenirInstance ().GetCurrentTargetId ();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnEnable () {
		Pointeur pointeur = Pointeur.obtenirInstance ();

		float largeur = 0.24f / 2.0f;
		float hauteur = 0.38f / 2.0f;

		float yHaut = 0.48f;
		float yBas = 0.08f;

		float xGauche = 0.325f;
		float xCentre = 0.565f;
		float xDroite = 0.79f;

		// Mettre les bonnes cibles pour le pointeur.
		pointeur.RemoveAllTargets ();
		if (!boutons[0].GetComponent<BoutonMusique> ().EstDesactive()) {
			pointeur.AddTarget (0, new Vector2 (xGauche, (yHaut + yBas) / 2.0f), new Vector2 (largeur, hauteur * 2.0f));
			pointeur.SetBoutonRetourPresent (true);
		} else {
			pointeur.SetBoutonRetourPresent (false);
		}

		if (!boutons[1].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (1, new Vector2 ((xCentre + xDroite) / 2.0f, yHaut), new Vector2 (largeur * 2.0f, hauteur));
		
		if (!boutons[2].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (2, new Vector2 ((xCentre + xDroite) / 2.0f, yBas), new Vector2 (largeur * 2.0f, hauteur));

		if (!boutons[3].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (3, new Vector2 (xGauche, yBas), new Vector2 (largeur, hauteur));

		if (!boutons[4].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (4, new Vector2 (xDroite, yBas), new Vector2 (largeur, hauteur));

		if (!boutons[5].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (5, new Vector2 (xGauche, yHaut), new Vector2 (largeur, hauteur));
		
		if (!boutons[6].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (6, new Vector2 (xDroite, yHaut), new Vector2 (largeur, hauteur));

		// Activer le pointeur.
		pointeur.gameObject.SetActive (true);
	}
	
	void OnDisable () {
	}

	// Indique si on est en train de se cacher.
	private bool estEnTrainDeCacher = false;

	// Nombre de boutons qui se sont desactives correctement.
	private int numBoutonsDesactives = 0;

	// Unique instance de cette classe.
	private static MenuAssisteController instance;
}
