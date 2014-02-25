using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntelHandController : MonoBehaviour {

	// Prefab de boule rouge représentant un doigt.
	public GameObject fingerSpherePrefab;

	// Prefab de boule transparent repréesentant un joint de la main.
	public GameObject handJointSpherePrefab;

	// Liste des boules rouges affichées dans la scene.
	private List<GameObject> fingerSpheres = new List<GameObject>();

	// Nombre de boules rouges a créer.
	private const int numFingerSpheres = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2;

	void Start () {
		// Initialiser la caméra Creative.
		KinectPowerInterop.InitializeHandTracker ();

		// Créer les boules rouges de doigts.
		for (int i = 0; i < numFingerSpheres; ++i) {
			KinectPowerInterop.HandJointIndex joint_index =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

			if (joint_index == KinectPowerInterop.HandJointIndex.PINKY_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.RING_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.MIDDLE_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.INDEX_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.THUMB_TIP) {
				createFingerSphere(new Vector3(i * 1.5f, -5, -5));
			} else {
				createHandJointSphere(new Vector3(i * 1.5f, -5, -5));
			}
		}
	}

	public Vector3 TransformerPositionDoigt(KinectPowerInterop.HandJointInfo handJointInfo) {
		return new Vector3(-handJointInfo.x * 45,
		                   -handJointInfo.y * 45 - 10,
		                   -handJointInfo.z * 52 + 20);
	}

	private const float hauteurCibleMain = 0.43f;
	private const float differenceHauteurCarreMaxPourAjustement = 0.030f;

	void Update () {
		// Mettre a jour la position des boules rouge en fonction des donnees de la caméra Creative.
		KinectPowerInterop.GetHandsSkeletons (hand_joints);

		// Calculer les ajustements de hauteur.
		float[] ajustementsHauteur = new float[2];
		for (int i = 0; i < 2; ++i) {
			// Calculer la hauteur moyenne des bases de doigts.
			float sommeHauteurs = 0.0f;
			for (int j = (int)KinectPowerInterop.HandJointIndex.PINKY_BASE;
			     j <= (int)KinectPowerInterop.HandJointIndex.INDEX_BASE;
			     j += 3) {
				int indexDeBase = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*i + j;
				sommeHauteurs += hand_joints[indexDeBase].z;
			}
			float hauteurMoyenne = sommeHauteurs / 4.0f;

			// Calculer l'ajustement nécessaire pour cette main.
			float difference = hauteurCibleMain - hauteurMoyenne;
			float differenceCarre = difference * difference;
			if (differenceCarre > differenceHauteurCarreMaxPourAjustement) {
				differenceCarre = differenceHauteurCarreMaxPourAjustement;
			}

			ajustementsHauteur[i] = difference;
			if (differenceHauteurCarreMaxPourAjustement != 0) {
				difference *= (differenceHauteurCarreMaxPourAjustement - differenceCarre) /
					differenceHauteurCarreMaxPourAjustement;
			}
		}

		// Placer toutes les boules de doigts.
		for (int i = 0; i < hand_joints.Length; ++i) {
			KinectPowerInterop.HandJointIndex jointIndex =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);
			int indexMain = i / (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS;

			// Ajuster la hauteur (idee de Vanier).
			hand_joints[i].z += ajustementsHauteur[indexMain];

			// Appliquer de belles multiplications.
			Vector3 targetPosition = TransformerPositionDoigt(hand_joints[i]);


			if (hand_joints[i].error > 5.0f) {
				targetPosition = new Vector3(0.0f, 0.0f, 50.0f);
			}

			// Allonger le pouce.
			if (jointIndex == KinectPowerInterop.HandJointIndex.THUMB_TIP) {
				/*
				// Trouver l'index de la base du pouce.
				int diffBoutExtremite = (int)KinectPowerInterop.HandJointIndex.THUMB_TIP -
										(int)KinectPowerInterop.HandJointIndex.THUMB_BASE;
				int indexBase = i - diffBoutExtremite;
				Vector3 positionBasePouce = TransformerPositionDoigt(hand_joints[indexBase]);

				Vector2 baseVersExtremitePouce = new Vector2(targetPosition.x - positionBasePouce.x,
				                                             targetPosition.y - positionBasePouce.y);
				*/

				targetPosition.y = targetPosition.y + 2.0f;
			}

			// Appliquer les positions aux boules.
			HandJointSphere handJointSphereScript = (HandJointSphere)fingerSpheres[i].GetComponent(typeof(HandJointSphere));
			if (handJointSphereScript != null) {
				handJointSphereScript.SetTargetPosition(targetPosition);
			} else {
				FingerSphere fingerSphereScript = (FingerSphere)fingerSpheres[i].GetComponent(typeof(FingerSphere));
				fingerSphereScript.SetTargetPosition(targetPosition);
			}
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

	private GameObject createHandJointSphere(Vector3 position) {
		GameObject handJointSphere = (GameObject)Instantiate (handJointSpherePrefab,
		                                                      position,
		                                                      Quaternion.identity);
		fingerSpheres.Add (handJointSphere);
		return handJointSphere;
	}

	// Buffer pour récupérer les positions des boules rouges.
	private KinectPowerInterop.HandJointInfo[] hand_joints =
		new KinectPowerInterop.HandJointInfo[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2];
}
