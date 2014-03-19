using UnityEngine;
using System.Collections;

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

	// Composants du drum.
	public DrumComponent crash;
	public HighHatComponent highHat;
	public DrumComponent ride;
	public DrumComponent snare;
	public DrumComponent tom1;
	public DrumComponent tom2;
	public DrumComponent tomBig;

	// Bouts des baguettes.
	public GameObject tipLeft;
	public GameObject tipRight;

	// Clips audio du tutorial.
	public AudioClip sonPosition;
	public AudioClip sonTambours;
	public AudioClip sonImprovisez;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (4);
	}

	public void PrepareToStop() {
		gameObject.SetActive (false);
		MenuAssisteController.ObtenirInstance ().Cacher ();
		Tutorial.ObtenirInstance ().gameObject.SetActive (false);
	}

	// Methode appelee quand l'instrument "drum" est choisi.
	void OnEnable() {
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
		                             sonPosition,
		                             sonTambours,
		                             sonImprovisez);
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
				assistedcontroller.gameObject.SetActive(true);
				break;
			case 2:
				// Assiste.
				assistedcontroller.gameObject.SetActive(false);
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

	// Indique si le menu est presentement affiche.
	private bool menuActif = false;

	// Tutorial.
	TutorialDrum tutorial;

	// Indique que le tutorial est en cours.
	bool tutorialActif = true;
}
