using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuitarPlayer : MonoBehaviour {

	public AudioClip[] HighVelocityNotes;
	public AudioClip[] LowVelocityNotes;
	public AudioClip[] MedVelocityNotes;

	public enum Tone : int
	{
		E = 0, F, Gb, G, Ab, A, Bb, B, C, Db, D
	}

	public enum Mode : int
	{
		BLUES = 0,
		MAJOR,
		MINOR,
		PENT
	}

	private Mode scale_mode;
	private int dummy_counter;

	private List<AudioClip> HighVelocityPlayableNotes;


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
		default:
			Debug.LogError("Invalid scale mode!");
			break;
		}
	}
	public void PlayNextRandomNote()
	{
		if(HighVelocityNotes.Length != 0)
		{
			audio.clip = HighVelocityNotes[dummy_counter % HighVelocityNotes.Length];
			audio.Play();
			dummy_counter ++;
		}
		else
			Debug.LogError ("No notes found!!!");
	}

	//TODO take distance of leftHand from Hip into consideration instead of random
	//TODO take velocity into consideration
	public void PlayNextNote()
	{

		int maxIndex = HighVelocityPlayableNotes.Count;
		Debug.Log("Count : " + maxIndex);

		int idx = (int)Random.Range (0, maxIndex);
		Debug.Log("Index : " + idx);

		audio.clip = HighVelocityPlayableNotes[idx];
		audio.Play();
		//dummy_counter ++;
	}

	// Use this for initialization
	void Start () {
		dummy_counter = 0;
		scale_mode = Mode.BLUES;
		HighVelocityPlayableNotes = new List<AudioClip>();

		SetScaleModeAndTone(scale_mode, Tone.E);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Trigger with guitar player! : " + col.gameObject.tag);
		if(col.gameObject.tag == "PlayHand")
			PlayNextNote();
	}
	
	/********************************SET SCALES**********************************************/
	void SetBluesScale(Tone tone)
	{
		int toneInt = (int) tone;
		for(int i = 0 ; i < HighVelocityNotes.Length ; i++)
		{
			for(int j = -1 ; j<= 4; j++)
			{
				/// Blues scale
				if ((i==toneInt+(j*12)) || i==(toneInt+3+(j*12)) || (i==toneInt+5+(j*12)) 
				    || (i==toneInt+6+(j*12)) || (i==toneInt+7+(j*12)) || (i==toneInt+10+(j*12)) )
				{
					//Add note to PlayableNotes
					HighVelocityPlayableNotes.Add (HighVelocityNotes[i]);
				}
			}
		}
	}

	void SetMajorScale(Tone tone)
	{
		
	}

	void SetMinorScale(Tone tone)
	{
		
	}


}
