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

	public void StartSong()
	{
		HasStarted = true;
		audio.volume = 0.4f;
		audio.Play();
	}

	void Start () {
		tempsEcoule = 0.0f;
		tempsNotes = 0.0f;
		partitionMaker = new PartitionGuitar ();
		partitionMaker.ChargerFichier(".\\Assets\\Modes\\Assiste\\Guitare\\Chansons\\Lonely Boy Audacity.aup");
		partition = new List<PartitionGuitar.Playable>();
		// Remplir des notes jusqu'au temps actuel.
		partitionMaker.RemplirPartition( partition );
		currentPartitionIndex = 0;
		currentTone = partition[currentPartitionIndex].note;
		currentStyle = partition[currentPartitionIndex].style;
		currentOctave = partition[currentPartitionIndex].octave;
		HasStarted = false;
		//audio.source = ... toune de fond
		//audio.play
	}

	void Update () {
		if(HasStarted)
		{
			// Temps actuel, en secondes.
			//Set le tone et style de la note a jouer
			tempsEcoule = tempsEcoule + Time.deltaTime;
			//Debug.Log("Temps ecoulé : " + tempsEcoule);
			if (tempsEcoule > partition[currentPartitionIndex+1].time)
			{
				tempsNotes = tempsNotes + partition[currentPartitionIndex+1].time;
				if(currentPartitionIndex < partition.Count-1)
					currentPartitionIndex ++;
			}
			currentTone = partition[currentPartitionIndex].note;
			currentStyle = partition[currentPartitionIndex].style;
			currentOctave = partition[currentPartitionIndex].octave;
		}
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
	private bool HasStarted;
}
