using UnityEngine;
using System.Collections;

public class MenuAssisteController : MonoBehaviour {

	public MenuAssisteController() {
		instance = this;
	}

	// Retourne l'unique instance de cette classe.
	public static MenuAssisteController ObtenirInstance() {
		return instance;
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

		// Activer le pointeur.
		pointeur.gameObject.SetActive (true);
	}

	void OnGUI () {
		
	}
	
	void OnDisable () {
		Pointeur.obtenirInstance ().gameObject.SetActive (false);
	}

	// Unique instance de cette classe.
	private static MenuAssisteController instance;
}
