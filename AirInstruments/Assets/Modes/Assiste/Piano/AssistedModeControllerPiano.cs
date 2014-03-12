using UnityEngine;
using System.Collections;

public class AssistedModeControllerPiano : MonoBehaviour {

	// GameObject de l'instrument controle par ce mode.
	public GameObject instrument;

	// GameObject de cubes tombants.
	public GameObject cubesTombants;

	// Indique s'il y a presentement une musique en train d'etre jouee
	// dans le mode assiste.
	public static bool EstActive () {
		return estActive;
	}

	// Charge un fichier de partition et demarre le mode assiste.
	public void ChargerPartition(string fichierPartition, float speed) {
		Reinitialiser ();

		partition = new PartitionPiano ();
		partition.ChargerFichier (fichierPartition);
		this.speed = speed;
		
		// Remplir tout le tableau de prochaines notes avec des notes muettes.
		for (int i = 0; i < nombreEchantillons; ++i) {
			for (int j = 0; j < nombreNotes; ++j) {
				prochainesNotes[i, j] = PartitionPiano.StatutNote.Muette;
			}
		}

		// Activer les cubes tombants.
		cubesTombants.SetActive (true);

		// Se rappeler qu'il y a une chanson en cours.
		estActive = true;
	}

	// Active le mode libre (desactive toute partition en cours).
	public void ActiverLibre() {
		estActive = false;
		cubesTombants.SetActive (false);
		Reinitialiser ();
	}

	// Reinitialise les cubes, la musique et tout :)
	private void Reinitialiser() {
		Start ();

		prochainesNotes = new PartitionPiano.StatutNote[nombreEchantillons, nombreNotes];
		dernierTempsJoue = 0;
		tempsActuel = 0.0f;
		peutContinuer = true;
		partition = null;
		speed = 0;
		
		cubesTombantsScript.ViderCubes ();

		// Mettre toutes les notes muettes.
		for (int i = 0; i < nombreNotes; ++i) {
			instrumentScript.DefinirStatutNote (i, PartitionPiano.StatutNote.Muette);
		}
	}

	bool initialized = false;
	void Start () {
		if (initialized)
			return;
		instrumentScript = (Instrument)(instrument.GetComponent (typeof(PianoBuilder)));
		cubesTombantsScript = (CubesTombants)(cubesTombants.GetComponent (typeof(CubesTombants)));
		cubesTombantsScript.AssignerInstrument (instrumentScript);
		initialized = true;
	}

	void Update () {
		if (!EstActive())
			return;

		if (peutContinuer) {
			// Temps actuel, en secondes. Utilise pour remplir le tableau de prochaines notes.
			tempsActuel += Time.deltaTime * speed;

			// Temps de la note qu'on entend, en secondes.
			float tempsAJouer = tempsActuel - (tempsAttendreDebutMusique * speed);

			// Temps a jouer, en echantillons.
			int tempsAJouerEchantillons = (int) (tempsAJouer * resolutionInverse);

			// Remplir des notes jusqu'au temps actuel.
			partition.RemplirProchainesNotes(tempsActuel,
			                                 prochainesNotes,
			                                 nombreEchantillons,
			                                 resolutionInverse,
			                                 cubesTombantsScript);

			// Faire avancer les cubes.
			cubesTombantsScript.AssignerTempsCourant (tempsAJouer);

			// Jouer le temps actuel.
			if (tempsAJouerEchantillons >= 0) {
				for (int i = dernierTempsJoue; i < tempsAJouerEchantillons; ++i) {
					int tempsAJouerEchantillonsModulo = i % nombreEchantillons;

					// Reinitialiser les booleens indiquant si chaque note est adjacent
					// a une note a jouer.
					for (int j = 0; j < nombreNotes; ++j) {
						instrumentScript.DefinirAdjacentAJouer(j, false);
					}

					// Passer toutes les notes que le joueur doit jouer.
					for (int j = 0; j < nombreNotes; ++j) {
						PartitionPiano.StatutNote statutNote = prochainesNotes [tempsAJouerEchantillonsModulo, j];
						if (statutNote != PartitionPiano.StatutNote.Accompagnement) {
							instrumentScript.DefinirStatutNote (j, statutNote);
						}
					}

					// Verifier si on peut continuer.
					bool peutContinuerARemplir = true;
					for (int j = 0; j < nombreNotes; ++j) {
						PianoNote note = instrumentScript.ObtenirNote(j);
						if (!note.PeutContinuer()) {
							peutContinuerARemplir = false;
							break;
						}
					}

					if (peutContinuerARemplir) {
						// Faire l'accompagnement.
						for (int j = 0; j < nombreNotes; ++j) {
							PartitionPiano.StatutNote statutNote = prochainesNotes [tempsAJouerEchantillonsModulo, j];
							
							if (statutNote == PartitionPiano.StatutNote.Accompagnement) {
								instrumentScript.DefinirStatutNote (j, statutNote);
							}

							// Nettoyer le tableau.
							prochainesNotes [tempsAJouerEchantillonsModulo, j] = PartitionPiano.StatutNote.Muette;
						}
					} else {
						tempsAJouerEchantillons = i;
						tempsActuel = (tempsAttendreDebutMusique * speed) + tempsAJouerEchantillons * resolution;
						break;
					}
				}

				dernierTempsJoue = tempsAJouerEchantillons;
			}
		}

		// Gerer les notes qui doivent etre jouees.
		peutContinuer = true;
		for (int indexNote = 0; indexNote < nombreNotes; ++indexNote) {
			// TODO(aimantation)

			PianoNote note = instrumentScript.ObtenirNote(indexNote);
			bool ok = note.GererNoteQuiDoitEtreJouee();
			if (!ok)
				peutContinuer = false;
		}

		// Mettre a jour les timers des notes enfoncees par erreur et jouer les notes au besoin.
		for (int indexNote = 0; indexNote < nombreNotes; ++indexNote) {
			PianoNote note = instrumentScript.ObtenirNote(indexNote);
			note.MettreAJourTimerEnfonceeParErreur();
		}

		// Remettre tous les angles a zero.
		for (int indexNote = 0; indexNote < nombreNotes; ++indexNote) {
			PianoNote note = instrumentScript.ObtenirNote(indexNote);
			note.DefinirAngle(0);
		}

		// Verifier si la musique est terminee.
		float tempsFin = partition.ObtenirTempsFin ();
		if (tempsFin != -1.0f && tempsActuel > tempsFin) {
			ActiverLibre();
		}
	}

	// Indique si le mode assiste est active.
	private static bool estActive = false;

	// Dernier temps qu'on a joue (non inclusivement), en nombre d'echantillons.
	private int dernierTempsJoue = 0;

	// Temps auquel la musique a commence a jouer.
	private float tempsActuel = 0.0f;
	
	// Indique si la musique peut continuer a avancer au temps suivant.
	private bool peutContinuer = true;
	
	// Partition a jouer.
	private PartitionPiano partition;

	// Tableau qui contient les prochaines notes a jouer.
	private PartitionPiano.StatutNote[,] prochainesNotes;

	// Facteur pour jouer plus rapidement.
	private float speed = 0;

	// Temps a attendre avant de commencer a jouer la musique, en secondes.
	// Ceci correspond au decalage entre le remplissage et le jouage.
	private const float tempsAttendreDebutMusique = 6.0f;
	
	// Resolution du tableau de prochaines notes a jouer.
	private const float resolution = 0.1f;

	// Resolution inverse du tableau de prochaines notes a jouer.
	private const float resolutionInverse = 10.0f;

	// Nombre d'echantillons presents dans le tableau de prochaines notes a jouer.
	private const int nombreEchantillons = (int)(600 * 6);

	// Nombre de notes de l'instrument controle par ce mode.
	private const int nombreNotes = 52;

	// Interface Instrument de l'instrument controle par ce mode.
	private Instrument instrumentScript;

	// Script du GameObject de cubes tombants.
	private CubesTombants cubesTombantsScript;
}
