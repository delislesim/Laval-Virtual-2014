using UnityEngine;
using System.Collections;

public class ChoixInstrumentController : MonoBehaviour {

	// Controleur de la camera.
	public CameraController cameraController;

	// Spotlight du piano.
	public SpotlightControl spotPiano;

	// Spotlight du drum.
	public SpotlightControl spotDrum;

	// Spotlight de la guitare.
	public SpotlightControl spotGuitare;

	// Appele avant d'entrer dans le mode de choix d'instrument.
	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
	}

	public void PrepareToStop() {
		// Des que l'animation de camera pour aller vers un instrument
		// commence, on se desactive.
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*
		// Verifier s'il y a un choix d'instrument actif.
		Pointeur pointeur = Pointeur.obtenirInstance ();
		int targetid = pointeur.GetCurrentTargetId ();
		if (targetid != -1) {
			switch (targetid) {
				case kDrumTargetId : {
					GameState.ObtenirInstance().AccederEtat(GameState.State.Drum);
					break;
				}
				case kGuitarTargetId : {
					GameState.ObtenirInstance().AccederEtat(GameState.State.Guitar);
					break;
				}
				case kPianoTargetId : {
					GameState.ObtenirInstance().AccederEtat(GameState.State.Piano);
					break;
				}
			}
		}
		*/

		GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();

		// Mettre a jour les spots.
		float completionPiano = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_PIANO);
		float completionDrum = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_DRUM);
		float completionGuitare = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_GUITAR);

		if (completionPiano < 0.2f && completionDrum < 0.2f && completionGuitare < 0.2f) {
			RallumerSpots();
		} else {
			// Ajuster les spots selon le niveau de complétion du geste.
			spotPiano.SetTargetIntensity(completionPiano == 0 ? 0 : 4.0f * (1.0f + completionPiano), 6.0f);
			spotDrum.SetTargetIntensity(completionDrum < 0.25 ? 0 : 4.0f * (1.0f + completionDrum), 6.0f);
			spotGuitare.SetTargetIntensity(completionGuitare == 0 ? 0 : 4.0f * (1.0f + completionGuitare), 6.0f);
		}


		// Aller a l'instrument dont le geste est complete.
		GestureId gesture = gestureRecognition.GetCurrentGesture ();
		switch (gesture) {
		case GestureId.GESTURE_DRUM : {
			RallumerSpots();
			GameState.ObtenirInstance().AccederEtat(GameState.State.Drum);
			break;
		}
		case GestureId.GESTURE_GUITAR : {
			RallumerSpots();
			GameState.ObtenirInstance().AccederEtat(GameState.State.Guitar);
			break;
		}
		case GestureId.GESTURE_PIANO : {
			RallumerSpots();
			GameState.ObtenirInstance().AccederEtat(GameState.State.Piano);
			break;
		}
		}

		// Initialiser le guidage si necessaire.
		if (!guidageInitialise) {
			GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.MENU_PRINCIPAL);
			guidageInitialise = true;
		}
	}

	void RallumerSpots () {
		spotPiano.SetTargetIntensity(8.0f, 6.0f);
		spotDrum.SetTargetIntensity(8.0f, 6.0f);
		spotGuitare.SetTargetIntensity(8.0f, 6.0f);
	}

	void OnEnable () {
		/*
		// Inserer les cibles possibles pour la main.
		Pointeur pointeur = Pointeur.obtenirInstance ();
		pointeur.RemoveAllTargets ();
		pointeur.AddTarget (kDrumTargetId,
		                   new Vector2 (0.9f, 0.3f),
		                   new Vector2 (0.4f, 0.4f));
		pointeur.AddTarget (kGuitarTargetId,
		                    new Vector2 (2.1f, 0.4f),
		                    new Vector2 (0.4f, 0.4f));
		pointeur.AddTarget (kPianoTargetId,
		                    new Vector2 (-0.5f, 0.5f),
		                    new Vector2 (0.4f, 0.4f));

		// Activer le pointeur.
		pointeur.gameObject.SetActive (true);
		*/

		// Ajouter les gestes.
		GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
		gestureRecognition.AddGesture (new GesturePiano ());
		gestureRecognition.AddGesture (new GestureDrum ());
		gestureRecognition.AddGesture (new GestureGuitar ());

		// Affichage du guidage
		guidageInitialise = false;
	}

	void OnDisable () {
		Pointeur.obtenirInstance ().gameObject.SetActive (false);
	}

	private bool guidageInitialise = false;

	private const int kDrumTargetId = 0;
	private const int kGuitarTargetId = 1;
	private const int kPianoTargetId = 2;
}
