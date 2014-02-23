using UnityEngine;
using System.Collections;

public class AssistedModeController : MonoBehaviour {

	// GameObject de l'instrument controle par ce mode.
	public GameObject instrument;


	void Start () {
		partition = new Partition ();
		partition.ChargerFichier ("C:\\piano\\scientist.txt");

		tempsDebut = Time.time;

		instrumentScript = (Instrument)(instrument.GetComponent (typeof(PianoBuilder)));

		// Remplir tout le tableau de prochaines notes avec des notes muettes.
		for (int i = 0; i < nombreEchantillons; ++i) {
			for (int j = 0; j < nombreNotes; ++j) {
				prochainesNotes[i, j] = Partition.StatutNote.Muette;
			}
		}
	}

	void Update () {
		// Temps actuel, en echantillons --> temps de remplissage.
		int tempsActuel = (int)(((Time.time - tempsDebut) / resolution) * speed);

		// Temps a jouer, en echantillons.
		int tempsAJouer = tempsActuel - (int)(tempsAttendreDebutMusique * speed);

		// Remplir des notes jusqu'au temps actuel.
		partition.RemplirProchainesNotes((Time.time - tempsDebut) * speed, prochainesNotes,
		                                 nombreEchantillons, resolution);

		// Jouer le temps actuel.
		if (tempsAJouer < 0)
			return;

		for (int i = dernierTempsJoue; i < tempsAJouer; ++i) {
			int tempsAJouerModulo = i % nombreEchantillons;

			// Passer toutes les notes.
			for (int j = 0; j < nombreNotes; ++j) {
				// Jouer la note si necessaire.
				if (prochainesNotes[tempsAJouerModulo, j] != Partition.StatutNote.Muette) {
					instrumentScript.PlayNote(j);
				} else {
					instrumentScript.StopNote(j);
				}

				// Nettoyer le tableau.
				prochainesNotes[tempsAJouerModulo, j] = Partition.StatutNote.Muette;
			}
		}

		dernierTempsJoue = tempsAJouer;
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

	// Nombre d'echantillons presents dans le tableau de prochaines notes a jouer.
	private const int nombreEchantillons = 120;

	// Nombre de notes de l'instrument controle par ce mode.
	private const int nombreNotes = 48;

	// Interface Instrument de l'instrument controle par ce mode.
	private Instrument instrumentScript;

	// Temps auquel la musique a commence a jouer.
	private float tempsDebut;

	// Partition a jouer.
	private Partition partition;
}
