using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IntelHandController : MonoBehaviour {

	// Prefab de boule rouge représentant un doigt.
	public GameObject fingerSpherePrefab;

	// Prefab de boule transparent repréesentant un joint de la main.
	public GameObject handJointSpherePrefab;

	// Prefab de cylindre.
	public GameObject cylindrePrefab;

	// Fleches de guidage.
	public List<PianoArrow> flechesGuidage;

	// Variables pour détecter si un snap est nécessaire
	private Vector3[] positionPrecedenteDoigts = new Vector3[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*2];
	private const float timeDownMove = 0.06f;
	private const float timeoutDownMove = 0.08f;
	private const float timeoutUp = 0.25f;
	private const float hauteurMaximale = 3.0f; // Relative aux touches blanches
	private const float vitesseMinimale = -10.0f;
	private List<DownMoveInfo> downMoveList = new List<DownMoveInfo>();

	// Rayon des doigts.
	private float kRayonDoigts;

	// Hauteur des blanches.
	private const float kHauteurBlanches = -2.8f;

	private struct DownMoveInfo
	{
		public float elapsedTime;
		public KinectPowerInterop.HandJointIndex indexDoigt;
		public float hauteurInitiale;
		public float elapsedTimeout;

		// Temps avec une vitesse vers le haut.
		public float elapsedTimeUp; 
	}

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

	// Tailles de mains.
	private float[] kLargeur = {0.06f, 0.07f, 0.08f, 0.095f};
	private float[] kHauteur = {0.14f, 0.15f, 0.19f, 0.21f};
	private int kIndexTailleDefaut = 2;

	// Index des mains.
	private const int kIndexMainDroite = 0;
	private const int kIndexMainGauche = 1;

	// Buffer pour récupérer les positions des boules rouges.
	private KinectPowerInterop.HandJointInfo[] hand_joints =
		new KinectPowerInterop.HandJointInfo[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2];
	
	// Erreur maximale permise avant d'etre invalide.
	private const float kErreurMaxPermise = 5.0f;

	// Limites de positions des bases de doigts avant d'afficher du guidage.
	private Vector3 kLimitesMin = new Vector3 (-9.0f, -2.5f, -7.35f);
	private Vector3 kLimitesMax = new Vector3 (8.6f, 0.6026f, -2.0f);

	// Limites de positions des bases de doigts pour passer l'etape de
	// mettre ses mains devant le capteur du tutorial.
	private Vector3 kLimitesCapteurMin = new Vector3 (-11.0f, -2.5f, -10f);
	private Vector3 kLimitesCapteurMax = new Vector3 (11.0f, 13.0f, -4.0f);

	void Start () {
		// Initialiser la caméra Creative.
		KinectPowerInterop.InitializeHandTracker ();

		// Taille de mains par defaut.
		KinectPowerInterop.SetHandMeasurements (kLargeur [kIndexTailleDefaut],
		                                        kHauteur [kIndexTailleDefaut]);

		// Créer les boules de doigts et de jointures.
		for (int i = 0; i < nombreSpheres; ++i) {
			KinectPowerInterop.HandJointIndex joint_index =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

			if (EstBoutDoigt(joint_index)) {
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

		// Noter le rayon des doigts.
		kRayonDoigts = spheres [(int)KinectPowerInterop.HandJointIndex.PINKY_TIP].transform.localScale.x;
	}

	void OnEnable () {
		// Creer une liste avec des infos de mouvement pour chaque doigt pour l'assistance.
		downMoveList.Clear ();
		for (int i = 0; i < (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2; ++i) {
			DownMoveInfo info = new DownMoveInfo();
			info.indexDoigt = (KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);
			downMoveList.Add(info);
		}
	}

	private bool EstBoutDoigt(KinectPowerInterop.HandJointIndex joint_index) {
		return joint_index == KinectPowerInterop.HandJointIndex.PINKY_TIP ||
			   joint_index == KinectPowerInterop.HandJointIndex.RING_TIP ||
			   joint_index == KinectPowerInterop.HandJointIndex.MIDDLE_TIP ||
			   joint_index == KinectPowerInterop.HandJointIndex.INDEX_TIP ||
			   joint_index == KinectPowerInterop.HandJointIndex.THUMB_TIP;
	}

	private void DetecterVitesse()
	{
		// Calculer la vitesse de chaque articulation.
		int i = 0;
		Vector3[] vectVitesse = new Vector3[(int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2];
		foreach(Vector3 posPrecedente in positionPrecedenteDoigts)
		{
			Vector3 posCourante = TransformerPositionDoigt (hand_joints[i]);
			if(posPrecedente != Vector3.zero) {
				vectVitesse[i] = (posCourante - posPrecedente)/Time.deltaTime;

				if (vectVitesse[i].y > 0) {
					DownMoveInfo info = downMoveList[i];
					info.elapsedTimeUp += Time.deltaTime;
					downMoveList[i] = info;
				}

				if ((downMoveList[i].elapsedTimeUp > timeoutUp || posCourante.y > downMoveList[i].hauteurInitiale) &&
				    EstBoutDoigt(downMoveList[i].indexDoigt)) {
					FingerSphere sphere = spheres[i].GetComponent<FingerSphere>();
					spheres[i].GetComponent<FingerSphere>().SetAllongementAssiste(false);
				}
			}
			positionPrecedenteDoigts[i] = posCourante;
			++i;
		}

		// Trouver les tips admissibles dans la main droite.
		List<DownMoveInfo> vitesseAdmissible = new List<DownMoveInfo> ();
		for(int j = (int)KinectPowerInterop.HandJointIndex.PINKY_TIP;
		    j <= (int)KinectPowerInterop.HandJointIndex.THUMB_TIP;
		    j += 3) {
			DownMoveInfo curDownMove = downMoveList[j];
			MettreAJourDownMoveInfo(curDownMove, j, vectVitesse);
		}

		// Trouver les tips admissibles dans la main gauche.
		for(int j = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS + (int)KinectPowerInterop.HandJointIndex.PINKY_TIP;
		    j <= (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS + (int)KinectPowerInterop.HandJointIndex.THUMB_TIP;
		    j += 3) {
			DownMoveInfo curDownMove = downMoveList[j];
			MettreAJourDownMoveInfo(curDownMove, j, vectVitesse);
		}

		List<DownMoveInfo> tipsAdmissible = new List<DownMoveInfo> ();
		// Afficher tous les doigts admissibles.
		for (int index = 0; index < (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * 2; ++index) {
			DownMoveInfo info = downMoveList[index];
			if (EstBoutDoigt(info.indexDoigt) && info.elapsedTime > timeDownMove) {
				tipsAdmissible.Add(info);
				//spheres[index].GetComponent<FingerSphere>().SetAllongementAssiste(true);
			}
		}

		KinectPowerInterop.HandJointIndex bestCase = KinectPowerInterop.HandJointIndex.NO_JOINT;
		float bestCasePercent = 0.0f;
		// Filtrer la liste des tips admissible pour garder seulement le pouce et le plus bas doigt
		for(int j= 0;j < tipsAdmissible.Count; j++) {
			DownMoveInfo downMove = tipsAdmissible[j];
			KinectPowerInterop.HandJointIndex curIndex = downMove.indexDoigt;
			if(curIndex == KinectPowerInterop.HandJointIndex.THUMB_TIP)
				continue;
			if(bestCase == KinectPowerInterop.HandJointIndex.NO_JOINT)
			{
				bestCase = downMove.indexDoigt;
				continue;
			}

			int intIndex = (int) curIndex;
			Vector3 posTip = TransformerPositionDoigt(hand_joints[intIndex]);
			Vector3 posMid = TransformerPositionDoigt(hand_joints[intIndex-1]);
			Vector3 posBase = TransformerPositionDoigt(hand_joints[intIndex-2]);
			float totalFingerLength = Vector3.Distance(posTip, posMid) + Vector3.Distance(posMid, posBase);

			float fingerVerticalLength = posTip.y - posBase.y;

			if(fingerVerticalLength > 0)
				tipsAdmissible.Remove(downMove);
			else
			{
				if(fingerVerticalLength / totalFingerLength > bestCasePercent)
				{
					bestCase = downMove.indexDoigt;
					bestCasePercent = fingerVerticalLength / totalFingerLength;
				}
				else
				{
					tipsAdmissible.Remove (downMove);
				}
			}
		}

		// Do the magic
		foreach(DownMoveInfo tip in tipsAdmissible) {
			//Debug.Log(tip.indexDoigt);
			spheres[(int)tip.indexDoigt].GetComponent<FingerSphere>().SetAllongementAssiste(true);
		}
		//Debug.Log("");
	}

	private void MettreAJourDownMoveInfo(DownMoveInfo curDownMove, int j, Vector3[] vectVitesse) {
		if (!EstBoutDoigt(curDownMove.indexDoigt))
			return;

		// Si le doigt a une vitesse suffisante et est pres des blanches.
		if(vectVitesse[j].y <= vitesseMinimale && DoigtApprocheBlanches(positionPrecedenteDoigts[j], hauteurMaximale))
		{
			// Noter la hauteur initiale du doigt.
			if (curDownMove.elapsedTime == 0) {
				curDownMove.hauteurInitiale = positionPrecedenteDoigts[j].y;
			}

			// Augmenter les timers disant que le doigt est admissible.
			curDownMove.elapsedTime += Time.deltaTime;
			curDownMove.elapsedTimeout = 0;
			curDownMove.elapsedTimeUp = 0;
		}
		// Si le doigt n'a pas une vitesse suffisante ou est trop loin.
		else
		{
			curDownMove.elapsedTimeout += Time.deltaTime;
			if (curDownMove.elapsedTimeout >= timeoutDownMove) {
				curDownMove.elapsedTime = 0;
			}
		}

		downMoveList [j] = curDownMove;
	}

	public Vector3 TransformerPositionDoigt(KinectPowerInterop.HandJointInfo handJointInfo) {
		// Mettre a jour l'autre overload en meme temps!
		return new Vector3(-handJointInfo.x * 45,
		                   -(-handJointInfo.z * 52 + 20),
		                   -handJointInfo.y * 45 - 5);
	}
	public Vector3 TransformerPositionDoigt(Vector3 position) {
		// Mettre a jour l'autre overload en meme temps!
		return new Vector3(-position.x * 45,
		                   -(-position.z * 52 + 20),
		                   -position.y * 45 - 5);
	}

	void Update () {
		// Changer la taille des mains.
		if (Input.GetButtonDown ("TailleMain1")) {
			KinectPowerInterop.SetHandMeasurements (kLargeur [0], kHauteur [0]);
		} else if (Input.GetButtonDown ("TailleMain2")) {
			KinectPowerInterop.SetHandMeasurements (kLargeur [1], kHauteur [1]);
		} else if (Input.GetButtonDown ("TailleMain3")) {
			KinectPowerInterop.SetHandMeasurements (kLargeur [2], kHauteur [2]);
		} else if (Input.GetButtonDown ("TailleMain4")) {
			KinectPowerInterop.SetHandMeasurements (kLargeur [3], kHauteur [3]);
		}

		// Mettre a jour la position des boules rouge en fonction des donnees de la caméra Creative.
		KinectPowerInterop.GetHandsSkeletons (hand_joints);

		DetecterVitesse ();

		// Placer des spheres sur les articulations et bouts de doigts.
		for (int i = 0; i < hand_joints.Length; ++i) {
			PlacerSphere(i);
		}

		// Placer les cylindres pour qu'ils relient les spheres.
		for (int i = 0; i < cylindres.Count; ++i) {
			PlacerCylindre(i);
		}

		// Verifier si du guidage est requis.
		for (int indexMain = 0; indexMain < 2; ++indexMain) {
			// Prendre la position de la base du doigt RING.
			int indexDeBaseRing = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
				(int)KinectPowerInterop.HandJointIndex.RING_BASE;
			if (hand_joints[indexDeBaseRing].error < kErreurMaxPermise) {
				Vector3 minBases;
				Vector3 maxBases;
				CalculerBoundingBoxMain(indexMain, false, out minBases, out maxBases);
				Vector3 minMain;
				Vector3 maxMain;
				CalculerBoundingBoxMain(indexMain, true, out minMain, out maxMain);

				int indexPalm = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
					(int)KinectPowerInterop.HandJointIndex.PALM;

				// Afficher le guidage par ordre de priorite: x, y, z.
				for (int i = 0; i < 3; ++i) {
					if (minBases[i] < kLimitesMin[i]) {
						flechesGuidage[indexMain].AfficherGuidage(i, 1, minMain, maxMain, TransformerPositionDoigt(hand_joints[indexPalm]));
					} else if (maxBases[i] > kLimitesMax[i]) {
						flechesGuidage[indexMain].AfficherGuidage(i, -1, minMain, maxMain, TransformerPositionDoigt(hand_joints[indexPalm]));
					}
				} 
			}
		}
	}

	// Indique si les 2 mains sont suffisamment pres du capteur pour qu'on
	// puisse dire leur position avec certitude. Aussi, il faut que les
	// mains aient la bonne orientation.
	public bool MainsSontVisibles() {
		for (int indexMain = 0; indexMain < 2; ++indexMain) {
			// Prendre la position de la base du doigt RING.
			int indexDeBaseRing = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
				(int)KinectPowerInterop.HandJointIndex.RING_BASE;
			if (hand_joints[indexDeBaseRing].error < kErreurMaxPermise) {
				Vector3 minBases;
				Vector3 maxBases;
				CalculerBoundingBoxMain(indexMain, false, out minBases, out maxBases);
				
				// Afficher le guidage par ordre de priorite: x, y, z.
				for (int i = 0; i < 3; ++i) {
					if (minBases[i] < kLimitesCapteurMin[i]) {
						return false;
					} else if (maxBases[i] > kLimitesCapteurMax[i]) {
						return false;
					}
				} 

				int indexPouce = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
					(int)KinectPowerInterop.HandJointIndex.THUMB_MID;
				int indexPinky = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
					(int)KinectPowerInterop.HandJointIndex.PINKY_MID;
				if (indexMain == kIndexMainGauche) {
					if (-hand_joints[indexPouce].x < -hand_joints[indexPinky].x)
						return false;
				} else if (indexMain == kIndexMainDroite) {
					if (-hand_joints[indexPouce].x > -hand_joints[indexPinky].x)
						return false;
				}

			} else {
				return false;
			}
		}
		return true;
	}

	// Indique si les 2 mains sont pres du piano, c'est a dire qu'il n'y a pas de
	// fleches de guidage requises.
	public bool MainsSontPresDuPiano() {
		for (int indexMain = 0; indexMain < 2; ++indexMain) {
			// Prendre la position de la base du doigt RING.
			int indexDeBaseRing = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS*indexMain +
				(int)KinectPowerInterop.HandJointIndex.RING_BASE;
			if (hand_joints[indexDeBaseRing].error < kErreurMaxPermise) {
				Vector3 minBases;
				Vector3 maxBases;
				CalculerBoundingBoxMain(indexMain, false, out minBases, out maxBases);
				
				// Afficher le guidage par ordre de priorite: x, y, z.
				for (int i = 0; i < 3; ++i) {
					if (minBases[i] < kLimitesMin[i]) {
						return false;
					} else if (maxBases[i] > kLimitesMax[i]) {
						return false;
					}
				} 
			} else {
				return false;
			}
		}
		return true;
	}

	private void CalculerBoundingBoxMain(int indexMain, bool avecDoigts, out Vector3 min, out Vector3 max) {
		min = new Vector3 (1000, 1000, 1000);
		max = new Vector3 (-1000, -1000, -1000);

		int indexMin = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * (indexMain) + 2;  // +1 pour eviter le bras, la paume.
		int indexMax = (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS * (indexMain + 1);
	
		for (int i = indexMin; i < indexMax; ++i) {
			KinectPowerInterop.HandJointIndex joint =
				(KinectPowerInterop.HandJointIndex)(i % (int)KinectPowerInterop.HandJointIndex.NUM_JOINTS);

			if (joint == KinectPowerInterop.HandJointIndex.THUMB_BASE) {
				continue;
			}
			if (!avecDoigts) {
				if (joint != KinectPowerInterop.HandJointIndex.RING_BASE &&
				    joint != KinectPowerInterop.HandJointIndex.INDEX_BASE &&
				    joint != KinectPowerInterop.HandJointIndex.MIDDLE_BASE) {
					continue;
				}
			}

			if (hand_joints[i].y < min.y)
				min.y = hand_joints[i].y;
			if (hand_joints[i].y > max.y)
				max.y = hand_joints[i].y;

			if (hand_joints[i].x < min.x)
				min.x = hand_joints[i].x;
			if (hand_joints[i].x > max.x)
				max.x = hand_joints[i].x;

			if (hand_joints[i].z < min.z)
				min.z = hand_joints[i].z;
			if (hand_joints[i].z > max.z)
				max.z = hand_joints[i].z;
		}

		min = TransformerPositionDoigt (min);
		max = TransformerPositionDoigt (max);
	}

	private void PlacerSphere(int index) {
		KinectPowerInterop.HandJointInfo jointInfo = hand_joints [index];

		KinectPowerInterop.HandJointIndex jointIndex;
		int indexMain;
		ObtenirJointEtNumeroMain(index, out jointIndex, out indexMain);

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

	private bool DoigtApprocheBlanches(Vector3 position, float tolerance) {
		return position.y < (kHauteurBlanches + kRayonDoigts + tolerance);
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

	// --- Snap ---

	// Hauteur a laquelle la main doit se trouver.
	//const float kHauteurCibleMain = -0.4f;
	const float kHauteurCibleMain = -1.1f;

	// Hauteur max pour le snap.
	//const float kHauteurMaxSnap = 2.0f;
	const float kHauteurMaxSnap = 1.0f;

	// Hauteur min pour le snap.
	//const float kHauteurMinSnap = -1.0f;
	const float kHauteurMinSnap = -2.0f;

}
