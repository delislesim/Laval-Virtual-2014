﻿using UnityEngine;
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

		// Mettre les bonnes cibles pour le pointeur.
		pointeur.RemoveAllTargets ();
		if (!boutons[0].GetComponent<BoutonMusique> ().EstDesactive()) {
			pointeur.AddTarget (0, new Vector2 (19.0f/68.0f, 20.0f/38.0f), new Vector2 (7.0f/68.0f, 14.0f/38.0f));
			pointeur.SetBoutonRetourPresent (true);
		} else {
			pointeur.SetBoutonRetourPresent (false);
		}

		if (!boutons[1].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (1, new Vector2 (44.0f/68.0f, 0.13f), new Vector2 (14.0f/68.0f, 7.0f/38.0f));
		
		if (!boutons[2].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (2, new Vector2 (44.0f/68.0f, 0.55f), new Vector2 (14.0f/68.0f, 7.0f/38.0f));

		if (!boutons[3].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (3, new Vector2 (37.0f/68.0f, 0.55f), new Vector2 (7.0f/68.0f, 7.0f/38.0f));

		if (!boutons[4].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (4, new Vector2 (51.0f/68.0f, 0.55f), new Vector2 (7.0f/68.0f, 7.0f/38.0f));

		if (!boutons[5].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (5, new Vector2 (37.0f/68.0f, 0.13f), new Vector2 (7.0f/68.0f, 7.0f/38.0f));
		
		if (!boutons[6].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (6, new Vector2 (51.0f/68.0f, 0.13f), new Vector2 (7.0f/68.0f, 7.0f/38.0f));

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
