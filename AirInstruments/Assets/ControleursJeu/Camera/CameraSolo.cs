using UnityEngine;
using System.Collections;

// Controle la caméra pour le solo de guitare.
public class CameraSolo : MonoBehaviour {

	// Caméra principale du jeu.
	public Camera mainCamera;

	// Hanches du joueur de guitare.
	public Transform hipCenter;

	public static CameraSolo ObtenirInstance() {
		return instance;
	}

	void Start () {
		instance = this;
	}

	void Reinitialiser() {
		initialise = false;
		timerCible = 0;
		timerPause = 0;
	}

	// Update is called once per frame
	void Update () {
		// Determiner s'il est approprie de faire l'animation de camera.
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance ();
		bool peutAnimer = !menuAssiste.EstVisible () &&
				          !GuitareController.EstEnTrainDeQuitter ();
		if (!peutAnimer) {
			Reinitialiser();
			return;
		}

		bool estSolo = AssistedModeControllerGuitar.EstSolo ();

		// Fin d'animation.
		if (!estSolo && cible == kPositionBaseCamera && timerCible >= kTempsAnimation) {
			ReinitialiserCamera();
			Reinitialiser();
			return;
		}
		if (!initialise && !estSolo)
			return;

		// Sauvegarde des parametres de base de la camera.
		if (!initialise) {
			kPositionBaseCamera = mainCamera.transform.position;
			kRotationBaseCamera = mainCamera.transform.rotation;
			initialise = true;
			ChoisirNouvelleCibleAleatoire();
		}

		// Choix d'une nouvelle cible.
		if (timerCible >= kTempsAnimation) {
			if (estSolo && hipCenter.position != MoveJointsForGuitar.HIDING_POS) {
				// Rester quelques temps a la position cible.
				timerPause += Time.deltaTime;

				if (timerPause >=  kTempsImmobile) {
					// Dans un solo, on choisit des cibles aleatoires.
					ChoisirNouvelleCibleAleatoire();
				}
			} else {
				// Hors d'un solo, la cible est la position de base.
				AppliquerNouvelleCible(kPositionBaseCamera);
				cibleOrientation = Vector3.up;
			}
		}

		// Animation de la position vers la cible.
		timerCible += Time.deltaTime;
		float proportion = timerCible / kTempsAnimation;
		if (proportion > 1.0f)
			proportion = 1.0f;

		mainCamera.transform.position = new Vector3 (easeInOutQuad (positionInitiale.x, cible.x, proportion),
		                                             easeInOutQuad (positionInitiale.y, cible.y, proportion),
		                                             easeInOutQuad (positionInitiale.z, cible.z, proportion));
		
		// Regarder vers le joueur.
		Quaternion targetRotation = kRotationBaseCamera;
		if (estSolo && hipCenter.position != MoveJointsForGuitar.HIDING_POS) {
			Vector3 direction = hipCenter.position + new Vector3(0, 2.0f, 0) - mainCamera.transform.position;
			targetRotation.SetLookRotation(direction, cibleOrientation);
		}
		mainCamera.transform.rotation = Quaternion.RotateTowards (mainCamera.transform.rotation,
		                                                          targetRotation,
		                                                          30.0f * Time.deltaTime);

	}

	void ChoisirNouvelleCibleAleatoire() {
		// Choix de la position.
		Vector3 distanceHanches = new Vector3(Random.Range(kDistanceMin.x, kDistanceMax.x),
		                                      Random.Range(kDistanceMin.y, kDistanceMax.y),
		                                      Random.Range(kDistanceMin.z, kDistanceMax.z));
		AppliquerNouvelleCible(hipCenter.position + distanceHanches);

		// Choix de l'orientation.
		cibleOrientation = (new Vector3 (Random.Range (-kToleranceHaut.x, kToleranceHaut.x),
		                                 1.0f,
		                                 Random.Range (-kToleranceHaut.z, kToleranceHaut.z))).normalized;
	}

	void AppliquerNouvelleCible(Vector3 cible) {
		this.cible = cible;
		positionInitiale = mainCamera.transform.position;
		timerCible = 0;
		timerPause = 0;
	}

	// Retour aux parametres de base de la camera.
	public void ReinitialiserCamera() {
		if (initialise) {
			mainCamera.transform.position = kPositionBaseCamera;
			mainCamera.transform.rotation = kRotationBaseCamera;
		}
	}

	private float easeInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value + start;
		value--;
		return -end / 2 * (value * (value - 2) - 1) + start;
	}

	// Instance unique de cette classe.
	private static CameraSolo instance;

	// Indique si on a sauvegarde les position / rotation de base de la caméra.
	private bool initialise = false;

	// Position de base de la camera pour la guitare.
	private Vector3 kPositionBaseCamera;

	// Rotation de base de la camera pour la guitare.
	private Quaternion kRotationBaseCamera;

	// Distances minimales par rapport aux hanches.
	private Vector3 kDistanceMin = new Vector3(-2.0f, -1.0f, 3.8f);

	// Distances maximales par rapport aux hanches.
	private Vector3 kDistanceMax = new Vector3(2.0f, 0.0f, 6.0f);

	// Variations possibles pour le haut.
	private Vector3 kToleranceHaut = new Vector3(0.1f, 0.1f, 0.1f);

	// Position au debut du mouvement vers la cible.
	private Vector3 positionInitiale;

	// Cible de la camera par rapport aux hanches.
	private Vector3 cible;

	// Orientation du haut cible.
	private Vector3 cibleOrientation;

	// Timer pour la cible courante.
	private float timerCible = 0;

	// Timer pour prendre une pause a la fin de l'animation.
	private float timerPause = 0;

	// Temps pour une animation.
	private const float kTempsAnimation = 2.0f;

	// Temps immobile a la fin d'une animation.
	private const float kTempsImmobile = 0.5f;
}
