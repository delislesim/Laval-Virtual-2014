using UnityEngine;
using System.Collections;

public class CameraFOVFollower : MonoBehaviour {

	public Camera mainCamera;

	// Update is called once per frame
	void Update () {
		camera.fieldOfView = mainCamera.fieldOfView;
	}
}
