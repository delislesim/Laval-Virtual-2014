using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssistedModeControllerGuitar : MonoBehaviour {

	public AudioClip clipLonelyBoy;
	public AudioClip clipTNT;
	public AudioClip clipBoubou;

	private string fichierLonelyBoy = ".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\Lonely Boy Audacity.aup";
	private string fichierTNT = ".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\TNT.aup";
	private string fichierBoubou = ".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\Lonely Boy Audacity.aup";

	public enum Chanson
	{
		LONELY_BOY,
		TNT,
		BOUBOU
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
		tempsNotes = 0.0f;

		string nomFichier;
		AudioClip clip;
		switch (chanson) {
		case Chanson.LONELY_BOY:
			nomFichier = fichierLonelyBoy;
			clip = clipLonelyBoy;
			break;
		case Chanson.TNT:
			nomFichier = fichierTNT;
			clip = clipTNT;
			break;
		case Chanson.BOUBOU:
			nomFichier = fichierBoubou;
			clip = clipBoubou;
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

		// Partir la musique.
		audio.clip = clip;
		audio.volume = 0.35f;
		audio.Play();
	}

	public void StopSong() {
		audio.Stop ();
		partition = null;
	}

	public bool EstActive() {
		return partition != null;
	}

	void Update () {
		if (!EstActive())
			return;
		
		// Temps actuel, en secondes.
		//Set le tone et style de la note a jouer
		tempsEcoule = tempsEcoule + Time.deltaTime;
		//Debug.Log("Temps ecoulé : " + tempsEcoule);
		if(currentPartitionIndex < partition.Count-1){
			if (tempsEcoule > partition[currentPartitionIndex+1].time)
			{
				tempsNotes = tempsNotes + partition[currentPartitionIndex+1].time;
					currentPartitionIndex ++;
			}
		}
		currentTone = partition[currentPartitionIndex].note;
		currentStyle = partition[currentPartitionIndex].style;
		currentOctave = partition[currentPartitionIndex].octave;
	}
	
	private PartitionGuitar partitionMaker;
	// Partition a jouer.
	private List<PartitionGuitar.Playable> partition;

	private float tempsEcoule;
	private float tempsNotes; // Temps qui augmente avec les duree des notes. (par step)
	private int currentPartitionIndex;
	private int currentOctave;

	private GuitarPlayer.Tone currentTone;
	private GuitarPlayer.Style currentStyle;
}
