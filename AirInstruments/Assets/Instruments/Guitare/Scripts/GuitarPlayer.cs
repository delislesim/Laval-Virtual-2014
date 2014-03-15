using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuitarPlayer : MonoBehaviour {

	public AudioClip[] HighVelocityNotes;
	public AudioClip[] LowVelocityNotes;
	public AudioClip[] MedVelocityNotes;
	public AudioClip[] PowerChords;
	public Transform HipTransform;
	public Transform LeftHandTransform;
	public AssistedModeControllerGuitar AssistedCtrl;

	public enum Tone : int
	{
		E = 0, F, Gb, G, Ab, A, Bb, B, C, Db, D, Eb
	}

	public enum Style : int
	{
		NOTE = 0,
		CHORD
	}

	public enum Mode : int
	{
		BLUES = 0,
		MAJOR,
		MINOR,
		PENT
	}

	private int dummy_counter;
	//List of audio lists.
	//The different lists for different pitch level (left hand position)
	private List<List<AudioClip>> HighVelocityPlayableNotes;
	private const float LONGUEUR_MANCHE = 3.0f;

/*********************************************************/
	public void SetScaleModeAndTone(Mode mode, Tone tone)
	{
		switch (mode)
		{
		case Mode.BLUES:
			SetBluesScale(tone);
			break;
		case Mode.MAJOR:
			SetMajorScale(tone);
			break;
		case Mode.MINOR:
			SetMinorScale(tone);
			break;
		case Mode.PENT:
			SetPentatonicScale(tone);
			break;
		default:
			Debug.LogError("Invalid scale mode!");
			break;
		}
	}

	/// <summary>
	/// Plays the next random note.
	/// </summary>
	public void PlayNextRandomNote()
	{
		int pitchLevel = (int)Random.Range (0,4); 

		if(HighVelocityNotes.Length != 0)
		{
			audio.clip = HighVelocityNotes[dummy_counter % HighVelocityNotes.Length];
			audio.Play();
			dummy_counter ++;
		}
		else
			Debug.LogError ("No notes found!!!");
	}
	
	//TODO take velocity into consideration
	/// <summary>
	/// Plays the next note, according to scale and tone and distance of left hand, and velocity.
	/// </summary>
	public void PlayNextNote()
	{
		//Play on good positions of left hand only
		int level = SetPitchLevel();
		if (level == -1)
			return;

		if(AssistedModeControllerGuitar.EstActive())
		{
			///get the note 
			Style style = AssistedCtrl.getCurrentStyle();
			int note = (int)AssistedCtrl.getCurrentTone();
			int octave = AssistedCtrl.getCurrentOctave();
			int idx = 0;
			level = level / 2; //Max level = 2, should be 0 or 1
			//Debug.Log ("Octave : " + octave);
			if(style == Style.NOTE){
				switch(level){
				case 0:
					idx = note + (12*octave);
					if(idx > 44)//be safe
						idx = idx-12;
					break;
				default :
					idx = note + (12*(octave + 1));
					if(idx > 44)//be safe
						idx = idx-12;
					break;
				}
				audio.clip = HighVelocityNotes[idx];

			}
			else if(style == Style.CHORD){
				if(level == 0)
					idx = note;
				else
					idx = note+12;
				idx = idx%18;
				audio.clip = PowerChords[idx];
			}

		}
		else //random
		{
			int maxIndex = HighVelocityPlayableNotes[level].Count;
			//Debug.Log("Count : " + maxIndex);
			
			int idx = (int)Random.Range (0, maxIndex);
			//int idx =  (dummy_counter % maxIndex);
			//dummy_counter ++;
			//Debug.Log("Index : " + idx);
			
			audio.clip = HighVelocityPlayableNotes[level][idx];
			//Debug.Log ("Note name : " + audio.clip.name);

		}

		audio.Play();
	}

	// Use this for initialization
	void Start () {
		dummy_counter = 0;
		HighVelocityPlayableNotes = new List<List<AudioClip>>();
		SetScaleModeAndTone(Mode.BLUES, Tone.E);
		audio.pitch = audio.pitch*Mathf.Pow(1.05946f,-1);
	}
	
	// Update is called once per frame
	void Update () {
	}

	/// <summary>
	/// Sets the pitch level, according to distance of left hand from hip.
	/// </summary>
	/// <returns>The pitch level.</returns>
	int SetPitchLevel()
	{
		//Distance main gauche - hip center
		float dist = Vector3.Distance(HipTransform.position, LeftHandTransform.position);
		int niveauAigue = -1 ; //Plus grave

		if (dist > LONGUEUR_MANCHE)
			niveauAigue = 0;
		else if(LONGUEUR_MANCHE > dist && dist >= 4*LONGUEUR_MANCHE/5)
			niveauAigue = 1;
		else if(4*LONGUEUR_MANCHE/5 > dist && dist >= 3*LONGUEUR_MANCHE/5)
			niveauAigue = 2;
		else if(3*LONGUEUR_MANCHE > dist && dist >= LONGUEUR_MANCHE/10)
			niveauAigue = 3;
		//Debug.Log ("Pitch Level : " + niveauAigue );
		return niveauAigue;
	}

	/// <summary>
	/// Dispatches playable notes inside pitch level lists.
	/// There are 4 pitch levels. Each one has a third of the available
	/// notes. Each level "starts" one third (of a level size) before
	/// the end of the previous level.
	/// </summary>
	/// <param name="notes">Notes.</param>
	/// 
	 void SetPlayableLists(List<AudioClip> notes)
	{
		//TODO Remove hardcoding?
		int totalCount = notes.Count;
		int levelCount = (int)totalCount/3; //Each level has 1/3 of notes
		int[] levelStarts = {0, (int)(totalCount*0.2222f), (int)(totalCount*0.4444f), (int)(totalCount*0.6666f)};

		//Creat level lists.
		for(int i = 0; i<=3; i++)
		{
			HighVelocityPlayableNotes.Add (new List<AudioClip>());
			for(int j = levelStarts[i] ; j< levelStarts[i]+levelCount ; j++)
			{
				HighVelocityPlayableNotes[i].Add(notes[j]);
			}
		}
		
	}

	/********************************SET SCALES**********************************************/
	void SetBluesScale(Tone tone)
	{
		//Temp values
		int toneInt = (int) tone;
		List<AudioClip> PlayableNotes = new List<AudioClip>();

		//Set Scale
		for(int i = 0 ; i < HighVelocityNotes.Length ; i++)
		{
			for(int j = -1 ; j<= 4; j++)
			{
				/// Blues scale
				if (i==toneInt+(j*12))
				{
					//Add base note more times so it plays more often
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
				else if (i==toneInt+5+(j*12))
				{
					//Add 5th twice
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
				}

			    else if( (i==toneInt+3+(j*12))  || (i==toneInt+6+(j*12)) || (i==toneInt+7+(j*12)) || (i==toneInt+10+(j*12)) )
				{
					//Add other notes
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
			}
		}

		SetPlayableLists(PlayableNotes);
	}

	void SetMajorScale(Tone tone)
	{
		int toneInt = (int) tone;
		List<AudioClip> PlayableNotes = new List<AudioClip>();

		for(int i = 0 ; i < HighVelocityNotes.Length ; i++)
		{
			for(int j = -1 ; j<= 4; j++)
			{
				/// Major Scale
				if (i==toneInt+(j*12))
				{
					//Add base note more times so it plays more often
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
				else if( (i==toneInt+2+(j*12))  || (i==toneInt+4+(j*12)) 
				        || (i==toneInt+5+(j*12)) || (i==toneInt+7+(j*12)) 
				        || (i==toneInt+9+(j*12)) || (i==toneInt+11+(j*12)))
				{
					//Add other notes
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
			}
		}
		SetPlayableLists(PlayableNotes);
	}

	void SetMinorScale(Tone tone)
	{
		int toneInt = (int) tone;
		List<AudioClip> PlayableNotes = new List<AudioClip>();

		for(int i = 0 ; i < HighVelocityNotes.Length ; i++)
		{
			for(int j = -1 ; j<= 4; j++)
			{
				/// Major Scale
				if (i==toneInt+(j*12))
				{
					//Add base note more times so it plays more often
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
				else if( (i==toneInt+2+(j*12))  || (i==toneInt+3+(j*12)) 
				        || (i==toneInt+5+(j*12)) || (i==toneInt+7+(j*12)) 
				        || (i==toneInt+8+(j*12)) || (i==toneInt+10+(j*12)))
				{
					//Add other notes
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
			}
		}
		SetPlayableLists(PlayableNotes);
	}

	void SetPentatonicScale (Tone tone)
	{
		int toneInt = (int) tone;
		List<AudioClip> PlayableNotes = new List<AudioClip>();

		for(int i = 0 ; i < HighVelocityNotes.Length ; i++)
		{
			for(int j = -1 ; j<= 4; j++)
			{
				/// Major Scale
				if (i==toneInt+(j*12))
				{
					//Add base note more times so it plays more often
					PlayableNotes.Add (HighVelocityNotes[i]);
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
				else if((i==toneInt+3+(j*12)) || (i==toneInt+5+(j*12)) 
				        || (i==toneInt+7+(j*12))  || (i==toneInt+10+(j*12)))
				{
					//Add other notes
					PlayableNotes.Add (HighVelocityNotes[i]);
				}
			}
		}
		SetPlayableLists(PlayableNotes);
	}
	/*************************************************************************************/


}
