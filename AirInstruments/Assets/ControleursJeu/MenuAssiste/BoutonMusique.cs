using UnityEngine;
using System.Collections;

public class BoutonMusique : MonoBehaviour {

	// Assigne un texte au bouton.
	public void AssignerTexte(string texteHaut, string texteBas) {
		transform.Find ("texteBoutonHaut").GetComponent<TextMesh> ().text = texteHaut;
		transform.Find ("texteBoutonBas").GetComponent<TextMesh> ().text = texteBas;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
