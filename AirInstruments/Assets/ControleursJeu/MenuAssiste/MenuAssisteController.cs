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

	public void Afficher() {
		numBoutonsDesactives = 0;
		estEnTrainDeCacher = false;
		gameObject.SetActive (true);
		for (int i = 0; i < boutons.Count; ++i) {
			boutons[i].GetComponent<BoutonMusique>().Afficher();
		}
	}

	public void Cacher() {
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

	// Assigne un texte a un bouton du menu.
	public void AssignerTexte(int indexBouton, string texteHaut, string texteBas) {
		boutons [indexBouton].GetComponent<BoutonMusique> ().AssignerTexte (texteHaut, texteBas);
	}

	// Desactiver un bouton.
	public void DesactiverBouton(int indexBouton) {
		boutons [indexBouton].GetComponent<BoutonMusique> ().DefinirDesactive (true);
	}

	// Obtient l'index du bouton presse. -1 si aucun bouton n'est presse.
	public int ObtenirBoutonPresse() {
		return Pointeur.obtenirInstance ().GetCurrentTargetId ();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnEnable () {
		Pointeur pointeur = Pointeur.obtenirInstance ();

		// Mettre les bonnes cibles pour le pointeur.
		pointeur.RemoveAllTargets ();
		if (!boutons[0].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (0, new Vector2 (0, 0.1f), new Vector2 (0.35f, 0.5f));

		if (!boutons[1].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (1, new Vector2 (1.35f, -0.4f), new Vector2 (0.5f, 0.35f));
		
		if (!boutons[2].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (2, new Vector2 (1.35f, -0.4f), new Vector2 (0.5f, 0.35f));

		if (!boutons[3].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (3, new Vector2 (0.9f,  0.5f), new Vector2 (0.35f, 0.35f));

		if (!boutons[4].GetComponent<BoutonMusique> ().EstDesactive())
			pointeur.AddTarget (4, new Vector2 (1.8f,  0.5f), new Vector2 (0.35f, 0.35f));

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
