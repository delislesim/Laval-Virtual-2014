using UnityEngine;
using System.Collections;

public class ChoixInstrumentController : MonoBehaviour {

	// Controleur de la camera.
	public CameraController cameraController;

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
		GestureId gesture = gestureRecognition.GetCurrentGesture ();
		switch (gesture) {
		case GestureId.GESTURE_DRUM : {
			GameState.ObtenirInstance().AccederEtat(GameState.State.Drum);
			break;
		}
		case GestureId.GESTURE_GUITAR : {
			GameState.ObtenirInstance().AccederEtat(GameState.State.Guitar);
			break;
		}
		case GestureId.GESTURE_PIANO : {
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
