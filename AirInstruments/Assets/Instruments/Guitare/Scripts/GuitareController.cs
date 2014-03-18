using UnityEngine;
using System.Collections;

public class GuitareController : MonoBehaviour, InstrumentControllerInterface {

	// Wrapper de tout ce qui doit etre active pour la guitare.
	public GameObject guitareWrapper;

	// joints unity
	public Joints jointsObject;

	//Objet de guitare
	public Transform GuitarContainer;

	// Giga spot a allumer pour switcher la guitare.
	public Light gigaSpotGuitare;

	// Guitare decorative, pour le menu de choix d'instrument.
	public GameObject guitareDecorative;

	// Controleur du mode assiste.
	public AssistedModeControllerGuitar assistedModeController;

	// Joueur de guitare.
	public GuitarPlayer guitarPlayer;

	// Hand follower.
	public HandFollower handFollower;

	// Spot de la guitare.
	public SpotlightControl spotGuitare;

	// Spot un peu plus haut pointant sur le visage du guitariste.
	public SpotlightControl spotGuitareHaut;

	// Pied gauche du joueur.
	public GameObject leftFoot;

	// Sons du tutorial.
	public AudioClip sonLeverBras;
	public AudioClip sonCordes;
	public AudioClip sonAssiste;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (10);
		tempsPreparation = 0;
		aFaitSwitch = false;
		estEnTrainDeQuitter = false;
		gameObject.SetActive (true);
	}

	public void PrepareToStop() {
		tempsPreparation = -4.0f;
		aFaitSwitch = false;
		estEnTrainDeQuitter = true;

		MenuAssisteController.ObtenirInstance ().Cacher ();
		Tutorial.ObtenirInstance ().gameObject.SetActive (false);
	}

	public void Update() {
		// Gerer la fin du tutorial.
		if (tutorialActif && tutorial.EstComplete()) {
			// Affichage du guidage pour le geste du menu.
			GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.INSTRUMENTS);
			
			// Activation de la reconnaissance du geste de menu.
			GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
			gestureRecognition.AddGesture (new GestureMenu());
			
			tutorialActif = false;
		}

		// Faire l'animation du gigaspot qui permet de switcher la guitare subtilement.
		tempsPreparation += Time.deltaTime;
		if (tempsPreparation > 0) {
			if (tempsPreparation < 0.5f) {
				gigaSpotGuitare.intensity += 16.0f * Time.deltaTime;
			} else if (tempsPreparation < 2.0f) {
				gigaSpotGuitare.intensity -= 5.33f * Time.deltaTime;
			} else if (gigaSpotGuitare.intensity != 0) {
				gigaSpotGuitare.intensity = 0;
			}

			// Switcher la guitare decorative / controlee par le joueur.
			if (tempsPreparation > 0.5f && !aFaitSwitch) {
				if (estEnTrainDeQuitter) {
					guitareWrapper.SetActive (false);
					guitareDecorative.SetActive (true);

					joueurVisible = false;

					// Reinitialiser le spot au-dessus de la guitare.
					spotGuitare.Reinitialiser();
					spotGuitareHaut.Reinitialiser();
				} else {
					guitareWrapper.SetActive (true);
					guitareDecorative.SetActive (false);

					joueurVisible = true;

					// Rapetisser l'angle du spot au-dessus de la guitare.
					spotGuitare.SetAngle(24.0f, 8.0f);
					spotGuitareHaut.SetTargetIntensity(3.32f, 2.0f);
				}
				aFaitSwitch = true;
			}
		}

		// Bouger le spot vers le guitariste.
		if (joueurVisible && leftFoot.renderer.enabled) {
			spotGuitare.SetLookAt(leftFoot.transform.position, 10.0f);
		}

		// Les autres options ne sont pas disponibles tant qu'on a pas fini
		// l'animation ou quand on est en train de quitter.
		if (!aFaitSwitch  || estEnTrainDeQuitter) {
			return;
		}

		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();

		// Bras dans les airs == afficher le menu.
		if (!menuModeAssisteActif && !doitAfficherChoixGamme && !menuChoixGammeActif && (
			Input.GetButtonDown ("MenuAssiste") ||
			GestureRecognition.ObtenirInstance().GetCurrentGesture() == GestureId.GESTURE_MENU)) {

			// Positionner le menu du mode assiste.
			//menuAssiste.transform.position = new Vector3(7.020516f, 7.6f, -16.13901f);
			menuAssiste.transform.position = new Vector3(7.02124f, 9.326828f, -16.13635f);
			menuAssiste.transform.eulerAngles = new Vector3(0.09139769f, 15.2244f, 0);
			menuAssiste.transform.localScale = new Vector3(1.48f, 1.48f, 1.2f);

			// Mettre le texte dans les boutons du mode assiste.
			menuAssiste.DesactiverTousBoutons ();
			menuAssiste.AssignerTexte(0, "Retour aux", "instruments");
			menuAssiste.AssignerTexte(1, "Mode", "libre");
			menuAssiste.AssignerTexte(3, "Lonely", "Boy");
			menuAssiste.AssignerTexte(4, "TNT", "");

			// Desactiver le son de la guitare.
			// TODO

			// Changer un peu le FOV.
			CameraController.ObtenirInstance().ForcerFieldOfView(70.2f);

			// Activer le menu du mode assiste.
			menuAssiste.Afficher();

			// Decaler le pointeur.
			Pointeur.obtenirInstance().AppliquerDecalageVertical(-0.4f);
			
			// Se rappeler que le menu est active.
			menuModeAssisteActif = true;
		}

		// Afficher le menu de choix de gamme.
		if (doitAfficherChoixGamme && !menuAssiste.gameObject.activeSelf) {
			// Positionner le menu du choix de gamme.
			menuAssiste.transform.position = new Vector3(7.02124f, 9.326828f, -16.13635f);
			menuAssiste.transform.eulerAngles = new Vector3(0.09139769f, 15.2244f, 0);
			menuAssiste.transform.localScale = new Vector3(1.48f, 1.48f, 1.2f);

			// Mettre le texte dans les boutons du choix de gamme.
			menuAssiste.DesactiverTousBoutons ();
			menuAssiste.AssignerTexte(5, "Gamme", "Majeure");
			menuAssiste.AssignerTexte(6, "Gamme", "Mineure");
			menuAssiste.AssignerTexte(3, "Gamme", "Pentatonique");
			menuAssiste.AssignerTexte(4, "Gamme", "Blues");

			// Activer le menu du mode assiste.
			menuAssiste.Afficher();

			// Se rappeler que le menu est active.
			menuChoixGammeActif = true;
			doitAfficherChoixGamme = false;
		}

		// Traiter les choix de l'utilisateur dans le menu du mode assisté.
		if (menuModeAssisteActif) {
			int boutonPresse = menuAssiste.ObtenirBoutonPresse();
			switch (boutonPresse) {
			case 0:
				// Quitter la guitare.
				GameState.ObtenirInstance().AccederEtat(GameState.State.ChooseInstrument);
				break;
			case 1:
				assistedModeController.StopSong();
				doitAfficherChoixGamme = true;
				break;
			case 3:
				assistedModeController.StartSong(AssistedModeControllerGuitar.Chanson.LONELY_BOY);
				CameraController.ObtenirInstance().ForcerFieldOfView(CameraController.kFovGuitare);
				break;
			case 4:
				assistedModeController.StartSong(AssistedModeControllerGuitar.Chanson.TNT);
				CameraController.ObtenirInstance().ForcerFieldOfView(CameraController.kFovGuitare);
				break;
			}
			
			if (boutonPresse != -1) {
				// Fermer le menu du mode assiste.
				MenuAssisteController.ObtenirInstance ().Cacher();
				menuModeAssisteActif = false;
				return;
			}
		}

		// Traiter les choix de l'utilisateur dans le menu de gamme.
		if (menuChoixGammeActif) {

			int boutonPresse = menuAssiste.ObtenirBoutonPresse();
			switch (boutonPresse) {
			case 5:
				guitarPlayer.SetScaleModeAndTone(GuitarPlayer.Mode.MAJOR, GuitarPlayer.Tone.E);
				break;
			case 6:
				guitarPlayer.SetScaleModeAndTone(GuitarPlayer.Mode.MINOR, GuitarPlayer.Tone.E);
				break;
			case 3:
				guitarPlayer.SetScaleModeAndTone(GuitarPlayer.Mode.PENT, GuitarPlayer.Tone.E);
				break;
			case 4:
				guitarPlayer.SetScaleModeAndTone(GuitarPlayer.Mode.BLUES, GuitarPlayer.Tone.E);
				break;
			}
			
			if (boutonPresse != -1) {
				// Retablir le zoom.
				CameraController.ObtenirInstance().ForcerFieldOfView(CameraController.kFovGuitare);
				// Fermer le menu du mode assiste.
				MenuAssisteController.ObtenirInstance ().Cacher();
				menuChoixGammeActif = false;
				return;
			}
		}
	}

	// Methode appelee quand l'instrument "guitare" est choisi.
	void OnEnable() {
		// Reinitialiser tous les bools.
		tutorialActif = false;
		aFaitSwitch = false;
		joueurVisible = false;
		estEnTrainDeQuitter = false;
		menuModeAssisteActif = false;
		doitAfficherChoixGamme = false;
		menuChoixGammeActif = false;
	}

	public void AnimationTerminee() {
		// Demarrer le tutorial.
		tutorial = new TutorialGuitare (sonLeverBras,
		                                sonCordes,
		                                sonAssiste,
		                                handFollower,
		                                assistedModeController);
		tutorial.Demarrer ();
		tutorialActif = true;	
	}

	// Methode appelee quand l'instrument "guitare" n'est plus choisi.
	void OnDisable () {
		// Faire le switch de guitare au cas ou il n'a pas deja ete fait.
		guitareWrapper.SetActive (false);
		guitareDecorative.SetActive (true);

		// Reinitialiser le mode assiste.
		assistedModeController.StopSong ();
	}

	// Indique si le joueur est visible, c'est a dire que le switch
	// de guitare a ete fait.
	public static bool JoueurEstVisible() {
		return joueurVisible && !menuModeAssisteActif && !menuChoixGammeActif;
	}

	// Tutorial.
	private TutorialGuitare tutorial;

	// Indique si le tutorial est en cours.
	private bool tutorialActif = false;

	// Temps depuis que le mode a commencé a se préparer.
	private float tempsPreparation;

	// Indique si on a fait le switch de guitare.
	private bool aFaitSwitch = false;

	// Indique si le joueur de guitare est visible.
	private static bool joueurVisible = false;

	// Indique si on est en train de quitter la scene.
	private bool estEnTrainDeQuitter = false;

	// Indique si le menu du mode assiste est affiche.
	private static bool menuModeAssisteActif = false;

	// Indique si on doit afficher le menu de choix de gamme.
	private bool doitAfficherChoixGamme = false;

	// Indique si le menu de choix de gamme est affiche.
	private static bool menuChoixGammeActif = false;

}
