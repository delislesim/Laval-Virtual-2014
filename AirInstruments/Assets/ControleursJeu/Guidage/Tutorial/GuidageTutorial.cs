using UnityEngine;
using System.Collections;

public class GuidageTutorial : MonoBehaviour {

	// GameObject affichant le texte du guidage.
	public GUIText texte;
	public GUISkin tutorialSkin;

	// GameObject affichant le pictogramme.
	public GUITexture pictogramme;

	// GameObject affichant le background du guidage.
	public GUITexture background;

	// Images de fond.
	public Texture backgroundCompletedTop;
	public Texture backgroundCompletedBottom;
	public Texture backgroundBeginningTop;
	public Texture backgroundBeginningBottom;

	// Use this for initialization
	void Start () {
		if (initialized)
			return;

		tailleBackground = new Vector2 (background.pixelInset.width,
		                                background.pixelInset.height);
		tailleTexte = texte.fontSize;
		texte.font = tutorialSkin.font;
		positionTexte = texte.pixelOffset;

		taillePictogramme = new Vector2 (pictogramme.pixelInset.width,
		                                 pictogramme.pixelInset.height);
		positionPictogramme = new Vector2 (pictogramme.pixelInset.x,
		                                   pictogramme.pixelInset.y);

		initialized = true;
	}

	// Update is called once per frame
	void Update () {
		tempsEcoule += Time.deltaTime;
		float proportion = tempsEcoule / kTempsAnimation;

		if(EstEnTrainEntrer) {
			if (proportion > 1.0f) {
				proportion = 1.0f;
				EstEnTrainEntrer = false;
			}
			if (positionCible == Position.HAUT) {
				float position = spring(kPositionCacherHaut, kPositionHaut, proportion);
				DefinirPositionReelle(position);
			} else {
				float position = spring(kPositionCacherBas, kPositionBas, proportion);
				DefinirPositionReelle(position);
			}
		} else if(EstEnTrainDeMasquer) {
			if (proportion > 1.0f) {
				proportion = 1.0f;
				EstEnTrainDeMasquer = false;
				gameObject.SetActive (false);
			}

			if (positionCible == Position.HAUT) {
				float position = spring(kPositionHaut, kPositionCacherHaut, proportion);
				DefinirPositionReelle(position);
			} else {
				float position = spring(kPositionBas, kPositionCacherBas, proportion);
				DefinirPositionReelle(position);
			}
		}
	}

	public enum Position {
		HAUT, 
		BAS
	};

	public void DefinirPosition(Position positionCible) {
		Start ();
		this.positionCible = positionCible;
	}

	private void DefinirPositionReelle(float positionRelle) {
		Vector2 positionTransformee = TransformerCoordonnees (new Vector2(0, positionRelle));
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

		Rect pixelInsetPictogramme = pictogramme.pixelInset;

		Vector2 positionPictogrammeTransformee = positionTransformee + TransformerCoordonnees (positionPictogramme);
		pixelInsetPictogramme.x = positionPictogrammeTransformee.x;
		pixelInsetPictogramme.y = positionPictogrammeTransformee.y;

		Vector2 taillePictogrammeTransformee = TransformerCoordonnees (taillePictogramme);
		pixelInsetPictogramme.width = taillePictogrammeTransformee.x;
		pixelInsetPictogramme.height = taillePictogrammeTransformee.y;

		pictogramme.pixelInset = pixelInsetPictogramme;
	}

	private Vector2 TransformerCoordonnees(Vector2 coord) {
		return coord * ((float)Screen.width) / 1755f;
	}

	public void AfficherEtape(EtapeTutorial etape) {

		texte.text = etape.ObtenirTexte ();
		if(positionCible == Position.HAUT)
			background.texture = backgroundBeginningTop;
		else
			background.texture = backgroundBeginningBottom;
		doitFeliciter = etape.DoitFeliciter ();
		gameObject.SetActive (true);
		EstEnTrainEntrer = true;
		tempsEcoule = 0;

		// Jouer le son.
		audio.clip = etape.ObtenirAudio ();
		audio.Play ();

		if (positionCible == Position.HAUT) {
			DefinirPositionReelle(kPositionCacherHaut);
		} else {
			DefinirPositionReelle(kPositionCacherBas);
		}
	}

	public void Masquer() {
		EstEnTrainDeMasquer = true;

		if (doitFeliciter) {
			if(positionCible == Position.HAUT)
				background.texture = backgroundCompletedTop;
			else
				background.texture = backgroundCompletedBottom;
		}
		tempsEcoule = 0;
	}

	// Indique si le guidage est en train de faire une animation.
	public bool EstEnAnimation() {
		return audio.isPlaying || EstEnTrainDeMasquer || EstEnTrainEntrer;
	}

	// Indique si le guidage est visible (meme s'il est en train de faire une animation).
	public bool EstVisible() {
		return gameObject.activeSelf;
	}

	private float spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	// Indique si le guidage a ete demarre une premiere fois.
	private bool initialized = false;

	// Taille du background pour une résolution de 1080 * 768.
	private Vector2 tailleBackground;

	// Taille du texte pour une résolution de 1080 * 768.
	private int tailleTexte;

	// Position relative du texte pour une résolution de 1080 * 768.
	private Vector2 positionTexte;

	// Taille du pictogramme pour une résolution de 1080 * 768.
	private Vector2 taillePictogramme;

	// Position relative du pictogramme pour une résolution de 1080 * 768.
	private Vector2 positionPictogramme;

	// Indique si l'animation pour masquer l'objet est en cours
	private bool EstEnTrainDeMasquer = false;

	// Indique si l'animation pour afficher l'objet est en cours
	private bool EstEnTrainEntrer = false;

	// Duree d'une animation.
	private const float kTempsAnimation = 1.0f;

	// Temps écoulé depuis le début de l'animation
	private float tempsEcoule = 0;

	// Position cible du guidage.
	private Position positionCible;

	// Position du guidage en haut de l'écran.
	private const float kPositionHaut = 689.94f;

	// Position pour se cacher en haut de l'écran.
	private const float kPositionCacherHaut = 1000.0f;

	// Position du guidage en bas de l'écran.
	private const float kPositionBas = 0;

	// Position pour se cacher en bas de l'écran.
	private const float kPositionCacherBas = -300.0f;

	// Indique si on doit feliciter le jouer.
	private bool doitFeliciter;
}
