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

	// Sons du tutorial.
	public AudioClip sonCapteur;
	public AudioClip sonPosition;
	public AudioClip sonGamme;
	public AudioClip sonAssiste;

	// Sons du tutorial en anglais.
	public AudioClip sonAnglaisCapteur;
	public AudioClip sonAnglaisPosition;
	public AudioClip sonAnglaisGamme;
	public AudioClip sonAnglaisAssiste;

	public void Prepare() {
		spotlightPiano.SetTargetIntensity (kSpotlightIntensityPlaying, 1.0f);

		// Fermer les spots multicolores.
		SpolightFou.SetActive (false);
	}

	public void PrepareToStop() {
		Tutorial.ObtenirInstance ().gameObject.SetActive (false);
		spotlightPiano.SetTargetIntensity (kSpotlightIntensityDefault, 1.0f);
		assistedModeController.ActiverLibre ();
		MenuAssisteController.ObtenirInstance ().Cacher ();
	}

	// Methode appelee quand l'instrument "piano" est choisi.
	void OnEnable() {
		menuActif = false;

		pianoWrapper.SetActive (true);

		// Demarrer le tutorial.
		tutorial = new TutorialPiano (intelHandController,
		                              assistedModeController,
		                              sonCapteur,
		                              sonPosition,
		                              sonGamme,
		                              sonAssiste,
		                              sonAnglaisCapteur,
		                              sonAnglaisPosition,
		                              sonAnglaisGamme,
		                              sonAnglaisAssiste);
		tutorial.Demarrer ();
		tutorialActif = true;
	}
	
	// Methode appelee quand l'instrument "piano" n'est plus choisi.
	void OnDisable () {
		pianoWrapper.SetActive (false);
	}

	// Methode appelee a chaque frame quand le piano est l'instrument courant.
	void Update () {
		// Gerer la fin du tutorial.
		if (tutorialActif && tutorial.EstComplete()) {
			// Affichage du guidage pour le geste du menu.
			GuidageController.ObtenirInstance ().changerGuidage(typeGuidage.PIANO);

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
		menuAssiste.transform.position = new Vector3(-13.40673f, 1.765814f, -13.54324f);
		menuAssiste.transform.eulerAngles = new Vector3(324.1f, 180f, 0);
		menuAssiste.transform.localScale = new Vector3(0.32f, 0.32f, 0.32f);
		
		// Mettre le texte dans les boutons du mode assiste.
		menuAssiste.DesactiverTousBoutons ();
		menuAssiste.AssignerTexte(0, "Retour aux", "instruments");
		menuAssiste.AssignerTexte(1, "Mode", "libre");
		menuAssiste.AssignerTexte(3, "Für", "Elise");
		menuAssiste.AssignerTexte(4, "Comptine", "d'été");
		
		// Desactive le piano.
		pianoWrapper.SetActive (false);
		
		// Activer le menu du mode assiste.
		menuAssiste.Afficher();
		
		// Se rappeler que le menu est active.
		menuActif = true;
	}

	// Calcule les moyennes des hauteurs des coups
	// pour permettre de calibrer le snap ultérieurement.
	private void CalculerMoyennes()
	{

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
			case 3:
				assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\fur_elise.txt", 1.0f);
				pianoWrapper.SetActive (true);
				break;
			case 4:
				assistedModeController.ChargerPartition (".\\Assets\\Modes\\Assiste\\Piano\\partitions\\valse.txt", 4.0f);
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

	// Indique si un tutorial est en cours.
	private bool tutorialActif = false;

	// Tutorial.
	private TutorialPiano tutorial;

	// Intensité du spotlight piano par defaut.
	private const float kSpotlightIntensityDefault = 8.0f;

	// Intensité du spotlight piano quand on est au piano.
	private const float kSpotlightIntensityPlaying = 2.04f;
}
