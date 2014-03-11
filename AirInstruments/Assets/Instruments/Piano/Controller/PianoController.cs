using UnityEngine;
using System.Collections;

public class PianoController : MonoBehaviour, InstrumentControllerInterface {

	// Englobe tout ce qui doit etre active/desactive quand le piano est active/desactive.
	public GameObject pianoWrapper;

	// Controleur du mode assiste.
	public AssistedModeControllerPiano assistedModeController;

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
	}
	
	// Methode appelee quand l'instrument "piano" n'est plus choisi.
	void OnDisable () {
		pianoWrapper.SetActive (false);

		// Desactiver le menu du mode assiste.
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();
		if (menuAssiste != null)
			menuAssiste.gameObject.SetActive (false);
		menuModeAssisteActif = false;
	}

	// Methode appelee a chaque frame quand le piano est l'instrument courant.
	void Update () {
		MenuAssisteController menuAssiste = MenuAssisteController.ObtenirInstance();

		// Si le menu du mode assiste est affiche, repondre aux choix de l'utilisateur.
		if (menuModeAssisteActif) {
			int boutonPresse = menuAssiste.ObtenirBoutonPresse();
			switch (boutonPresse) {
			case 0:
				// Quitter le piano.
				GameState.ObtenirInstance().AccederEtat(GameState.State.ChooseInstrument);
				break;
			case 1:
				Debug.Log("Mode Libre");
				pianoWrapper.SetActive (true);
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
				// Fermer le menu du mode assiste.
				MenuAssisteController.ObtenirInstance ().Cacher();
				menuModeAssisteActif = false;
				return;
			}
		}
		
		// Verifier si le mode assiste est demande.
		if (!menuModeAssisteActif && (
			Input.GetButtonDown ("MenuAssiste") ||
		    GestureRecognition.ObtenirInstance().GetCurrentGesture() == GestureId.GESTURE_MENU)) {

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
			menuModeAssisteActif = true;
		} 
	}

	// Indique si le menu du mode assiste est presentement affiche.
	private bool menuModeAssisteActif = false;

	// Intensité du spotlight piano par defaut.
	private const float kSpotlightIntensityDefault = 6.92f;

	// Intensité du spotlight piano quand on est au piano.
	private const float kSpotlightIntensityPlaying = 2.04f;
}
