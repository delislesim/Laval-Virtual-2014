using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrumAssistedController : MonoBehaviour {
	public TipFollower tipRight;
	public TipFollower tipLeft;


	public GameObject[] DrumComponentObjects;
	public enum DrumComponentIndexes : int{
		BIGTOM = 0,
		TOM1,
		TOM2,
		SNARE,
		CRASH,
		RIDE,
		HIHAT,
		COUNT
	}

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
	//private bool trackAplaying;
	private bool track2Needed;
	private bool track1Needed;
	private float SAMPLE_TIME;
	private bool choiceNeeded;

	// Use this for initialization
	void Start () {

		FillDictionary();
		baseRythmA.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");
		baseRythmB.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");

		track1_A.clip = TracksCollection[DrumComponentObjects[(int)DrumComponentIndexes.HIHAT]][0][0];
		track1_B.clip = TracksCollection[DrumComponentObjects[(int)DrumComponentIndexes.HIHAT]][0][0];

		track2_A.clip = TracksCollection[DrumComponentObjects[(int)DrumComponentIndexes.SNARE]][0][0];
		track2_B.clip = TracksCollection[DrumComponentObjects[(int)DrumComponentIndexes.SNARE]][0][0];

		trackBplaying = false;
//		trackAplaying = true;
		track1Needed = false;
		track2Needed = false;
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
			MakeTrackChoices();
			choiceNeeded = false;
		}

		//Track Control
		if (elapsedTime >= (SAMPLE_TIME - S_DELAY))
		{
			//Debug.Log ("Elapsed time : " + elapsedTime + ", SAMPLE_TIME : " + SAMPLE_TIME + " , S_DELAY : " + S_DELAY); 
			PlayTracks ();
			resetCoupsEnregistres();
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

	void MakeTrackChoices()
	{
		GameObject closestFromLeft = new GameObject();
		GameObject closestFromRight = new GameObject();

		track1Needed = getClosestDrumComponent(tipLeft, out closestFromLeft);
		track2Needed = getClosestDrumComponent(tipRight, out closestFromRight);

		bool OnSameComponent = closestFromLeft = closestFromRight;
		//No need of 2 tracks if we're hitting the same component with both tips
		track2Needed = (track2Needed && (!OnSameComponent));

		int idxCoups;
		int idxProb;

		if(track1Needed)
		{
			idxCoups = Random.Range(0, TracksCollection[closestFromLeft].Count);
			idxProb = Random.Range(0, TracksCollection[closestFromLeft][idxCoups].Count);

			if(trackBplaying){
				track1_A.clip = TracksCollection[closestFromLeft][idxCoups][idxProb];
			}
			else{
				track1_B.clip = TracksCollection[closestFromLeft][idxCoups][idxProb];
			}
		}
		if(track2Needed)
		{
			idxCoups = Random.Range(0, TracksCollection[closestFromRight].Count);
			idxProb = Random.Range(0, TracksCollection[closestFromRight][idxCoups].Count);

			if(trackBplaying){
				track2_A.clip = TracksCollection[closestFromRight][idxCoups][idxProb];
			}
			else{
				track2_B.clip = TracksCollection[closestFromRight][idxCoups][idxProb];
			}
		}

	}

	//ugly
	//TODO make array of drumComponent gameObjects
	bool getClosestDrumComponent(TipFollower tip,  out GameObject closest)
	{
		float dist = 9999999 ;
		float bestDist = dist;
		GameObject result = null; 

		for(int i = 0 ; i < (int)DrumComponentIndexes.COUNT ; i++)
		{
			dist = Vector3.Distance (tip.transform.position, DrumComponentObjects[i].transform.position);
			if(dist<bestDist){
				bestDist = dist;
				result = DrumComponentObjects[i];
			}
		}
		closest=result;
		return bestDist < 1.5;

	}

	void resetCoupsEnregistres()
	{
		for(int i = 0 ; i < (int)DrumComponentIndexes.COUNT ; i++)
		{
			if(i != (int)DrumComponentIndexes.HIHAT)
				DrumComponentObjects[i].GetComponent<DrumComponent>().ResetCoupsDernierTemps();
		}
		DrumComponentObjects[(int)DrumComponentIndexes.HIHAT].GetComponent<HighHatComponent>().ResetCoupsDernierTemps();
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
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/BigTom/8a"));

		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}

		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.BIGTOM], new List<List<AudioClip>>(ComponentList));

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
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.TOM1], new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//TOM2
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/1a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/2b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/4b"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/6a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/6b"));
	
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.TOM2], new List<List<AudioClip>>(ComponentList));
		
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
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/Snare/8a"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.SNARE], new List<List<AudioClip>>(ComponentList));
		
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
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.CRASH], new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();

		//HI HAT
		//ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1a_c"));
		//ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1a_o"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1b_c"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/1b_o"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/2c"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/HiHat/4c"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.HIHAT], new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();


		//RIDE
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Ride/1a"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Ride/1b"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Ride/1c"));
		ProbLists[0].Add ((AudioClip)Resources.Load("DrumTracks/Ride/1d"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Ride/2a"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Ride/2b"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Ride/2c"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Ride/2c"));
		ProbLists[1].Add ((AudioClip)Resources.Load("DrumTracks/Ride/2d"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Ride/3a"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Ride/3b"));
		ProbLists[2].Add ((AudioClip)Resources.Load("DrumTracks/Ride/3c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4a"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4b"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4c"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4d"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4e"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4f"));
		ProbLists[3].Add ((AudioClip)Resources.Load("DrumTracks/Ride/4g"));
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Ride/5a"));
		ProbLists[5].Add ((AudioClip)Resources.Load("DrumTracks/Ride/6a"));
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/Ride/8a"));
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/Ride/8b"));
		ProbLists[6].Add ((AudioClip)Resources.Load("DrumTracks/Ride/8c"));
		
		for(int i = 0 ; i < 8 ; i++)
		{
			ComponentList.Add (new List<AudioClip>(ProbLists[i]));
		}
		
		TracksCollection.Add(DrumComponentObjects[(int)DrumComponentIndexes.RIDE], new List<List<AudioClip>>(ComponentList));
		
		// **  CLEAR  ** //
		for(int i = 0 ; i < 8 ; i++)
		{
			ProbLists[i].Clear(); //******
		}
		ComponentList.Clear();


	}
}
