﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DrumAssistedController : MonoBehaviour {
	public TipFollower tipRight;
	public TipFollower tipLeft;
	public GameObject bigTom;
	public GameObject tom1;
	public GameObject tom2;
	public GameObject snare;
	public GameObject crash;
	public GameObject ride;
	public GameObject hihat;

	public AudioSource baseRythmA;
	public AudioSource baseRythmB;
	public AudioSource track1_A;
	public AudioSource track1_B;
	public AudioSource track2_A;
	public AudioSource track2_B;

	private Dictionary<GameObject, List<List<AudioClip>>> TracksCollection = new Dictionary<GameObject, List<List<AudioClip>>>();

	private float elapsedTime;
	private float S_DELAY;
	private float CHOICE_DELAY = 0.3f;
	private bool trackBplaying;
	private bool trackAplaying;
	private bool track2Needed;
	private bool track1Needed;
	private float SAMPLE_TIME;
	private bool choiceNeeded;

	// Use this for initialization
	void Start () {

		FillDictionary();
		baseRythmA.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");
		baseRythmB.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");

		track1_A.clip = TracksCollection[bigTom][3][0];
		track1_B.clip = TracksCollection[bigTom][3][0];

		track2_A.clip = TracksCollection[snare][0][0];
		track2_B.clip = TracksCollection[snare][0][0];

		trackBplaying = false;
		trackAplaying = true;
		track1Needed = true;
		track2Needed = true;
		choiceNeeded = true;

		SAMPLE_TIME = baseRythmA.clip.length; 

		S_DELAY = 0.1f;
		elapsedTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime = elapsedTime + Time.deltaTime;

		//Track Logic
		if (elapsedTime >= (SAMPLE_TIME - CHOICE_DELAY) && choiceNeeded)
		{
			SetUpAudioSources();
			choiceNeeded = false;
		}

		//Track Control
		if (elapsedTime >= (SAMPLE_TIME - S_DELAY))
		{
			Debug.Log ("Elapsed time : " + elapsedTime + ", SAMPLE_TIME : " + SAMPLE_TIME + " , S_DELAY : " + S_DELAY); 
			PlayTracks ();
			choiceNeeded = true;
			elapsedTime = 0;
		}
	}

	void PlayTracks()
	{
		//ALTERNATE A and B so transitions are fluid and on time
		if(trackBplaying){
			baseRythmA.Play();
			
			if(track1Needed){
				track1_A.Play ();
			}
			if(track2Needed){
				track2_A.Play ();
			}

			trackBplaying=false;
		}
		else{
			baseRythmB.Play();
			
			if(track1Needed){
				track1_B.Play ();
			}
			if(track2Needed){
				track2_B.Play ();
			}
			trackBplaying=true;
		}
	}

	void SetUpAudioSources()
	{

	}

	void FillDictionary()
	{
		List<AudioClip>[] ProbLists = 
		{
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>(),
			new List<AudioClip>()
		};

		List<List<AudioClip>> ComponentList = new List<List<AudioClip>>();

		//BIG TOM									\Assets\Instruments\Drum\DrumTracks\BigTom
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/1b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/2c"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/4b"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/5a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/6a"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/8a"));

		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}

		TracksCollection.Add(bigTom, new List<List<AudioClip>>(ComponentList));

		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//TOM1
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/1b"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/1c"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/2c"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3c"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3d"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/4b"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/5a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/6a"));
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/8a"));
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(tom1, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//TOM2
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/1a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/2b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/4b"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/6a"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/Tom1/6b"));
	
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(tom2, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//Snare
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2c"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4b"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Snare/5a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Snare/6a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Snare/6b"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/Snare/8a"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(snare, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//CRASH
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Crash/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Crash/1b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Crash/2a"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(crash, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//HI HAT
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1a_c"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1a_o"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1b_c"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1b_o"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4b"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4c"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(hihat, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();


		//RIDE
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1b"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1c"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Snare/1d"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2c"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2c"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Snare/2d"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Snare/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4b"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4d"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4e"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4f"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Snare/4g"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Snare/5a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Snare/6a"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/Snare/8a"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/Snare/8b"));
		ProbLists[7].Add ((AudioClip)Resources.Load("DrumTracks/Snare/8c"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(ride, new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();


	}
}
