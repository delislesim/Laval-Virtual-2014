﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrumController : MonoBehaviour, InstrumentControllerInterface {

	// Controleur du squelette du joueur de drum.
	public MoveJoints jointsController;

	//Dude 
	public GameObject dude;

	// Tete du joueur de drum, utilise pour controler la camera.
	public GameObject teteDrummer;

	// Camera principale du jeu.
	public Camera mainCamera;

	//Assisted Controller
	public DrumAssistedController assistedcontroller;

	// Spotlight du drum.
	public SpotlightControl spotDrum;

	// Effets de lumiere et de fumee du drum.
	public SpotCouleurDrumMaster spotCouleurDrum;

	// Composants du drum.
	public DrumComponent crash;
	public DrumComponent highHat;
	public DrumComponent ride;
	public DrumComponent snare;
	public DrumComponent tom1;
	public DrumComponent tom2;
	public DrumComponent tomBig;

	// Game object des composants du drum, incluant leur decoration.
	public DrumDecoration bassDecoration;
	public DrumDecoration crashDecoration;
	public DrumDecoration highHatDecoration;
	public DrumDecoration rideDecoration;
	public DrumDecoration snareDecoration;
	public DrumDecoration tom1Decoration;
	public DrumDecoration tom2Decoration;
	public DrumDecoration tomBigDecoration;

	// Bouts des baguettes.
	public GameObject tipLeft;
	public GameObject tipRight;

	// Mains.
	public GameObject leftHand;
	public GameObject rightHand;

	// Planes empechant les mains d'aller trop bas.
	public GameObject[] planesCollisions;

	// Indique si les planes de collisions sont actifs.
	private bool planesCollisionsActifs = false;

	// Clips audio du tutorial.
	public AudioClip sonPosition;
	public AudioClip sonTambours;
	public AudioClip sonImprovisez;
	public AudioClip sonMitrailler;
	public AudioClip sonMitraillerRide;

	// Clips audio du tutorial en anglais.
	public AudioClip sonAnglaisPosition;
	public AudioClip sonAnglaisTambours;
	public AudioClip sonAnglaisImprovisez;
	public AudioClip sonAnglaisMitrailler;
	public AudioClip sonAnglaisMitraillerRide;

	public void Prepare() {
		tutorialActif = false;

		// Desactiver les planes de collisions.
		SetPlanesCollisionsActive (false);

		// Diminuer un peu le spot.
		spotDrum.SetTargetIntensity (4.0f, 1.0f);

		// Allumer les spots psychedeliques.
		spotCouleurDrum.Allumer ();
	}

	private void SetPlanesCollisionsActive(bool active) {
		for (int i = 0; i < planesCollisions.Length; ++i) {
			planesCollisions[i].SetActive(active);
		}
		planesCollisionsActifs = active;
	}

	public void PrepareToStop() {
		tutorialActif = false;
		gameObject.SetActive (false);
		MenuAssisteController.ObtenirInstance ().Cacher ();
		Tutorial.ObtenirInstance ().gameObject.SetActive (false);
	}

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
		DrumAssistedController.DefinirInsance (assistedcontroller);

		dude.gameObject.SetActive (true);

		// Mettre la tete du drummer a la position de la camera.
		teteDrummer.transform.position = mainCamera.transform.position;
		Quaternion rotationCamera = mainCamera.transform.rotation;

		// Prendre le controle de la camera.
		mainCamera.transform.parent = teteDrummer.transform;
		mainCamera.transform.localPosition = Vector3.zero;
		mainCamera.transform.rotation = rotationCamera;

		// Demarrer le tutorial.
		tutorial = new TutorialDrum (tipLeft,
		                             tipRight,
		                             crash,
		                             highHat,
		                             ride,
		                             snare,
		                             tom1,
		                             tom2,
		                             tomBig,
		                             bassDecoration,
		                             crashDecoration,
		                             highHatDecoration,
		                             rideDecoration,
		                             snareDecoration,
		                             tom1Decoration,
		                             tom2Decoration,
		                             tomBigDecoration,
		                             sonPosition,
		                             sonTambours,
		                             sonImprovisez,
		                             sonMitrailler,
		                             sonMitraillerRide,
		                             sonAnglaisPosition,
		                             sonAnglaisTambours,
		                             sonAnglaisImprovisez,
		                             sonAnglaisMitrailler,
		                             sonAnglaisMitraillerRide);
		tutorial.Demarrer ();
		tutorialActif = true;

		// S'assurer que Move Joints est active.
		MoveJoints moveJoints = MoveJoints.ObtenirInstance ();
		if (moveJoints != null) {
			moveJoints.gameObject.SetActive(true);
		}
	}
	
	// Methode appelee quand l'instrument "drum" n'est plus choisi.
	void OnDisable () {
		dude.gameObject.SetActive (false);
		assistedcontroller.gameObject.SetActive (false);
	}
	
	// Methode appelee a chaque frame quand le drum est l'instrument courant.
	void Update () {
		// Activer les planes de collisions quand les mains sont assez hautes.
		const float kHauteurMainsMinimale = 0.77f;
		if (leftHand.transform.position.y > kHauteurMainsMinimale &&
		    rightHand.transform.position.y > kHauteurMainsMinimale) {
			SetPlanesCollisionsActive(true);
		}

		// Gerer la fin du tutorial.
		if (tutorialActif && tutorial.EstComplete ()) {

			assistedcontroller.gameObject.SetActive(true);

			// Activation du guidage
			GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.GUITARE_DRUM);
			
			// Activation de la reconnaissance du geste de menu.
			GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
			gestureRecognition.AddGesture (new GestureMenu());

			tutorialActif = false;
		}

		// Gérer les choix de l'utilisateur dans le menu.
		if (GererMenu ())
			return;

		// Afficher le menu.
		if (!menuActif && (
			Input.GetButtonDown ("MenuAssiste") ||
			(!tutorialActif && GestureRecognition.ObtenirInstance().GetCurrentGesture() == GestureId.GESTURE_MENU))) {
			AfficherMenu();
		}
	}

	// Affiche le menu.
	private void AfficherMenu() {
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();
		
		// Positionner le menu du mode assiste.
		Vector3 kPositionPourTeteDefaut = new Vector3(1.44f, 4.24f, -0.6465988f);
		Vector3 differenceAvecTeteDefaut = kPositionPourTeteDefaut - MoveJoints.kPositionTeteDefaut;

		menuAssiste.transform.position = mainCamera.transform.position + differenceAvecTeteDefaut;
		menuAssiste.transform.eulerAngles = new Vector3 (346.2f, 180f, 0);
		menuAssiste.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

		// Desactiver Move joints.
		MoveJoints moveJoints = MoveJoints.ObtenirInstance ();
		if (moveJoints != null) {
			moveJoints.gameObject.SetActive(false);
		}

		// Mettre le texte dans les boutons du mode assiste.
		menuAssiste.DesactiverTousBoutons ();
		menuAssiste.AssignerTexte(0, "Retour aux", "instruments");
		menuAssiste.AssignerTexte(1, "Mode", "libre");
		menuAssiste.AssignerTexte(2, "Mode", "assiste");
		
		// Activer le menu du mode assiste.
		menuAssiste.Afficher();
		
		// Se rappeler que le menu est active.
		menuActif = true;
	}

	// Gere les choix de l'utilisateur dans le menu assiste. Retourne
	// vrai si un choix est fait, faux sinon.
	private bool GererMenu() {
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();
		
		// Si le menu du mode assiste est affiche, repondre aux choix de l'utilisateur.
		if (menuActif) {
			int boutonPresse = menuAssiste.ObtenirBoutonPresse();
			
			switch (boutonPresse) {
			case 0:
				// Quitter drum.
				GameState.ObtenirInstance().AccederEtat(GameState.State.ChooseInstrument);
				break;
			case 1:
				// Mode libre.
				assistedcontroller.gameObject.SetActive(false);
				break;
			case 2:
				// Assiste.
				assistedcontroller.gameObject.SetActive(true);
				break;
			}
			
			if (boutonPresse != -1) {
				// Fermer le menu.
				MenuAssisteController.ObtenirInstance ().Cacher();

				// Reactiver move joints.
				MoveJoints moveJoints = MoveJoints.ObtenirInstance ();
				if (moveJoints != null) {
					moveJoints.gameObject.SetActive(true);
				}

				menuActif = false;
				return true;
			}
		}
		
		return false;
	}

	// Indique si le tutorial est actif.
	public static bool TutorialActif() {
		return tutorialActif;
	}

	// Indique si le menu est presentement affiche.
	private bool menuActif = false;

	// Tutorial.
	TutorialDrum tutorial;

	// Indique que le tutorial est en cours.
	static bool tutorialActif = false;
}
