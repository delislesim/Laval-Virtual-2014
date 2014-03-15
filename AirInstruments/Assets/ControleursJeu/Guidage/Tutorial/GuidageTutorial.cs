using UnityEngine;
using System.Collections;

public class GuidageTutorial : MonoBehaviour {

	// GameObject affichant le texte du guidage.
	public GUIText texte;

	public GUISkin tutorialSkin;

	// GameObject affichant le background du guidage.
	public GUITexture background;
	public Texture backgroundCompleted;
	public Texture backgroundBeginning;

	// Use this for initialization
	void Start () {
		if (initialized)
			return;

		tailleBackground = new Vector2 (background.pixelInset.width,
		                                background.pixelInset.height);
		tailleTexte = texte.fontSize;
		positionTexte = texte.pixelOffset;

		texte.font = tutorialSkin.font;

		initialized = true;
	}

	// Update is called once per frame
	void Update () {
		if(EstEnTrainEntrer) {
			if(positionGuidage.x < positionArrivee.x){
				positionGuidage.x += vitesseAnimation;
				DefinirPositionReelle();
			} else {
				EstEnTrainEntrer = false;
			}
		}

		if(EstEnTrainDeMasquer) {
			//tempsEcoule += Time.deltaTime;
			if(positionGuidage.x <= Screen.width + (tailleBackground.x/2)){
				background.texture = backgroundCompleted;
				positionGuidage.x += vitesseAnimation;
				DefinirPositionReelle();
			} else {
				//tempsEcoule = 0;
				EstEnTrainDeMasquer = false;
				gameObject.SetActive (false);
				positionGuidage.x = -tailleBackground.x/2;
			}
		}
	}

	public void DefinirPosition(Vector2 position) {
		Start ();
		positionArrivee = position;
		positionGuidage.y = position.y;
	}

	private void DefinirPositionReelle() {
		Vector2 positionTransformee = TransformerCoordonnees (positionGuidage);
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
		background.texture = backgroundBeginning;
		gameObject.SetActive (true);
		EstEnTrainEntrer = true;

		// Jouer le son.
		audio.clip = etape.ObtenirAudio ();
		audio.Play ();
	}

	public void Masquer() {
		EstEnTrainDeMasquer = true;
		//gameObject.SetActive (false);
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

	// Position actuelle du tutorial
	private Vector2 positionGuidage;

	// Position cible du tutorial
	private Vector2 positionArrivee;

	// Indique si l'animation pour masquer l'objet est en cours
	private bool EstEnTrainDeMasquer = false;

	// Indique si l'animation pour afficher l'objet est en cours
	private bool EstEnTrainEntrer = false;

	// Temps de l'animation
	private float tempsAnimationMasquer = 1f;

	// Vitesse de l'animation
	private float vitesseAnimation = 15;

	// Temps écoulé depuis le début de l'animation
	private float tempsEcoule = 0;
}
