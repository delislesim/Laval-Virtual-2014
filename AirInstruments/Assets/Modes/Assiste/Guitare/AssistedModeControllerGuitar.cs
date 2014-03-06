using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssistedModeControllerGuitar : MonoBehaviour {

	public GuitarPlayer.Tone getCurrentTone(){
		return currentTone;
	}

	public GuitarPlayer.Style getCurrentStyle(){
		return currentStyle;
	}

	public int getCurrentOctave(){
		return currentOctave;
	}

	public void StartSong(string fichierPartition)
	{
		// Reinitialiser les variables.
		tempsEcoule = 0.0f;
		tempsNotes = 0.0f;
		
		// Lire la partition.
		partitionMaker = new PartitionGuitar ();
		partitionMaker.ChargerFichier(fichierPartition);
		partition = new List<PartitionGuitar.Playable>();

		partitionMaker.RemplirPartition( partition );
		currentPartitionIndex = 0;
		currentTone = partition[currentPartitionIndex].note;
		currentStyle = partition[currentPartitionIndex].style;
		currentOctave = partition[currentPartitionIndex].octave;

		// Partir la musique.
		audio.volume = 0.35f;
		audio.Play();
	}

	void Update () {
		if (partition == null)
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
