using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntelHandController : MonoBehaviour {

	// Prefab de boule rouge représentant un doigt.
	public GameObject fingerSpherePrefab;

	// Prefab de boule transparent repréesentant un joint de la main.
	public GameObject handJointSpherePrefab;

	// Prefab de cylindre.
	public GameObject cylindrePrefab;

	// Liste des boules rouges affichées dans la scene.
	private List<GameObject> spheres = new List<GameObject>();

	// Liste des cylindres affiches dans la scene.
	private List<GameObject> cylindres = new List<GameObject> ();

	// Nombre de boules rouges a créer.
	private const int nombreSpheres = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2;

	// Nombre de cylindre a créer par main.
	private const int nombreCylindresParDoigt = 16;

	// Nombre de cylindres a créer.
	private const int nombreCylindres = nombreCylindresParDoigt * 2;

	// Indique si le hand tracker de Intel a été initialisé.
	private bool handTrackerInitialized = false;

	void Start () {
		// Initialiser la caméra Creative.
		KinectPowerInterop.InitializeHandTracker ();

		// Créer les boules de doigts et de jointures.
		for (int i = 0; i < nombreSpheres; ++i) {
			KinectPowerInterop.HandJointIndex joint_index =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

			if (joint_index == KinectPowerInterop.HandJointIndex.PINKY_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.RING_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.MIDDLE_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.INDEX_TIP ||
			    joint_index == KinectPowerInterop.HandJointIndex.THUMB_TIP) {
				createFingerSphere(Vector3.zero);
			} else {
				createHandJointSphere(Vector3.zero);
			}
		}

		// Creer les cylindres.
		for (int i = 0; i < nombreCylindres; ++i) {
			GameObject cylindre = (GameObject)Instantiate (cylindrePrefab);
			cylindre.transform.parent = this.transform;
			cylindre.transform.localScale = cylindrePrefab.transform.localScale;
			cylindres.Add (cylindre);
		}
	}

	public Vector3 TransformerPositionDoigt(KinectPowerInterop.HandJointInfo handJointInfo) {
		return new Vector3(-handJointInfo.x * 45,
		                   -(-handJointInfo.z * 52 + 20),
		                   -handJointInfo.y * 45 - 5);
	}

	void Update () {
		// Mettre a jour la position des boules rouge en fonction des donnees de la caméra Creative.
		KinectPowerInterop.GetHandsSkeletons (hand_joints);

		// Calculer les ajustements de hauteur.
		float[] ajustementsHauteur = CalculerAjustementsHauteur ();

		// Placer des spheres sur les articulations et bouts de doigts.
		for (int i = 0; i < hand_joints.Length; ++i) {
			PlacerSphere(i, ajustementsHauteur);
		}

		// Placer les cylindres pour qu'ils relient les spheres.
		for (int i = 0; i < cylindres.Count; ++i) {
			PlacerCylindre(i);
		}
	}

	private void PlacerSphere(int index, float[] ajustementsHauteur) {
		KinectPowerInterop.HandJointInfo jointInfo = hand_joints [index];

		KinectPowerInterop.HandJointIndex jointIndex;
		int indexMain;
		ObtenirJointEtNumeroMain(index, out jointIndex, out indexMain);
		
		// Ajuster la hauteur du joint selon le snap (idee de Vanier).
		jointInfo.z += ajustementsHauteur[indexMain];

		// Allonger le pouce.
		if (jointIndex == KinectPowerInterop.HandJointIndex.THUMB_TIP) {
			jointInfo.y -= 2.0f / 45.0f;
			// TODO(fdoray): Verifier cette donnee.
		}

		// Appliquer de belles multiplications.
		Vector3 targetPosition = TransformerPositionDoigt(jointInfo);

		// Appliquer les positions aux boules.
		HandJointSphereI jointureSphereScript = ObtenirHandJointSphereScript (index);
		jointureSphereScript.SetTargetPosition (targetPosition, jointInfo.error < kErreurMaxPermise);
	}

	private HandJointSphereI ObtenirHandJointSphereScript(int index) {
		HandJointSphere jointureSphereScript = (HandJointSphere)spheres[index].GetComponent(typeof(HandJointSphere));
		if (jointureSphereScript != null) {
			return jointureSphereScript;
		} else {
			FingerSphere sphereDoigtScript = (FingerSphere)spheres[index].GetComponent(typeof(FingerSphere));
			return sphereDoigtScript;
		}
	}

	private void PlacerCylindre(int index) {
		int indexModulo = index % nombreCylindresParDoigt;
		int indexMain = index / nombreCylindresParDoigt;

		// Trouver les identifiants des joints précédant et suivant ce cylindre.
		KinectPowerInterop.HandJointIndex jointAvant;
		KinectPowerInterop.HandJointIndex jointApres;
		switch (indexModulo) {
		case 0: {
			jointAvant = KinectPowerInterop.HandJointIndex.FOREARM;
			jointApres = KinectPowerInterop.HandJointIndex.PALM;
			break;
		}
		case 1: {
			jointAvant = KinectPowerInterop.HandJointIndex.PALM;
			jointApres = KinectPowerInterop.HandJointIndex.PINKY_BASE;
			break;
		}
		case 2: {
			jointAvant = KinectPowerInterop.HandJointIndex.PINKY_BASE;
			jointApres = KinectPowerInterop.HandJointIndex.PINKY_MID;
			break;
		}
		case 3: {
			jointAvant = KinectPowerInterop.HandJointIndex.PINKY_MID;
			jointApres = KinectPowerInterop.HandJointIndex.PINKY_TIP;
			break;
		}
		case 4: {
			jointAvant = KinectPowerInterop.HandJointIndex.PALM;
			jointApres = KinectPowerInterop.HandJointIndex.RING_BASE;
			break;
		}
		case 5: {
			jointAvant = KinectPowerInterop.HandJointIndex.RING_BASE;
			jointApres = KinectPowerInterop.HandJointIndex.RING_MID;
			break;
		}
		case 6: {
			jointAvant = KinectPowerInterop.HandJointIndex.RING_MID;
			jointApres = KinectPowerInterop.HandJointIndex.RING_TIP;
			break;
		}
		case 7: {
			jointAvant = KinectPowerInterop.HandJointIndex.PALM;
			jointApres = KinectPowerInterop.HandJointIndex.MIDDLE_BASE;
			break;
		}
		case 8: {
			jointAvant = KinectPowerInterop.HandJointIndex.MIDDLE_BASE;
			jointApres = KinectPowerInterop.HandJointIndex.MIDDLE_MID;
			break;
		}
		case 9: {
			jointAvant = KinectPowerInterop.HandJointIndex.MIDDLE_MID;
			jointApres = KinectPowerInterop.HandJointIndex.MIDDLE_TIP;
			break;
		}
		case 10: {
			jointAvant = KinectPowerInterop.HandJointIndex.PALM;
			jointApres = KinectPowerInterop.HandJointIndex.INDEX_BASE;
			break;
		}
		case 11: {
			jointAvant = KinectPowerInterop.HandJointIndex.INDEX_BASE;
			jointApres = KinectPowerInterop.HandJointIndex.INDEX_MID;
			break;
		}
		case 12: {
			jointAvant = KinectPowerInterop.HandJointIndex.INDEX_MID;
			jointApres = KinectPowerInterop.HandJointIndex.INDEX_TIP;
			break;
		}
		case 13: {
			jointAvant = KinectPowerInterop.HandJointIndex.PALM;
			jointApres = KinectPowerInterop.HandJointIndex.THUMB_BASE;
			break;
		}
		case 14: {
			jointAvant = KinectPowerInterop.HandJointIndex.THUMB_BASE;
			jointApres = KinectPowerInterop.HandJointIndex.THUMB_MID;
			break;
		}
		case 15: {
			jointAvant = KinectPowerInterop.HandJointIndex.THUMB_MID;
			jointApres = KinectPowerInterop.HandJointIndex.THUMB_TIP;
			break;
		}
		default: {
			return;
		}
		}

		int indexJointAvant = (int)jointAvant + indexMain * (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS;
		int indexJointApres = (int)jointApres + indexMain * (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS;
		Vector3 positionJointAvant = spheres [indexJointAvant].transform.position;
		Vector3 positionJointApres = spheres [indexJointApres].transform.position;

		HandJointSphereI handJointAvant = ObtenirHandJointSphereScript (indexJointAvant);
		HandJointSphereI handJointApres = ObtenirHandJointSphereScript (indexJointApres);

		if (handJointAvant.IsValid() && handJointApres.IsValid()) {
			HandCylinder handCylinder = (HandCylinder)cylindres [index].GetComponent (typeof(HandCylinder));
			handCylinder.DefinirExtremites (positionJointAvant, positionJointApres);
			cylindres[index].SetActive(true);
		} else {
			cylindres[index].SetActive(false);
		}
	}

	private void ObtenirJointEtNumeroMain(int index,
	                                      out KinectPowerInterop.HandJointIndex jointIndex, 
	                                      out int indexMain) {
		jointIndex = (KinectPowerInterop.HandJointIndex)(index % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);
		indexMain = index / (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS;
	}

	// "Snap" pour la hauteur des mains.
	private float[] CalculerAjustementsHauteur() {
		// Calcul du snap.
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
			ajustementsHauteur[i] = 0;
		}
		return ajustementsHauteur;
	}

	// Crée une boule rouge représentant un bout de doigt et l'ajoute
	// a la liste des boules rouges.
	private GameObject createFingerSphere(Vector3 position) {
		GameObject fingerSphere = (GameObject)Instantiate (fingerSpherePrefab,
		                                                   position,
		                                                   Quaternion.identity);
		spheres.Add (fingerSphere);
		fingerSphere.transform.parent = this.transform;
		fingerSphere.transform.localScale = fingerSpherePrefab.transform.localScale;
		return fingerSphere;
	}

	private GameObject createHandJointSphere(Vector3 position) {
		GameObject handJointSphere = (GameObject)Instantiate (handJointSpherePrefab,
		                                                      position,
		                                                      Quaternion.identity);
		spheres.Add (handJointSphere);
		handJointSphere.transform.parent = this.transform;
		handJointSphere.transform.localScale = handJointSpherePrefab.transform.localScale;
		return handJointSphere;
	}

	// Buffer pour récupérer les positions des boules rouges.
	private KinectPowerInterop.HandJointInfo[] hand_joints =
		new KinectPowerInterop.HandJointInfo[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2];

	// Erreur maximale permise avant d'etre invalide.
	private const float kErreurMaxPermise = 5.0f;

	// --- Snap ---

	// Hauteur a laquelle la main doit se trouver.
	const float hauteurCibleMain = 0.43f;

	// Distance au carre a partir de laquelle ne plus faire de snap.
	const float differenceHauteurCarreMaxPourAjustement = 0.040f;
}
