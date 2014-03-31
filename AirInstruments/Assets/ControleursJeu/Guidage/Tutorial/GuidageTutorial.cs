using UnityEngine;
using System.Collections;

public class GuidageTutorial : MonoBehaviour {

	// GameObject affichant le texte du guidage.
	public GUIText texte;
	public GUISkin tutorialSkin;

	// GameObject affichant le pictogramme.
	public GUITexture pictogramme;
	private Texture[] texturePictogramme;

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

		if (texturePictogramme == null) {
				pictogramme.texture = null;
		} else {
				// Timer pour l'animation des pictogrammes
				tempsPictogramme += Time.deltaTime;
				int indexAnimation = (int)(tempsPictogramme * (texturePictogramme.Length / 0.5f));
				if (indexAnimation >= texturePictogramme.Length) {
					tempsPictogramme = 0;
					indexAnimation = 0;
				}
				pictogramme.texture = texturePictogramme [indexAnimation];
		}

		if(EstEnTrainEntrer) {
			if (proportion > 1.0f) {
				proportion = 1.0f;
				EstEnTrainEntrer = false;
			}
			if (positionCible == Position.HAUT) {
				Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
				kPositionCacherHaut = Screen.height;
				kPositionHaut = Screen.height - tailleTransformee.y;


				float positionContenu = spring(kPositionCacherHaut, kPositionHaut, proportion);
				DefinirPositionReelle(positionContenu);
				float positionBackground = easeOutSine(kPositionCacherHaut, kPositionHaut, proportion);
				DefinirPositionReelleFond(positionBackground);
			} else {
				Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
				kPositionCacherBas = -tailleTransformee.y;
				kPositionBas = 0;

				float positionContenu = spring(kPositionCacherBas, kPositionBas, proportion);
				DefinirPositionReelle(positionContenu);
				float positionBackground = easeOutSine(kPositionCacherBas, kPositionBas, proportion);
				DefinirPositionReelleFond(positionBackground);
			}
		} else if(EstEnTrainDeMasquer) {
			if (proportion > 1.0f) {
				proportion = 1.0f;
				EstEnTrainDeMasquer = false;
				gameObject.SetActive (false);
			}

			if (positionCible == Position.HAUT) {
				Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
				kPositionCacherHaut = Screen.height;
				kPositionHaut = Screen.height - tailleTransformee.y;

				float positionContenu = spring(kPositionHaut, kPositionCacherHaut, proportion);
				DefinirPositionReelle(positionContenu);
				float positionBackground = easeOutSine(kPositionHaut, kPositionCacherHaut, proportion);
				DefinirPositionReelleFond(positionBackground);
			} else {
				Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
				kPositionCacherBas = -tailleTransformee.y;
				kPositionBas = 0;

				float positionContenu = spring(kPositionBas, kPositionCacherBas, proportion);
				DefinirPositionReelle(positionContenu);
				float positionBackground = easeOutSine(kPositionBas, kPositionCacherBas, proportion);
				DefinirPositionReelleFond(positionBackground);
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

	// Positionnement du background.
	private void DefinirPositionReelleFond(float positionRelle) {
		Vector2 positionTransformee = TransformerCoordonneesPosition (new Vector2(0, positionRelle));
		Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
		positionTransformee.y = positionRelle;

		Rect pixelInset = background.pixelInset;
		pixelInset.x = positionTransformee.x;
		pixelInset.y = positionTransformee.y;
		pixelInset.width = tailleTransformee.x;
		pixelInset.height = tailleTransformee.y;
		background.pixelInset = pixelInset;
	}

	// Positionnement du pictogramme et du texte.
	private void DefinirPositionReelle(float positionRelle) {
		Vector2 positionTransformee = TransformerCoordonneesPosition (new Vector2(0, positionRelle));
		Vector2 tailleTransformee = TransformerCoordonneesTaille (tailleBackground);
		positionTransformee.y = positionRelle;
		
		Vector2 positionTexteTransformee = positionTransformee + TransformerCoordonneesPosition (positionTexte);
		texte.pixelOffset = positionTexteTransformee;
		
		int tailleTexteTransformee = (int) (tailleTexte * ((float)Screen.width) / 1755f);
		texte.fontSize = tailleTexteTransformee;

		Rect pixelInsetPictogramme = pictogramme.pixelInset;

		Vector2 positionPictogrammeTransformee = positionTransformee + TransformerCoordonneesPosition (positionPictogramme);
		pixelInsetPictogramme.x = positionPictogrammeTransformee.x;
		pixelInsetPictogramme.y = positionPictogrammeTransformee.y;

		Vector2 taillePictogrammeTransformee = TransformerCoordonneesTaille (taillePictogramme);
		pixelInsetPictogramme.width = taillePictogrammeTransformee.x;
		pixelInsetPictogramme.height = taillePictogrammeTransformee.y;

		pictogramme.pixelInset = pixelInsetPictogramme;
	}

	private Vector2 TransformerCoordonneesTaille(Vector2 coord) {
		return coord * ((float)Screen.width) / 1755f;
	}

	private Vector2 TransformerCoordonneesPosition(Vector2 coord) {
		return new Vector2 (coord.x * ((float)Screen.width) / 1755f,
		                    coord.y * ((float)Screen.height) / 987.1875f);
	}

	public void AfficherEtape(EtapeTutorial etape) {
		tempsPictogramme = 0;
		texte.text = etape.ObtenirTexte ();
		texturePictogramme = etape.ObtenirAnimation ();
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

	private float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
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
	private const float kTempsAnimation = 0.5f;

	// Temps écoulé depuis le début de l'animation
	private float tempsEcoule = 0;

	private float tempsPictogramme = 0;

	// Position cible du guidage.
	private Position positionCible;

	// Position du guidage en haut de l'écran.
	private float kPositionHaut = 689.94f;

	// Position pour se cacher en haut de l'écran.
	private float kPositionCacherHaut = 1000.0f;

	// Position du guidage en bas de l'écran.
	private float kPositionBas = 0;

	// Position pour se cacher en bas de l'écran.
	private float kPositionCacherBas = -300.0f;

	// Indique si on doit feliciter le jouer.
	private bool doitFeliciter;
}
