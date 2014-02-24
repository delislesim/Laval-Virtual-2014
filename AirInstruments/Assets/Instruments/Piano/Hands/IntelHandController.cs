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
			KinectPowerInterop.HandJointIndex joint_index = (KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

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

	void Update () {
		// Mettre a jour la position des boules rouge en fonction des donnees de la caméra Creative.
		KinectPowerInterop.GetHandsSkeletons (hand_joints);

		for (int i = 0; i < hand_joints.Length; ++i) {
			KinectPowerInterop.HandJointIndex jointIndex =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

			Vector3 targetPosition = TransformerPositionDoigt(hand_joints[i]);

			if (hand_joints[i].error > 5.0f) {
				targetPosition = new Vector3(0.0f, 0.0f, 50.0f);
			}

			// Allonger le pouce.
			if (jointIndex == KinectPowerInterop.HandJointIndex.THUMB_TIP) {
				// Trouver l'index de la base du pouce.
				int diffBoutExtremite = (int)KinectPowerInterop.HandJointIndex.THUMB_TIP -
										(int)KinectPowerInterop.HandJointIndex.THUMB_BASE;
				int indexBase = i - diffBoutExtremite;
				Vector3 positionBasePouce = TransformerPositionDoigt(hand_joints[indexBase]);

				Vector2 baseVersExtremitePouce = new Vector2(targetPosition.x - positionBasePouce.x,
				                                             targetPosition.y - positionBasePouce.y);

				// Allonger la distance de la base du pouce a l'extremite.
				baseVersExtremitePouce = baseVersExtremitePouce * 1.5f;

				//targetPosition.x = positionBasePouce.x + baseVersExtremitePouce.x;
				targetPosition.y = positionBasePouce.y + baseVersExtremitePouce.y;
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
