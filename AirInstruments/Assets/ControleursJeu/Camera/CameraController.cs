using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Camera du jeu.
	public Camera mainCamera;

	public CameraController() {
		instance = this;
	}

	public static CameraController ObtenirInstance() {
		return instance;
	}

	// Update is called once per frame.
	void Update () {
		timerAnimation += Time.deltaTime;

		// Ajustements du zoom.
		float mainCameraFieldOfView = mainCamera.fieldOfView;
		if (mainCamera.fieldOfView != targetFieldOfView) {
			// Accelerer la vitesse du zoom.
			if (vitesseFovActuelle < kFieldOfViewSpeed && (estAnimationDrum || timerAnimation > kTempsAvantFov)) {
				float fovSpeed = estAnimationDrum ? kFieldOfViewSpeedDrum : kFieldOfViewSpeed;
				vitesseFovActuelle += kAccelerationFov * Time.deltaTime;
				if (vitesseFovActuelle > fovSpeed)
					vitesseFovActuelle = fovSpeed;
			}

			// Changer le zoom.
			Vector2 fovActuel = new Vector2(mainCameraFieldOfView, 0);
			Vector2 fovTarget = new Vector2(targetFieldOfView, 0);
			Vector2 newFov = Vector2.MoveTowards(fovActuel, fovTarget, vitesseFovActuelle * Time.deltaTime);
			mainCamera.fieldOfView = newFov.x;

			// Si on a atteint notre cible, la vitesse de changement du fov devient nulle.
			if (mainCamera.fieldOfView == targetFieldOfView) {
				vitesseFovActuelle = 0;
			}
		}

		// Ajustements de la rotation.
		if (ajusterRotation) {
			if (quittePiano && rotationSpeed > kRotationSpeedPiano) {
				rotationSpeed -= 2.0f * Time.deltaTime;
			}

			Quaternion rotActuel = mainCamera.transform.rotation;
			Quaternion newRot = Quaternion.RotateTowards(rotActuel,
			                                             targetRotation,
			                                             rotationSpeed * Time.deltaTime);
			mainCamera.transform.rotation = newRot;
		}
	}

	// Mettre la camera a la racine du projet.
	public void ReprendreCamera() {
		mainCamera.transform.parent = null;
	}

	// Deplace la camera entre les etats specifies.
	public void AccederEtat (GameState.State from, GameState.State to) {
		ajusterRotation = false;
		timerAnimation = 0;

		// Remettre la caméra dans le monde global.
		ReprendreCamera ();

		rotationSpeed = kRotationSpeedDefault;
		quittePiano = false;

		estAnimationDrum = false;

		// Aller de unknown (au debut) vers le menu de choix d'instrument.
		if (from == GameState.State.Unknown &&
		    to == GameState.State.ChooseInstrument) {
			targetRotation = Quaternion.Euler(kAngleChooseInstrument);
			targetFieldOfView = kFovChoixInstrument;
			GameState.ObtenirInstance().OnCompleteTransition();
		}
		// Aller vers le drum.
		else if (from == GameState.State.ChooseInstrument &&
		    to == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersDrum").Play();
			targetRotation = Quaternion.Euler(kAngleDrum);
			targetFieldOfView = kFovDrum;
			estAnimationDrum = true;
		}
		// Aller vers le piano.
		else if (from == GameState.State.ChooseInstrument &&
		         to == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersPiano").Play();
			targetRotation = Quaternion.Euler(kAnglePiano);
			targetFieldOfView = kFovPiano;
		}
		// Aller vers la guitare.
		else if (from == GameState.State.ChooseInstrument &&
		    	 to == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersGuitare").Play();
			targetRotation = Quaternion.Euler(kAngleGuitare);
			targetFieldOfView = kFovGuitare;
		}
		// Quitter le drum.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterDrum").Play();
			targetRotation = Quaternion.Euler(kAngleChooseInstrument);
			targetFieldOfView = kFovChoixInstrument;
			estAnimationDrum = true;
		}
		// Quitter le piano.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterPiano").Play();
			targetRotation = Quaternion.Euler(kAngleChooseInstrument);
			targetFieldOfView = kFovChoixInstrument;

			rotationSpeed = kRotationSpeedPiano;
			quittePiano = true;
		}
		// Quitter la guitare.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterGuitare").Play();
			targetRotation = Quaternion.Euler(kAngleChooseInstrument);
			targetFieldOfView = kFovChoixInstrument;
		}

	}

	public void AjusterRotation() {
		ajusterRotation = true;
	}

	// Unique instance du controleur de camera.
	public void ForcerFieldOfView(float targetFieldOfView) {
		vitesseFovActuelle = 0;
		this.targetFieldOfView = targetFieldOfView;

	}

	// Instance unique du controleur de camera.
	private static CameraController instance;

	// Indique si l'animation est pour le drum.
	private bool estAnimationDrum = false;

	// Indique si on doit ajuster la rotation de la caméra. On n'ajuste pas
	// la rotation pendant les animations.
	private bool ajusterRotation = false;

	// Timer pour animations.
	private float timerAnimation = 0;

	// Temps a partir duquel ajuster le FOV.
	private const float kTempsAvantFov = 1.5f;

	// Field of view que la caméra doit atteindre.
	private float targetFieldOfView;

	// Angle que la caméra doit atteindre.
	private Quaternion targetRotation = Quaternion.identity;

	// Vitesse actuelle de changement du FOV.
	private float vitesseFovActuelle = 0;

	// FOV du menu de choix d'instrument.
	private const float kFovChoixInstrument = 55f;

	// Angle de la caméra lors du menu de choix d'instrument.
	private Vector3 kAngleChooseInstrument = new Vector3 (15f, 0, 0);

	// FOV du drum.
	private const float kFovDrum = 76.2f;

	// Angle de la caméra lors du drum.
	private Vector3 kAngleDrum = new Vector3(14.15093f, 0, 0);

	// FOV du piano.
	private const float kFovPiano = 35.9f;
	
	// Angle de la caméra lors du piano.
	private Vector3 kAnglePiano = new Vector3(35.9f, 0, 0);

	// FOV de la guitare.
	public const float kFovGuitare = 76.2f;

	// Vitesse de rotation pour la guitare.
	private const float kRotationSpeedPiano = 10.0f;

	// Indique si on est en train de quitter le piano.
	private bool quittePiano = false;

	// Angle de la caméra lors de la guitare.
	private Vector3 kAngleGuitare = new Vector3(0.09139769f, 195.2244f, 0f);

	// Vitesse de rotation pour cette trajectoire.
	private float rotationSpeed = 0;

	// Vitesse de changement du field of view pour le drum, en unités par deltaTime.
	private const float kFieldOfViewSpeedDrum = 5.0f;

	// Vitesse de changement du field of view, en unités par deltaTime.
	private const float kFieldOfViewSpeed = 8.0f;

	// Acceleration pour changer FOV.
	private const float kAccelerationFov = 20.0f;

	// Vitesse de rotation par defaut.
	private const float kRotationSpeedDefault = 5f;

}
