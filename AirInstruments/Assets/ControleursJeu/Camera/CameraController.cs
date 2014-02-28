using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Camera du jeu.
	public Camera mainCamera;

	// Field of view par defaut.
	public float defaultFieldOfView;

	// Field of view du piano.
	public float pianoFieldOfView;

	// Vitesse de changement du field of view, en unités par deltaTime.
	public float fieldOfViewSpeed;

	// Vitesse de rotation, en degrés par deltaTime.
	public float rotationSpeed;

	void Start () {
		// Zoom par défaut.
		targetFieldOfView = defaultFieldOfView;

		// Se rappeler de la rotation initiale.
		chooseInstrumentRotation = mainCamera.transform.rotation;
	}

	// Update is called once per frame.
	void Update () {
		// Ajustements du zoom.
		float mainCameraFieldOfView = mainCamera.fieldOfView;
		if (mainCamera.fieldOfView != targetFieldOfView) {
			Vector2 fovActuel = new Vector2(mainCameraFieldOfView, 0);
			Vector2 fovTarget = new Vector2(targetFieldOfView, 0);
			Vector2 newFov = Vector2.MoveTowards(fovActuel, fovTarget, fieldOfViewSpeed * Time.deltaTime);
			mainCamera.fieldOfView = newFov.x;
		}

		// Ajustements de la rotation.
		if (targetRotation != Quaternion.identity) {
			Quaternion rotActuel = mainCamera.transform.rotation;
			Quaternion newRot = Quaternion.RotateTowards(rotActuel,
			                                             targetRotation,
			                                             rotationSpeed);
			mainCamera.transform.rotation = newRot;

			if (newRot == targetRotation) {
				targetRotation = Quaternion.identity;
			}
		}
	}

	// Deplace la camera entre les etats specifies.
	public void AccederEtat (GameState.State from, GameState.State to) {
		// Remettre la caméra dans le monde global.
		mainCamera.transform.parent = null;

		// Par défaut, on met le zoom par défaut.
		targetFieldOfView = defaultFieldOfView;

		// Aller de unknown (au debut) vers le menu de choix d'instrument.
		if (from == GameState.State.Unknown &&
		    to == GameState.State.ChooseInstrument) {
			GameState.ObtenirInstance().OnCompleteTransition();
		}
		// Aller vers le drum.
		else if (from == GameState.State.ChooseInstrument &&
		    to == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersDrum").Play();
		}
		// Aller vers le piano.
		else if (from == GameState.State.ChooseInstrument &&
		         to == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersPiano").Play();
			targetFieldOfView = pianoFieldOfView;
		}
		// Aller vers la guitare.
		else if (from == GameState.State.ChooseInstrument &&
		    	 to == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireVersGuitare").Play();
		}
		// Quitter le drum.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Drum) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterDrum").Play();
		}
		// Quitter le piano.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Piano) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterPiano").Play();
		}
		// Quitter la guitare.
		else if (to == GameState.State.ChooseInstrument &&
		         from == GameState.State.Guitar) {
			iTweenEvent.GetEvent(mainCamera.gameObject, "trajectoireQuitterGuitare").Play();
		}

	}

	public void RegarderPositionDefaut() {
		targetRotation = chooseInstrumentRotation;
	}

	// Field of view que la caméra doit atteindre.
	private float targetFieldOfView;

	// Angle de la caméra lors du menu de sélection d'instrument.
	private Quaternion chooseInstrumentRotation;

	// Angle que la caméra doit atteindre.
	private Quaternion targetRotation = Quaternion.identity;
}
