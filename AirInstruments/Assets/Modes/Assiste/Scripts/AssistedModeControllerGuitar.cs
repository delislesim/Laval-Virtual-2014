using UnityEngine;
using System.Collections;

public class AssistedModeControllerGuitar : MonoBehaviour {


	void Start () {
		partition = new PartitionGuitar ();
		partition.ChargerFichier(".\\Assets\\Modes\\Assiste\\Chanons\\01 Lonely Boy.txt");

		tempsDebut = Time.time;

		// Remplir tout le tableau de prochaines notes avec des notes muettes.
		for (int i = 0; i < nombreEchantillons; ++i) {
			for (int j = 0; j < nombreNotes; ++j) {
				prochainesNotes[i, j] = PartitionGuitar.StatutNoteGuitar.Muette;
			}
		}
	}

	void Update () {
		return;
		// Temps actuel, en secondes. Utilise pour remplir le tableau de prochaines notes.
		float tempsActuel = (Time.time - tempsDebut) * speed;

		// Temps de la note qu'on entend, en secondes.
		float tempsAJouer = tempsActuel - (tempsAttendreDebutMusique * speed);

		// Temps a jouer, en echantillons.
		int tempsAJouerEchantillons = (int) (tempsAJouer * resolutionInverse);

		// Remplir des notes jusqu'au temps actuel.
		partition.RemplirProchainesNotes(tempsActuel,
		                                 prochainesNotes,
		                                 nombreEchantillons,
		                                 resolutionInverse);
		// Jouer le temps actuel.
		if (tempsAJouerEchantillons >= 0) {
			for (int i = dernierTempsJoue; i < tempsAJouerEchantillons; ++i) {
				int tempsAJouerEchantillonsModulo = i % nombreEchantillons;

				// Passer toutes les notes.
				for (int j = 0; j < nombreNotes; ++j) {
					// Jouer la note si necessaire.
					PartitionGuitar.StatutNoteGuitar statutNote = prochainesNotes [tempsAJouerEchantillonsModulo, j];

					// Nettoyer le tableau.
					prochainesNotes [tempsAJouerEchantillonsModulo, j] = PartitionGuitar.StatutNoteGuitar.Muette;
				}
			}

			dernierTempsJoue = tempsAJouerEchantillons;
		}
		
		// Gerer les notes qui doivent etre jouees.
		for (int indexNote = 0; indexNote < nombreNotes; ++indexNote) {
			// TODO
		}
	}

	// Facteur pour jouer plus rapidement.
	private const float speed = 1.0f;

	// Temps a attendre avant de commencer a jouer la musique, en secondes.
	// Ceci correspond au decalage entre le remplissage et le jouage.
	private const float tempsAttendreDebutMusique = 4.0f;

	// Dernier temps qu'on a joue (non inclusivement), en nombre d'echantillons.
	private int dernierTempsJoue = 0;

	// Tableau qui contient les prochaines notes a jouer.
	private PartitionGuitar.StatutNoteGuitar[,] prochainesNotes = new PartitionGuitar.StatutNoteGuitar[nombreEchantillons, nombreNotes];
	
	// Resolution du tableau de prochaines notes a jouer.
	private const float resolution = 0.1f;

	// Resolution inverse du tableau de prochaines notes a jouer.
	private const float resolutionInverse = 10.0f;

	// Nombre d'echantillons presents dans le tableau de prochaines notes a jouer.
	private const int nombreEchantillons = (int)(120 * speed);

	// Nombre de notes de l'instrument controle par ce mode.
	private const int nombreNotes = 48;

	// Temps auquel la musique a commence a jouer.
	private float tempsDebut;

	// Partition a jouer.
	private PartitionGuitar partition;
}
