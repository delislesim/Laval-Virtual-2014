using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntelHandController : MonoBehaviour {

	// Prefab de boule rouge représentant un doigt.
	public GameObject fingerSpherePrefab;

	// Liste des boules rouges affichées dans la scene.
	private List<GameObject> fingerSpheres = new List<GameObject>();

	// Nombre de boules rouges a créer.
	private const int numFingerSpheres = 1;

	void Start () {
		// Créer les boules rouges de doigts.
		for (int i = 0; i < numFingerSpheres; ++i) {
			createFingerSphere(new Vector3(i * 1.5f, -5, -5));
		}
	}

	void Update () {
	
	}

	// Crée une boule rouge représentant un bout de doigt et l'ajoute
	// a la liste des boules rouges.
	private GameObject createFingerSphere(Vector3 position) {
		GameObject fingerSphere = (GameObject)Instantiate (fingerSpherePrefab,
		                                                   position,
		                                                   Quaternion.identity);
		fingerSpheres.Add (fingerSphere);
		return fingerSphere;
	}
}
