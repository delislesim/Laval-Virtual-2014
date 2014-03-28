using UnityEngine;
using System.Collections;

public class BaguetteDrum : MonoBehaviour {

	// Main dans laquelle se trouve la baguette.
	public GameObject main;

	// Bout de la baguette suppose etre dans la main.
	public GameObject boutBaguetteMain;

	// Extremite de la baguette.
	public GameObject tip;

	public Collider autreBaguette;

	private int layerMask;

	void Start () {
		positionInitiale = transform.position;
		rotationInitiale = transform.rotation;

		Physics.IgnoreCollision (collider, autreBaguette);

		layerMask = LayerMask.NameToLayer ("Cylinder") | LayerMask.NameToLayer ("DrumComponent");
	}

	void OnDisable () {
		// Remettre les baguettes a leur position initiale pour que
		// le joint soit bien connecte au redemarrage.
		transform.position = positionInitiale;
		transform.rotation = rotationInitiale;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float maximum = 0;
		Vector3 direction = main.transform.position - tip.transform.position;
		if (Physics.Raycast (tip.transform.position, direction.normalized, direction.magnitude, layerMask)) {
			maximum = 0.4f;
		}

		Vector3 deplacement = main.transform.position - boutBaguetteMain.transform.position;
		if (deplacement.magnitude > maximum) {
			transform.position = transform.position + deplacement;
		}
	}

	void Update() {
		renderer.enabled = main.renderer.enabled;
	}

	private const float kVitesseBaguette = 0.1f;

	// Position initiale.
	private Vector3 positionInitiale;

	// Rotation initiale.
	private Quaternion rotationInitiale;
}
