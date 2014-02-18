using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntelHandController : MonoBehaviour {

	// Prefab de boule rouge représentant un doigt.
	public GameObject fingerSpherePrefab;

	// Liste des boules rouges affichées dans la scene.
	private List<GameObject> fingerSpheres = new List<GameObject>();

	// Nombre de boules rouges a créer.
	private const int numFingerSpheres = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2;

	void Start () {
		// Initialiser la caméra Creative.
		KinectPowerInterop.InitializeHandTracker ();

		// Créer les boules rouges de doigts.
		for (int i = 0; i < numFingerSpheres; ++i) {
			createFingerSphere(new Vector3(i * 1.5f, -5, -5));
		}
	}

	void Update () {
		// Mettre a jour la position des boules rouge en fonction des donnees de la caméra Creative.
		KinectPowerInterop.GetHandsSkeletons (hand_joints);

		for (int i = 0; i < hand_joints.Length; ++i) {
			fingerSpheres[i].transform.position = new Vector3(-hand_joints[i].x * 45, -hand_joints[i].y * 45 - 10, -hand_joints[i].z * 60 + 20);

			/*
			const float errorMax = 5.0f;
			float error = hand_joints[i].error;
			if (error > errorMax)
				error = errorMax;

			fingerSpheres[i].renderer.material.SetColor("_Color", new Color32(255, 0, 0, (byte)(255*(errorMax - error) / errorMax)));
			*/
		}
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

	// Buffer pour récupérer les positions des boules rouges.
	private KinectPowerInterop.HandJointInfo[] hand_joints =
		new KinectPowerInterop.HandJointInfo[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2];
}
