using UnityEngine;
using System.Collections;

public class BaguetteDrum : MonoBehaviour {

	// Main dans laquelle se trouve la baguette.
	public GameObject main;

	// Bout de la baguette suppose etre dans la main.
	public GameObject boutBaguetteMain;

	public Collider autreBaguette;

	void Start () {
		positionInitiale = transform.position;
		rotationInitiale = transform.rotation;

		Physics.IgnoreCollision (collider, autreBaguette);
	}

	void OnDisable () {
		// Remettre les baguettes a leur position initiale pour que
		// le joint soit bien connecte au redemarrage.
		transform.position = positionInitiale;
		transform.rotation = rotationInitiale;
	}

	// Update is called once per frame
	void FixedUpdate () {
		// Bouger la baguette pour que son bout soit au centre de la main.
		/*
		Vector3 nouvellePositionBoutBaguette = Vector3.MoveTowards (boutBaguetteMain.transform.position,
		                                                           main.transform.position,
		                                                           kVitesseBaguette * Time.deltaTime);*/
		renderer.enabled = main.renderer.enabled;
		Vector3 deplacement = main.transform.position - boutBaguetteMain.transform.position;
		if (deplacement.magnitude >= 0.40f) {
			transform.position = transform.position + deplacement;
		}
	}

	private const float kVitesseBaguette = 0.25f;

	// Position initiale.
	private Vector3 positionInitiale;

	// Rotation initiale.
	private Quaternion rotationInitiale;
}
