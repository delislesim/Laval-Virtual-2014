using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KinectHelpers;

public class Pointeur : MonoBehaviour {

	// Images de main, a plusieurs stades de remplissage.
	public List<Texture> imagesMain;
	private Texture[] chargement = new Texture[12];

	public Pointeur() {
		// Singleton.
		instance = this;
	}

	void Start(){
		//Loader les ressources
		for (int i = 0; i < chargement.Length; i++) {
			string imageChargement;
			int indeximageChargment = i + 1;
			imageChargement = indeximageChargment + "_12";
			chargement [i] = (Texture)Resources.Load ("ChargementAnimation/" + imageChargement);
		}
	}

	void OnEnable() {
		kalman.SetInitialObservation (Vector4.zero);
		timerChangerMain = 0;
	}

	public static Pointeur obtenirInstance() {
		return instance;
	}

	public void RemoveAllTargets() {
		targets.Clear ();
		indexCibleActuelle = kIndexCibleInvalide;
		tempsCibleActuelle = 0;
		indexImageMain = 0;
	}

	public void AddTarget(int targetId,
	                      Vector2 targetCenter,
	                      Vector2 targetSize) {
		Target target = new Target ();
		target.id = targetId;
		target.center = targetCenter;
		target.size = targetSize;
		targets.Add (target);
	}

	public int GetCurrentTargetId() {
		int decalageBoutonRetour = boutonRetourPresent ? 0 : -1;

		if (Input.GetButtonDown ("ChoixMenu1")) {
			if (targets.Count > 1 + decalageBoutonRetour)
				return targets[1 + decalageBoutonRetour].id;
		} else if (Input.GetButtonDown("ChoixMenu2")) {
			if (targets.Count > 2)
				return targets[2 + decalageBoutonRetour].id;
		} else if (Input.GetButtonDown("ChoixMenu3")) {
			if (targets.Count > 3 + decalageBoutonRetour)
				return targets[3 + decalageBoutonRetour].id;
		} else if (Input.GetButtonDown("ChoixMenu4")) {
			if (targets.Count > 4 + decalageBoutonRetour)
				return targets[4 + decalageBoutonRetour].id;
		}

		if (indexCibleActuelle != kIndexCibleInvalide &&
		    tempsCibleActuelle > kTempsCibleChoisie) {
			return targets[indexCibleActuelle].id;
		}
		return kIndexCibleInvalide;
	}

	void Update () {
		if (!skeleton.IsDifferent())
			return;
		
		Vector3 positionShoulders;
		Skeleton.JointStatus status = skeleton.GetJointPosition (Skeleton.Joint.SpineShoulder, out positionShoulders);

		if (status == Skeleton.JointStatus.NotTracked)
			return;

		Vector3 positionHandLeft;
		skeleton.GetJointPosition (Skeleton.Joint.HandLeft, out positionHandLeft);
		Vector3 positionHandRight;
		skeleton.GetJointPosition (Skeleton.Joint.HandRight, out positionHandRight);

		float avancementMainGauche = positionShoulders.z - positionHandLeft.z;
		float avancementMainDroite = positionShoulders.z - positionHandRight.z;

		// Si la main actuelle a trop reculee, on ne la considere plus active.
		if (mainActive == MainActive.GAUCHE && avancementMainGauche < kDistanceActive) {
			mainActive = MainActive.AUCUNE;
		}
		if (mainActive == MainActive.DROITE && avancementMainDroite < kDistanceActive) {
			mainActive = MainActive.AUCUNE;
		}

		// Si aucune main n'est active, on doit en choisir une.
		//if (mainActive == MainActive.AUCUNE) {
			if (avancementMainGauche > avancementMainDroite) {
				if (avancementMainGauche > kDistanceActive) {
					timerChangerMain += Time.deltaTime;
					mainActive = MainActive.GAUCHE;
					timerChangerMain = 0;
				}
			} else {
				if (avancementMainDroite > kDistanceActive) {
					timerChangerMain += Time.deltaTime;
					mainActive = MainActive.DROITE;
					timerChangerMain = 0;
				}
			}
		//}

		// S'il n'y a toujours aucune main active, on abandonne.
		if (mainActive == MainActive.AUCUNE)
			return;

		// Determiner la position de la main active.
		Vector2 positionMain;
		Vector2 positionCentre;
		float avancement;

		if (mainActive == MainActive.GAUCHE) {
			Vector3 positionMain3d;
			skeleton.GetJointPosition(Skeleton.Joint.HandLeft, out positionMain3d);
			Vector3 positionEpaule;
			skeleton.GetJointPosition(Skeleton.Joint.ShoulderLeft, out positionEpaule);

			positionMain.x = positionMain3d.x;
			positionMain.y = positionMain3d.y;

			positionCentre.x = positionEpaule.x - kDistanceHorizontalCentre;
			positionCentre.y = positionEpaule.y;

			avancement = avancementMainGauche;
		} else {
			Vector3 positionMain3d;
			skeleton.GetJointPosition(Skeleton.Joint.HandRight, out positionMain3d);
			Vector3 positionEpaule;
			skeleton.GetJointPosition(Skeleton.Joint.ShoulderRight, out positionEpaule);
			
			positionMain.x = positionMain3d.x;
			positionMain.y = positionMain3d.y;
			
			positionCentre.x = positionEpaule.x + kDistanceHorizontalCentre;
			positionCentre.y = positionEpaule.y;

			avancement = avancementMainDroite;
		}

		// Position du pointeur.
		Vector2 coinHautGauche = positionCentre - new Vector2 (kLargeur / 2.0f, kHauteur / 2.0f);
		Vector2 positionAbsolue = positionMain - coinHautGauche;
		Vector4 handPositionTmp = new Vector4 (positionAbsolue.x / kLargeur, positionAbsolue.y / kHauteur, 0, 0);
		Vector4 handPositionSmooth = kalman.Update (handPositionTmp);
		handPosition.x = handPositionSmooth.x;
		handPosition.y = 1.0f - handPositionSmooth.y;
		
		// Verifier si on clique sur une cible.
		float pressExtent = (avancement - kDistanceActive) * 15.0f;

		// Si la main est deja sur une cible, augmenter son compteur.
		if (indexCibleActuelle != kIndexCibleInvalide) {
			// Verifier si on est encore sur la cible.
			Target target = targets[indexCibleActuelle];
			if (!EstPresDeCible(target)) {
				indexCibleActuelle = kIndexCibleInvalide;
				tempsCibleActuelle = 0;
				indexImageMain = 0;
				return;
			}

			// Augmenter le compteur de la cible.
			tempsCibleActuelle += Time.deltaTime;

			// Determiner quelle image de main afficher.
			//indexImageMain = (int)(tempsCibleActuelle * imagesMain.Count / kTempsCibleChoisie);
			indexImageMain = (int)(tempsCibleActuelle * chargement.Length / kTempsCibleChoisie);
			if (indexImageMain >= chargement.Length) {
				indexImageMain = chargement.Length - 1;
			}
		} else {
			// Trouver une nouvelle cible.
			for (int i = 0; i < targets.Count; ++i) {
				if (EstPresDeCible(targets[i])) {
					indexCibleActuelle = i;
				}
			}
		}
	}

	bool EstPresDeCible(Target target) {
		Vector2 distances = target.center - handPosition;
		if (Mathf.Abs(distances.x) > target.size.x ||
		    Mathf.Abs(distances.y) > target.size.y) {
			return false;
		}
		return true;
	}
	
	void OnGUI () {
		if (mainActive != MainActive.AUCUNE) {
			// S'assurer qu'on est en avant de tout.
			// (Petite valeur = plus vers l'avant)
			GUI.depth = 0;

			// Dessiner la main.
			float largeur = 100 * Screen.width / 1080;
			float hauteur = 100 * Screen.height / 768;
			GUI.BeginGroup(new Rect (handPosition.x * Screen.width - largeur,
			                         handPosition.y * Screen.height + hauteur,
			                         largeur,
			                         hauteur));
			GUI.DrawTexture (new Rect (largeur/4, hauteur/4, largeur/2, hauteur/2), imagesMain[0], ScaleMode.ScaleToFit);
			if(indexImageMain >= 1)
				GUI.DrawTexture(new Rect (0, 0, largeur, hauteur), chargement[indexImageMain - 1], ScaleMode.ScaleToFit);
			GUI.EndGroup();
		}
	}

	public void SetBoutonRetourPresent(bool boutonRetourPresent) {
		this.boutonRetourPresent = boutonRetourPresent;
	}

	// Structure décrivant une cible potentiel pour le pointeur.
	private struct Target {
			public int id;
			public Vector2 center;
			public Vector2 size;
	}

	// Unique instance du pointeur dans le jeu.
	private static Pointeur instance;

	// Squelette.
	private Skeleton skeleton = new Skeleton(0);

	// Distance pour qu'une main soit consideree active.
	private const float kDistanceActive = 0.15f;

	// Distance horizontale entre l'epaule et le "centre de l'ecran".
	private const float kDistanceHorizontalCentre = 0f;

	// Largeur pouvant etre parcourue par une main.
	private const float kLargeur = 0.9f;

	// Hauteur pouvant etre parcourue par une main.
	private const float kHauteur = 0.55f;

	// Indique quelle main est active.
	enum MainActive {
		GAUCHE,
		DROITE,
		AUCUNE
	}
	private MainActive mainActive = MainActive.AUCUNE;

	// Indique si le bouton de retour au menu est affiche a l'ecran.
	private bool boutonRetourPresent = false;

	// Index de la cible actuelle dans le tableau de cibles.
	private int indexCibleActuelle = kIndexCibleInvalide;

	// Temps passé sur la cible actuelle.
	private float tempsCibleActuelle = 0;

	// Index de l'image de main a afficher.
	private int indexImageMain = 0;

	// Liste de cibles sur lesquelles la main peut "cliquer".
	private List<Target> targets = new List<Target> ();

	// Position de la main.
	private Vector2 handPosition = new Vector2();

	// Index de cible invalide.
	private const int kIndexCibleInvalide = -1;

	// Temps nécessaire pour qu'une cible soit considérée choisie.
	private const float kTempsCibleChoisie = 1.15f;

	// Kalman pour le pointeur.
	private Kalman kalman = new Kalman (5.0f);

	// Timer pour changer de main.
	private float timerChangerMain = 0;

	// Temps pour changer de main.
	private float kTempsChangerMain = 0.25f;
}
