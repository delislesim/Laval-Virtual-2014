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
			boutons[i].GetComponent<BoutonMusique>().Cacher();
		}

		// Desactiver le pointeur.
		Pointeur.obtenirInstance ().gameObject.SetActive (false);
	}

	public void SignalerBoutonDesactive() {
		if (!estEnTrainDeCacher)
			return;

		Debug.Log ("signaler bouton desactive");

		++numBoutonsDesactives;
		if (numBoutonsDesactives == boutons.Count) {
			gameObject.SetActive (false);
		}
	}

	// Assigne un texte a un bouton du menu.
	public void AssignerTexte(int indexBouton, string texteHaut, string texteBas) {
		boutons [indexBouton].GetComponent<BoutonMusique> ().AssignerTexte (texteHaut, texteBas);
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
		// Verifier s'il y a un choix d'instrument actif.
		Pointeur pointeur = Pointeur.obtenirInstance ();
		int targetid = pointeur.GetCurrentTargetId ();
		if (targetid != -1) {
			// Verifier quel element du menu a ete choisi.
		}
	}
	
	void OnEnable () {
		Pointeur pointeur = Pointeur.obtenirInstance ();

		// Mettre les bonnes cibles pour le pointeur.
		pointeur.RemoveAllTargets ();
		pointeur.AddTarget (0, new Vector2 (0, 0.1f), new Vector2 (0.25f, 0.5f));
		pointeur.AddTarget (1, new Vector2 (0.9f, -0.4f), new Vector2 (0.25f, 0.25f));
		pointeur.AddTarget (2, new Vector2 (1.8f, -0.4f), new Vector2 (0.25f, 0.25f));
		pointeur.AddTarget (3, new Vector2 (0.9f,  0.5f), new Vector2 (0.25f, 0.25f));
		pointeur.AddTarget (4, new Vector2 (1.8f,  0.5f), new Vector2 (0.25f, 0.25f));

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
