using UnityEngine;
using System.Collections;

public class PianoController : MonoBehaviour, InstrumentControllerInterface {

	// Englobe tout ce qui doit etre active/desactive quand le piano est active/desactive.
	public GameObject pianoWrapper;

	// Controleur du mode assiste.
	public AssistedModeControllerPiano assistedModeController;

	// Controleur des mains.
	public IntelHandController intelHandController;

	// Spotlight du piano.
	public SpotlightControl spotlightPiano;

	public void Prepare() {
		KinectPowerInterop.SetKinectAngle (15);
		spotlightPiano.SetTargetIntensity (kSpotlightIntensityPlaying, 1.0f);
	}

	public void PrepareToStop() {
		spotlightPiano.SetTargetIntensity (kSpotlightIntensityDefault, 1.0f);
	}

	// Methode appelee quand l'instrument "piano" est choisi.
	void OnEnable() {
		pianoWrapper.SetActive (true);

		// Affichage du guidage pour le geste du menu.
		// TODO: A faire une fois que le tutorial est termine.
		//GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.MENU_PRINCIPAL);

		// Activation de la reconnaissance du geste de menu.
		GestureRecognition gestureRecognition = GestureRecognition.ObtenirInstance ();
		gestureRecognition.AddGesture (new GestureMenu());

		// Demarrer le tutorial.
		tutorial = new TutorialPiano (intelHandController, assistedModeController);
		tutorial.Demarrer ();

		// (Temporaire) Partir le mode assisté.
		//assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\valse.txt", 4.0f);
	}
	
	// Methode appelee quand l'instrument "piano" n'est plus choisi.
	void OnDisable () {
		pianoWrapper.SetActive (false);

		// Desactiver le menu du mode assiste.
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();
		if (menuAssiste != null)
			menuAssiste.gameObject.SetActive (false);
		menuActif = false;
	}

	// Methode appelee a chaque frame quand le piano est l'instrument courant.
	void Update () {
		// Gérer les choix de l'utilisateur dans le menu.
		if (GererMenu ())
			return;
		
		// Afficher le menu.
		if (!menuActif && (
			Input.GetButtonDown ("MenuAssiste") ||
		    GestureRecognition.ObtenirInstance().GetCurrentGesture() == GestureId.GESTURE_MENU)) {
			AfficherMenu();
		} 
	}

	// Affiche le menu.
	private void AfficherMenu() {
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();

		// Positionner le menu du mode assiste.
		menuAssiste.transform.position = new Vector3(-13.40673f, 1.765814f, -13.54324f);
		menuAssiste.transform.eulerAngles = new Vector3(324.1f, 180f, 0);
		menuAssiste.transform.localScale = new Vector3(0.32f, 0.32f, 0.32f);
		
		// Mettre le texte dans les boutons du mode assiste.
		menuAssiste.AssignerTexte(0, "Retour aux", "instruments");
		menuAssiste.AssignerTexte(1, "Mode", "libre");
		menuAssiste.AssignerTexte(2, "Für", "Elise");
		menuAssiste.AssignerTexte(3, "Comptine", "d'été");
		menuAssiste.AssignerTexte(4, "Boubou", "the Boubou");
		
		// Desactive le piano.
		pianoWrapper.SetActive (false);
		
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
				// Quitter le piano.
				GameState.ObtenirInstance().AccederEtat(GameState.State.ChooseInstrument);
				assistedModeController.ActiverLibre();
				break;
			case 1:
				pianoWrapper.SetActive (true);
				assistedModeController.ActiverLibre();
				break;
			case 2:
				assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\fur_elise.txt", 1.0f);
				pianoWrapper.SetActive (true);
				break;
			case 3:
				assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\valse.txt", 4.0f);
				pianoWrapper.SetActive (true);
				break;
			case 4:
				assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\fur_elise.txt", 1.0f);
				pianoWrapper.SetActive (true);
				break;
			}
			
			if (boutonPresse != -1) {
				// Fermer le menu.
				MenuAssisteController.ObtenirInstance ().Cacher();
				menuActif = false;
				return true;
			}
		}

		return false;
	}

	// Indique si le menu est presentement affiche.
	private bool menuActif = false;

	// Tutorial.
	private TutorialPiano tutorial;

	// Intensité du spotlight piano par defaut.
	private const float kSpotlightIntensityDefault = 6.92f;

	// Intensité du spotlight piano quand on est au piano.
	private const float kSpotlightIntensityPlaying = 2.04f;
}
