using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour, ComponentInterface {

	// Plan situe sur la surface du composant a jouer.
	public GameObject plane;

	// Spotlight qui eclaire ce composant.
	public DrumSpot spot;

	// Materiel pour indiquer que le composant doit etre joue.
	public Material materielDoitEtreJoue;

	// GameObject qui doit changer de couleur pour indiquer que
	// ce drum component doit etre joue.
	public GameObject gameObjectVisible;

	// Indique si c'est une cymbale.
	public bool estCymbale = false;

	public void PlaySoundWhenAssisted()
	{
		audio.Play();
		//Debug.Log ("Play when assisted called");

		TremblerCymbale ();

		if (spot != null) {
			spot.Play ();
		}
		aEteJoue = true;

		if (gameObjectVisible.renderer != null)
			gameObjectVisible.renderer.material = materialDefaut;
	}

	public void PlaySound()
	{
		if (!DrumAssistedController.EstActive())
			audio.Play();

		TremblerCymbale ();

		if (spot != null) {
			spot.Play ();
		}
		aEteJoue = true;

		if (gameObjectVisible.renderer != null)
			gameObjectVisible.renderer.material = materialDefaut;
	}

	public float DistanceToPoint (Vector3 point) {
		return planeMath.GetDistanceToPoint (point);
	}

	public void DefinirLuminosite(float luminosite) {
		spot.DefinirLuminosite (luminosite);
	}

	// Use this for initialization
	void Start () {
		// Calculer le plan situe sur la surface a jouer.
		if (plane != null) {
			Vector3 normal = plane.transform.rotation * Vector3.up;
			planeMath = new Plane (normal.normalized, plane.transform.position);
		}

		// Sauvegarder le materiel par defaut.
		if (gameObjectVisible.renderer != null)
			materialDefaut = gameObjectVisible.renderer.material;

		// Sauvegarder les parametres initiaux.
		positionInitiale = transform.position;
		if (gameObjectVisible) {
			positionInitialeVisible = gameObjectVisible.transform.position;
			rotationInitialeVisible = gameObjectVisible.transform.rotation;
		}
	}

	void Update () {
		if (estTremblement) {
			compteurAnimation += Time.deltaTime;

			if (compteurAnimation >= kTempsAnimation) {
				compteurAnimation = 0;
				amplitudeProchainTremblement -= 1.0f;
				if (amplitudeProchainTremblement <= 0) {
					estTremblement = false;
					AppliquerAngleVisible(0);
					return;
				}
			}

			if (compteurAnimation < kTempsAnimation / 2.0f) {
				float proportion = compteurAnimation / (kTempsAnimation / 2.0f);
				float angle = spring(0, amplitudeProchainTremblement, proportion);
				AppliquerAngleVisible(angle);
			} else if (compteurAnimation < kTempsAnimation) {
				float proportion = (compteurAnimation  / (kTempsAnimation / 2.0f)) - 1.0f;
				float angle = spring(amplitudeProchainTremblement, 0f, proportion);
				AppliquerAngleVisible(angle);
			}
		}
	}

	private void AppliquerAngleVisible(float angle) {
		gameObjectVisible.transform.position = positionInitialeVisible;
		gameObjectVisible.transform.rotation = rotationInitialeVisible;
		gameObjectVisible.transform.RotateAround (positionInitiale, Vector3.left, angle);
	}

	private void TremblerCymbale() {
		if (!estCymbale)
			return;

		// Animation du tutorial.
		if (transform.position != positionInitiale)
			return;

		// Amplitude du prochain tremblement.
		amplitudeProchainTremblement = 6.0f;

		// Deja une animation en cours.
		if (estTremblement)
			return;

		// OK, on peut demarrer l'animation.
		estTremblement = true;
		compteurAnimation = 0;
	}

	private float spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public void AjouterCoupAuTemps()
	{
		nbCoupsDernierTemps ++;

		TremblerCymbale ();
		if (spot != null) {
			spot.Play ();
		}
	}
	public int GetCoupsDernierTemps()
	{
		return nbCoupsDernierTemps;
	}
	public void ResetCoupsDernierTemps()
	{
		nbCoupsDernierTemps = 0;
	}

	void OnMouseDown()
	{
		PlaySound();
	}

	// Indique que le composant doit etre joue.
	public void DoitEtreJoue(bool materielBleu) {
		aEteJoue = false;
		if (materielBleu && gameObjectVisible.renderer != null) {
			gameObjectVisible.renderer.material = materielDoitEtreJoue;
		}
	}
	
	// Indique si le composant a ete joue depuis la derniere 
	// fois qu'on a demande qu'il soit joue.
	public bool AEteJoue() {
		return aEteJoue;
	}

	private bool aEteJoue = false;

	// Representation mathematique du plan situe sur sur la surface
	// du composant a jouer.
	private Plane planeMath;

	// Materiel par defaut.
	private Material materialDefaut;

	// Nombre de coups joues au cours du dernier temps.
	private int nbCoupsDernierTemps;

	// Position initiale de ce game object.
	private Vector3 positionInitiale;

	// Position initiale de la partie visible.
	private Vector3 positionInitialeVisible;

	// Rotation initiale de la partie visible.
	private Quaternion rotationInitialeVisible;

	// Temps depuis le debut du tremblement.
	private float compteurAnimation = 0;

	// Indique qu'on est en train de trembler.
	private bool estTremblement = false;

	// Amplitude du prochain tremblement.
	private float amplitudeProchainTremblement = 0;

	// Temps d'une animation.
	private const float kTempsAnimation = 0.15f;
}