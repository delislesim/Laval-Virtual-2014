using UnityEngine;
using System.Collections;

public class GuidageTutorial : MonoBehaviour {

	// GameObject affichant le texte du guidage.
	public GUIText texte;

	// GameObject affichant le background du guidage.
	public GUITexture background;

	// Use this for initialization
	void Start () {
		if (initialized)
			return;

		tailleBackground = new Vector2 (background.pixelInset.width,
		                                background.pixelInset.height);
		tailleTexte = texte.fontSize;
		positionTexte = texte.pixelOffset;

		initialized = true;
	}

	// Update is called once per frame
	void Update () {
	}

	public void DefinirPosition(Vector2 position) {
		Start ();

		Vector2 positionTransformee = TransformerCoordonnees (position);
		Vector2 tailleTransformee = TransformerCoordonnees (tailleBackground);
		Rect pixelInset = background.pixelInset;
		pixelInset.x = positionTransformee.x;
		pixelInset.y = positionTransformee.y;
		pixelInset.width = tailleTransformee.x;
		pixelInset.height = tailleTransformee.y;
		background.pixelInset = pixelInset;

		Vector2 positionTexteTransformee = positionTransformee + TransformerCoordonnees (positionTexte);
		texte.pixelOffset = positionTexteTransformee;

		int tailleTexteTransformee = (int) (tailleTexte * ((float)Screen.width) / 1755f);
		texte.fontSize = tailleTexteTransformee;
	}

	private Vector2 TransformerCoordonnees(Vector2 coord) {
		return coord * ((float)Screen.width) / 1755f;
	}

	public void AfficherEtape(EtapeTutorial etape) {
		texte.text = etape.ObtenirTexte ();
		gameObject.SetActive (true);

		// Jouer le son.
		audio.clip = etape.ObtenirAudio ();
		audio.Play ();
	}

	public void Masquer() {
		gameObject.SetActive (false);
	}

	// Indique si le guidage est en train de faire une animation.
	public bool EstEnAnimation() {
		return audio.isPlaying;
	}

	// Indique si le guidage est visible (meme s'il est en train de faire une animation).
	public bool EstVisible() {
		return gameObject.activeSelf;
	}

	// Indique si le guidage a ete demarre une premiere fois.
	private bool initialized = false;

	// Taille du background pour une résolution de 1080 * 768.
	private Vector2 tailleBackground;

	// Taille du texte pour une résolution de 1080 * 768.
	private int tailleTexte;

	// Position relative du texte pour une résolution de 1080 * 768.
	private Vector2 positionTexte;
}
