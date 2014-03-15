using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DrumAssistedController : MonoBehaviour {
	public TipFollower tipRight;
	public TipFollower tipLeft;
	public DrumComponent bigTom;
	public DrumComponent tom1;
	public DrumComponent tom2;
	public DrumComponent snare;
	public DrumComponent crash;
	public DrumComponent ride;
	public DrumComponent hihat;

	public Dictionary<DrumComponent, List<List<AudioClip>>> TracksCollection = new Dictionary<DrumComponent, List<List<AudioClip>>>();

	/*
	public struct DrumTrack{
		public AudioClip track;
		public DrumComponent component;
		public int hitNumber;
		public float probability; 
		
		// Constructor:
		public DrumTrack(string trackFileName, DrumComponent comp, int hitNb, float prob) 
		{
			this.track = (AudioClip)Resources.Load (trackFileName);
			this.component = comp;
			this.hitNumber = hitNb;
			this.probability = prob;
		}
	}
	*/	

	// Use this for initialization
	void Start () {

		FillDictionary();
		audio.clip = TracksCollection[bigTom][3][0];
		audio.clip = ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\1a.mp3"));
		audio.loop = true;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
		//CHECK FOR EMPTY LISts
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

		//BIG TOMD										\Assets\Instruments\Drum\DrumTracks\BigTom
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\1a.mp3"));
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\1b.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\2a.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\2b.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\2c.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\3a.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\3b.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\3c.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\4a.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\4b.mp3"));
		ProbLists[4].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\5a.mp3"));
		ProbLists[5].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\6a.mp3"));
		ProbLists[7].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\BigTom\\8a.mp3"));

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
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\1a.mp3"));
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\1b.mp3"));
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\1c.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\2a.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\2b.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\2c.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3a.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3b.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3c.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3d.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\4a.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\4b.mp3"));
		ProbLists[4].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\5a.mp3"));
		ProbLists[5].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\6a.mp3"));
		ProbLists[6].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\8a.mp3"));
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
		ProbLists[0].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\1a.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\2a.mp3"));
		ProbLists[1].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\2b.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3a.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3b.mp3"));
		ProbLists[2].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\3c.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\4a.mp3"));
		ProbLists[3].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\4b.mp3"));
		ProbLists[5].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\6a.mp3"));
		ProbLists[7].Add ((AudioClip)Resources.Load(".\\Assets\\Instruments\\Drum\\DrumTracks\\Tom1\\6b.mp3"));
	
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

	}
}
