using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssistedModeControllerGuitar : MonoBehaviour {

	public AudioClip clipLonelyBoy;
	public AudioClip clipTNT;
	public AudioClip clipBoubou;

	private string fichierLonelyBoy = ".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\Lonely Boy Audacity.aup";
	private string fichierTNT = ".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\TNTForGuitar.aup";

	// Cubes tombants.
	public CubesTombantsGuitare cubesTombants;

	// Main du guitariste.
	public HandFollower handFollower;

	public enum Chanson
	{
		LONELY_BOY,
		TNT
	}

	public void SetGuitarPlayer(GuitarPlayer guitarPlayer) {
		this.guitarPlayer = guitarPlayer;
	}

	public GuitarPlayer.Tone getCurrentTone(){
		return currentTone;
	}

	public GuitarPlayer.Style getCurrentStyle(){
		return currentStyle;
	}

	public int getCurrentOctave(){
		return currentOctave;
	}

	public void StartSong(Chanson chanson)
	{
		// Reinitialiser les variables.
		tempsEcoule = 0.0f;
		//tempsNotes = 0.0f;

		string nomFichier;
		AudioClip clip;
		switch (chanson) {
		case Chanson.LONELY_BOY:
			nomFichier = fichierLonelyBoy;
			clip = clipLonelyBoy;
			guitarPlayer.SetPitch(0);
			break;
		case Chanson.TNT:
			nomFichier = fichierTNT;
			clip = clipTNT;
			guitarPlayer.SetPitch(-1);
			break;
		default:
			return;
		}
		
		// Lire la partition.
		partitionMaker = new PartitionGuitar ();
		partitionMaker.ChargerFichier(nomFichier);
		partition = new List<PartitionGuitar.Playable>();

		partitionMaker.RemplirPartition( partition );
		currentPartitionIndex = 0;
		currentTone = partition[currentPartitionIndex].note;
		currentStyle = partition[currentPartitionIndex].style;
		currentOctave = partition[currentPartitionIndex].octave;
		currentSolo = partition [currentPartitionIndex].solo == PartitionGuitar.Solo.DEBUT;

		if (partition[currentPartitionIndex].time > 0.15f) {
			handFollower.DefinirTempsProchaineNote(partition[currentPartitionIndex].time);
		} else {
			handFollower.DefinirTempsProchaineNote(1000.0f);
		}

		// Charger les cubes.
		cubesTombants.ChargerPartition (partition);

		// Partir la musique.
		audio.clip = clip;
		audio.volume = 0.35f;
		audio.Play();
	}

	public void StopSong() {
		audio.Stop ();
		partition = null;
		cubesTombants.ViderCubes ();
	}

	public static bool EstActive() {
		return partition != null &&
			tempsEcoule < partition [partition.Count - 1].time + 5.0f; // Attendre 1 seconde apres la fin de la musique.
	}

	public static bool EstSolo() {
		return EstActive() && currentSolo;
	}

	void Update () {


		if (partition == null || tempsEcoule > partition [partition.Count - 1].time + 6.0f)
			return;

		tempsEcoule = audio.timeSamples/44100.0f + 0.055f;
		//Log.Debug("Temps de chanson : " + tempsEcoule);

		// Temps actuel, en secondes.
		//Set le tone et style de la note a jouer
		//tempsEcoule = tempsEcoule + Time.deltaTime;


		if(currentPartitionIndex < partition.Count-1) {
			if (tempsEcoule >= partition[currentPartitionIndex+1].time) {

				currentPartitionIndex ++;

				// Activer / desactiver le solo.
				PartitionGuitar.Solo solo = partition[currentPartitionIndex].solo;
				if (solo == PartitionGuitar.Solo.DEBUT) {
					currentSolo = true;
				} else if (solo == PartitionGuitar.Solo.FIN) {
					currentSolo = false;
				} else {
					// Note normale.
					currentTone = partition[currentPartitionIndex].note;
					currentStyle = partition[currentPartitionIndex].style;
					currentOctave = partition[currentPartitionIndex].octave;

					float tempsNoteCourante = partition[currentPartitionIndex].time;
					float tempsNotePrecedente = partition[currentPartitionIndex - 1].time;
					float tempsDepuisDerniereNote = tempsNoteCourante - tempsNotePrecedente;
					handFollower.JouerNoteMaintenant(tempsDepuisDerniereNote);
				}

				// Definir le temps de la prochaine note.
				if (currentPartitionIndex + 1 < partition.Count) {
					handFollower.DefinirTempsProchaineNote(partition[currentPartitionIndex + 1].time - tempsEcoule);
				}
			}
		}

		// Faire avancer les cubes.
		cubesTombants.AssignerTempsCourant (tempsEcoule);

		// Reinitialiser le pitch a la fin du mode assiste.
		if (!EstActive ()) {
			guitarPlayer.SetPitch (0);		
		}
	}
	
	private PartitionGuitar partitionMaker;
	// Partition a jouer.
	private static List<PartitionGuitar.Playable> partition;

	private static float tempsEcoule;
	//private float tempsNotes; // Temps qui augmente avec les duree des notes. (par step)
	private int currentPartitionIndex;
	private int currentOctave;
	private static bool currentSolo;

	private GuitarPlayer.Tone currentTone;
	private GuitarPlayer.Style currentStyle;

	// Guitar player.
	private GuitarPlayer guitarPlayer;
}
