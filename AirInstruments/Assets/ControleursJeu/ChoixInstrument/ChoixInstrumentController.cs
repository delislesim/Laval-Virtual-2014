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
		cameraController.RegarderPositionDefaut();
	}
}
