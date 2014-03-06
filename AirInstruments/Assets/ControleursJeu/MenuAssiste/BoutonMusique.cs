using UnityEngine;
using System.Collections;

public class BoutonMusique : MonoBehaviour {

	// Assigne un texte au bouton.
	public void AssignerTexte(string texteHaut, string texteBas) {
		transform.Find ("texteBoutonHaut").GetComponent<TextMesh> ().text = texteHaut;
		transform.Find ("texteBoutonBas").GetComponent<TextMesh> ().text = texteBas;
		DefinirDesactive (false);
	}

	// Activer / desactiver le bouton. Un bouton desactive ne
	// s'affiche pas dans le menu courant.
	public void DefinirDesactive(bool estDesactive) {
		this.estDesactive = estDesactive;
	}

	// Indique si le bouton est desactivé.
	public bool EstDesactive() {
		return estDesactive;
	}

	// Afficher le bouton avec une animation.
	public void Afficher() {
		if (EstDesactive ())
			return;

		tempsAfficher = (float)random.NextDouble () * kTempsCommencerAnimationmax;
		tempsCacher = kTempsInvalide;
		timer = 0;
	}

	// Masquer le bouton avec une animation.
	public void Cacher() {
		tempsAfficher = kTempsInvalide;
		tempsCacher = (float)random.NextDouble () * kTempsCommencerAnimationmax;
		timer = 0;
	}

	// Use this for initialization
	void Start () {
		positionDefaut = transform.localPosition;
		positionCachee = positionDefaut;
		positionCachee.y += kDistanceCacher;

		// Se cacher.
		transform.localPosition = positionCachee;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (tempsAfficher != kTempsInvalide && timer > tempsAfficher) {
			Vector3 positionDefautWorld = transform.parent.TransformPoint (positionDefaut);
			iTweenEvent.GetEvent (this.gameObject, "animation").OverridePosition (positionDefautWorld);
			iTweenEvent.GetEvent (this.gameObject, "animation").Play();
			tempsAfficher = kTempsInvalide;
		} else if (tempsCacher != kTempsInvalide && timer > tempsCacher) {
			Vector3 positionCacheeWorld = transform.parent.TransformPoint (positionCachee);
			iTweenEvent.GetEvent (this.gameObject, "animation").OverridePosition (positionCacheeWorld);
			iTweenEvent.GetEvent (this.gameObject, "animation").Play();
			tempsCacher = kTempsInvalide;
		}
	}

	void OnDisable () {
		transform.localPosition = positionCachee;
	}

	// Indique si on s'est rappele de nos parametres une premiere fois.
	private bool parametresEnregistres = false;

	// Position par defaut en coordonnées locales.
	private Vector3 positionDefaut;

	// Position cachée en coordonnées locales.
	private Vector3 positionCachee;

	// Timer.
	private float timer = 0;

	// Temps au bout duquel on va s'afficher.
	private float tempsAfficher = kTempsInvalide;

	// Temps au bout duquel on va se cacher.
	private float tempsCacher = kTempsInvalide;

	// Indique si le bouton est désactivé, c'est a dire qu'il ne
	// s'affiche pas dans le menu courant.
	private bool estDesactive = false;

	// Distance a parcourir en z pour se cacher, en coordonnées locales.
	private const float kDistanceCacher = 18.0f;

	// Temps invalide.
	private const float kTempsInvalide = -1.0f;

	// Temps maximum pour commencer les animations.
	private const float kTempsCommencerAnimationmax = 0.5f;

	// Générateur de nombres aléatoires.
	private System.Random random = new System.Random();
}
