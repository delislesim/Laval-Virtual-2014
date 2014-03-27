using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuitarPlayer : MonoBehaviour {

	public AudioClip[] HighVelocityNotes;
	public AudioClip[] LowVelocityNotes;
	public AudioClip[] MedVelocityNotes;
	public AudioClip[] PowerChords;
	public Transform LeftHandTransform;
	public AssistedModeControllerGuitar AssistedCtrl;
	public AudioSource audio1;
	public AudioSource audio2; //audio qui joue une note de plus pour faire un accord
	public AudioSource audio3;
	// Halos inquant sur quelle partie du manche on se trouve, pour le mode assiste.
	public List<GameObject> halosAssiste;

	// Halos indiquant sur quelle partie du manche on se trouve, pour le mode libre.
	public List<GameObject> halosLibre;

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
			audio1.clip = HighVelocityNotes[dummy_counter % HighVelocityNotes.Length];
			audio1.Play();
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
			//Debug.Log ("Octave : " + octave);
			if(style == Style.NOTE){
				//switch(level){
				//case 0:
					idx = note + (12*octave);
					if(idx > 44)//be safe
						idx = idx-12;
				//	break;
				//default :
				//	idx = note + (12*(octave + 1));
				//	if(idx > 44)//be safe
				//		idx = idx-12;
				//	break;
				//}
				audio1.clip = HighVelocityNotes[idx];

			}
			else if(style == Style.CHORD){
				if(level == 0)
					idx = note;
				else
					idx = note+12;
				idx = idx%18;
				audio1.clip = HighVelocityNotes[idx];
				//audio2.clip = HighVelocityNotes[idx+7];
				//audio3.clip = HighVelocityNotes[idx+12];
				//audio3.Play();
				//audio2.Play ();
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
			
			audio1.clip = HighVelocityPlayableNotes[level][idx];
			//Debug.Log ("Note name : " + audio1.clip.name);

		}

		audio1.Play();
	}

	public void SetPitch(int pitch) {
		audio1.pitch = 1.0f * Mathf.Pow(1.05946f, pitch);
		//audio2.pitch = 1.0f * Mathf.Pow(1.05946f, pitch);
		//audio3.pitch = 1.0f * Mathf.Pow(1.05946f, pitch);
	}

	// Use this for initialization
	void Start () {
		dummy_counter = 0;
		AssistedCtrl.SetGuitarPlayer (this);
		HighVelocityPlayableNotes = new List<List<AudioClip>>();
		SetScaleModeAndTone(Mode.BLUES, Tone.E);
	}
	
	// Update is called once per frame
	void Update () {
		// Animer les halos indiquant sur quelle partie du manche se trouve la main.
		int level = SetPitchLevel ();
		if (AssistedModeControllerGuitar.EstActive ()) {
			for (int i = 0; i < halosLibre.Count; ++i) {
				AnimerTransparent(halosLibre[i], false);
			}
			for (int i = 0; i < halosAssiste.Count; ++i) {
				AnimerTransparent(halosAssiste[i], level == i);
			}
		} else {
			for (int i = 0; i < halosLibre.Count; ++i) {
				AnimerTransparent(halosLibre[i], level == i);
			}
			for (int i = 0; i < halosAssiste.Count; ++i) {
				AnimerTransparent(halosAssiste[i], false);
			}
		}
	}

	// Animer les halos inquant quelle note le guitariste joue.
	void AnimerTransparent(GameObject transparent, bool allume) {
		Color32 color = transparent.renderer.material.color;
		Color32 nextColor;
		if (allume) {
			transparent.SetActive(true);
			nextColor = Color.Lerp(color, Color.white, Time.deltaTime * 6.0f);
		} else {
			nextColor = Color.Lerp(color, Color.black, Time.deltaTime * 6.0f);
			if (nextColor == Color.black) {
				transparent.SetActive (false);
			}
		}
		transparent.renderer.material.color = nextColor;
	}

	/// <summary>
	/// Sets the pitch level, according to distance of left hand from hip.
	/// </summary>
	/// <returns>The pitch level.</returns>
	int SetPitchLevel()
	{
		//Distance main gauche - hip center
		float dist = Vector3.Distance(MoveJointsForGuitar.GetPositionGuitare(),
		                              LeftHandTransform.position);
		int niveauAigue = -1 ; //Plus grave

		if (dist > LONGUEUR_MANCHE)
			niveauAigue = 0;
		else if(LONGUEUR_MANCHE > dist && dist >= 4*LONGUEUR_MANCHE/5)
			niveauAigue = 1;
		else if(4*LONGUEUR_MANCHE/5 > dist && dist >= 3*LONGUEUR_MANCHE/5)
			niveauAigue = 2;
		else if(3*LONGUEUR_MANCHE > dist && dist >= LONGUEUR_MANCHE/10)
			niveauAigue = 3;

		if (AssistedModeControllerGuitar.EstActive()) {
			// Seulement 2 niveaux dans le mode assiste.
			niveauAigue = niveauAigue / 2; //Max level = 2, should be 0 or 1
		}

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
