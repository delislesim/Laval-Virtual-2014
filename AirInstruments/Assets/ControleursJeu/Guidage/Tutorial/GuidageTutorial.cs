using UnityEngine;
using System.Collections;

public class GuidageTutorial : MonoBehaviour {

	// GameObject affichant le texte du guidage.
	public TextMesh text;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void AfficherEtape(EtapeTutorial etape) {
		text.text = etape.ObtenirTexte ();
		gameObject.SetActive (true);
	}

	public void Masquer() {
		gameObject.SetActive (false);
	}

	// Indique si le guidage est en train de faire une animation.
	public bool EstEnAnimation() {
		return false;
	}

	// Indique si le guidage est visible (meme s'il est en train de faire une animation).
	public bool EstVisible() {
		return gameObject.activeSelf;
	}
}
