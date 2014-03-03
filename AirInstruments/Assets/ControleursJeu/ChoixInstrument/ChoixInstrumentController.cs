using UnityEngine;
using System.Collections;

public class ChoixInstrumentController : MonoBehaviour {

	// Controleur de la camera.
	public CameraController cameraController;

	// Appele avant d'entrer dans le mode de choix d'instrument.
	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable () {
		// Ajuster l'angle de la camera Unity.
		cameraController.RegarderPositionDefaut();

		// Inserer les cibles possibles pour la main.
		Pointeur pointeur = Pointeur.obtenirInstance ();
		pointeur.RemoveAllTargets ();
		pointeur.AddTarget (kDrumTargetId,
		                   new Vector2 (0.6f, -0.6f),
		                   new Vector2 (0.4f, 1.0f));
		pointeur.AddTarget (kGuitarTargetId,
		                    new Vector2 (-0.3f, -0.4f),
		                    new Vector2 (0.4f, 1.0f));
		pointeur.AddTarget (kPianoTargetId,
		                    new Vector2 (1.8f, -0.3f),
		                    new Vector2 (0.4f, 1.0f));
	}

	private const int kDrumTargetId = 0;
	private const int kGuitarTargetId = 1;
	private const int kPianoTargetId = 2;
}
