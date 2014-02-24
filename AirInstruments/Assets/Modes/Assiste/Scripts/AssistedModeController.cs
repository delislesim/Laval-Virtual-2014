using UnityEngine;
using System.Collections;

public class AssistedModeController : MonoBehaviour {

	// GameObject de l'instrument controle par ce mode.
	public GameObject instrument;

	// GameObject de cubes tombants.
	public GameObject cubesTombants;

	void Start () {
		partition = new Partition ();
		partition.ChargerFichier ("C:\\piano\\scientist.txt");

		tempsDebut = Time.time;

		instrumentScript = (Instrument)(instrument.GetComponent (typeof(PianoBuilder)));
		cubesTombantsScript = (CubesTombants)(cubesTombants.GetComponent (typeof(CubesTombants)));
		cubesTombantsScript.AssignerTempsAvantDebutMusique (tempsAttendreDebutMusique * resolution * speed);
		cubesTombantsScript.AssignerInstrument (instrumentScript);

		// Remplir tout le tableau de prochaines notes avec des notes muettes.
		for (int i = 0; i < nombreEchantillons; ++i) {
			for (int j = 0; j < nombreNotes; ++j) {
				prochainesNotes[i, j] = Partition.StatutNote.Muette;
			}
		}
	}

	void Update () {
		// Temps actuel, en secondes.
		float tempsActuel = Time.time - tempsDebut * speed;

		// Temps actuel, en echantillons --> temps de remplissage.
		int tempsActuelEchantillons = (int)(tempsActuel / resolution);

		// Temps a jouer, en echantillons.
		int tempsAJouerEchantillons = tempsActuelEchantillons - (int)(tempsAttendreDebutMusique * speed);

		// Remplir des notes jusqu'au temps actuel.
		partition.RemplirProchainesNotes(tempsActuel,
		                                 prochainesNotes,
		                                 nombreEchantillons,
		                                 resolutionInverse,
		                                 cubesTombantsScript);

		// Faire avancer les cubes.
		cubesTombantsScript.AssignerTempsCourant (tempsActuel);

		// Jouer le temps actuel.
		if (tempsAJouerEchantillons < 0)
			return;

		for (int i = dernierTempsJoue; i < tempsAJouerEchantillons; ++i) {
			int tempsAJouerEchantillonsModulo = i % nombreEchantillons;

			// Passer toutes les notes.
			for (int j = 0; j < nombreNotes; ++j) {
				// Jouer la note si necessaire.
				Partition.StatutNote statutNote = prochainesNotes[tempsAJouerEchantillonsModulo, j];
				if (statutNote == Partition.StatutNote.Muette) {
					instrumentScript.DontPlayNotePlayer(j);
				} else if (statutNote == Partition.StatutNote.Accompagnement) {
					instrumentScript.PlayNoteOverride(j);
				} else {
					//instrumentScript.PlayNotePlayer(j);
					instrumentScript.PlayNoteOverride(j);
				}

				// Nettoyer le tableau.
				prochainesNotes[tempsAJouerEchantillonsModulo, j] = Partition.StatutNote.Muette;
			}
		}

		dernierTempsJoue = tempsAJouerEchantillons;
	}

	// Facteur pour jouer plus rapidement.
	private const float speed = 2.0f;

	// Temps a attendre avant de commencer a jouer la musique, en nombre d'echantillons.
	// Ceci correspond au decalage entre le remplissage et le jouage.
	private const int tempsAttendreDebutMusique = 40;

	// Dernier temps qu'on a joue (non inclusivement), en nombre d'echantillons.
	private int dernierTempsJoue = 0;

	// Tableau qui contient les prochaines notes a jouer.
	private Partition.StatutNote[,] prochainesNotes = new Partition.StatutNote[nombreEchantillons, nombreNotes];
	
	// Resolution du tableau de prochaines notes a jouer.
	private const float resolution = 0.1f;

	// Resolution inverse du tableau de prochaines notes a jouer.
	private const float resolutionInverse = 10.0f;

	// Nombre d'echantillons presents dans le tableau de prochaines notes a jouer.
	private const int nombreEchantillons = 120;

	// Nombre de notes de l'instrument controle par ce mode.
	private const int nombreNotes = 48;

	// Interface Instrument de l'instrument controle par ce mode.
	private Instrument instrumentScript;

	// Script du GameObject de cubes tombants.
	private CubesTombants cubesTombantsScript;

	// Temps auquel la musique a commence a jouer.
	private float tempsDebut;

	// Partition a jouer.
	private Partition partition;
}
