using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DrumAssistedController : MonoBehaviour {

	public static DrumAssistedController ObtenirInstance() {
		return instance;
	}
	public static void DefinirInsance(DrumAssistedController inst) {
		instance = inst;
	}

	private static DrumAssistedController instance;

	public TipFollower tipRight;
	public TipFollower tipLeft;

	public FeuDrum feuDrum;

	public DrumComponent[] DrumComponentObjects;
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

	private Dictionary<DrumComponent, List<List<AudioClip>>> TracksCollection = new Dictionary<DrumComponent, List<List<AudioClip>>>();

	private float elapsedTime;
	private const float S_DELAY = 0.1f;
	private const float MIN_DIST = 2.5f;
	private const float MIN_SPEED = 1.0f;
	private const int BEAT_MEMORY = 1;
	private bool trackBplaying;
	//private bool trackAplaying;
	private bool baseTrackNeeded;
	private bool track2Needed;
	private bool track1Needed;
	private float SAMPLE_TIME;
	private int memLeft;
	private int memRight;

	// Indique si le mode assiste est active.
	private static bool estActive;

	// Use this for initialization
	void Start () {
		instance = this;

		FillDictionary();
		baseRythmA.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");
		baseRythmB.clip = (AudioClip)Resources.Load("DrumTracks/BaseRythm");
		//track1_A.clip = TracksCollection[DrumComponentObjects[(int)DrumComponentIndexes.HIHAT]][1][2];
		SAMPLE_TIME = baseRythmA.clip.length;
	}

	void OnEnable(){
		baseTrackNeeded = false;
		trackBplaying = false;
		//		trackAplaying = true;
		track1Needed = false;
		track2Needed = false;
		elapsedTime = 0;

		estActive = true;
	}

	void OnDisable() {
		estActive = false;
	}

	public static bool EstActive() {
		return estActive;
	}

	// Update is called once per frame
	void Update () {
		elapsedTime = elapsedTime + Time.deltaTime;

		//Track Control
		if (elapsedTime >= (SAMPLE_TIME - S_DELAY))
		{
			//Debug.Log ("Elapsed time : " + elapsedTime + ", SAMPLE_TIME : " + SAMPLE_TIME + " , S_DELAY : " + S_DELAY);
			MakeTrackChoices();
			PlayTracks ();
			resetCoupsEnregistres();
			elapsedTime = 0;
		}

		if(memLeft > BEAT_MEMORY && memRight > BEAT_MEMORY)
			baseTrackNeeded = false;
		else
			baseTrackNeeded = true;
	}

	void PlayTracks()
	{
		//ALTERNATE A and B so transitions are fluid and on time
		if(trackBplaying){
			if(baseTrackNeeded)
				baseRythmA.Play();
			
			if(track1Needed)
				track1_A.Play ();
			
			if(track2Needed)
				track2_A.Play ();
			

			trackBplaying=false;
		}
		else{
			if(baseTrackNeeded)
				baseRythmB.Play();
			
			if(track1Needed)
				track1_B.Play ();
			
			if(track2Needed)
				track2_B.Play ();
			
			trackBplaying=true;
		}
	}

	void MakeTrackChoices()
	{
		DrumComponent closestFromLeft;
		DrumComponent closestFromRight;

		track1Needed = getClosestDrumComponent(tipLeft, out closestFromLeft);
		track2Needed = getClosestDrumComponent(tipRight, out closestFromRight);

		bool OnSameComponent = (closestFromLeft == closestFromRight && track1Needed);
		//No need of 2 tracks if we're hitting the same component with both tips
		//track2Needed = (track2Needed && (!OnSameComponent));
		if(OnSameComponent)
			track2Needed = false;

		int idxCoups = 0;
		int idxProb = 0;
		int bonusForBothHands = OnSameComponent ? 1 : 0;

		if(track1Needed) // LEFT HAND
		{
			memLeft = 0;
			try {
				//nombre de coups a jouer
				int nbCoups = Mathf.Min(closestFromLeft.GetCoupsDernierTemps()  + bonusForBothHands , TracksCollection[closestFromLeft].Count-1);
				idxCoups = nbCoups;

				// Feu.
				if (nbCoups == TracksCollection[closestFromLeft].Count-1) {
					feuDrum.Burst();
				}

				//Choisir track a, b, .... Les chances réduisent linéairement
				List<int> idxList = new List<int>();
				for(int i = 0 ; i < TracksCollection[closestFromLeft][idxCoups].Count ; i++){
					for(int j = 0 ; j < TracksCollection[closestFromLeft][idxCoups].Count - i ; j++){
						idxList.Add(i);
					}
				}

				idxProb = idxList[UnityEngine.Random.Range(0, idxList.Count)];
				//Debug.Log("LEFT NAME : " + closestFromLeft.name + ", NB COUPS: " + nbCoups);
				//Debug.Log("COUNT : " + TracksCollection[closestFromLeft][idxCoups].Count + ", IDX PROB : " + idxProb);
				//Debug.Log( "COUNT PROB + " + TracksCollection[closestFromdLeft][idxCoups].Count + ",  PROB : " + idxProb);

				setTrack1(TracksCollection[closestFromLeft][idxCoups][idxProb]);
			}
			catch (Exception e) {
				Debug.Log("OOPS!  " + e.Message);
			}
		}
		else
		{
			memLeft++;
			if (memLeft >= BEAT_MEMORY)
				tipLeft.resetLastOldComponentMemory();
				
		}
		tipLeft.resetLastComponentMemory();

		if(track2Needed) //RIGHT HAND
		{
			memRight = 0;
			
			int nbCoups = Mathf.Min(closestFromRight.GetCoupsDernierTemps() + bonusForBothHands, TracksCollection[closestFromRight].Count-1);
			idxCoups = nbCoups;

			// Feu.
			if (nbCoups == TracksCollection[closestFromRight].Count-1) {
				feuDrum.Burst();
			}

			List<int> idxList = new List<int>();
			for(int i = 0 ; i < TracksCollection[closestFromRight][idxCoups].Count ; i++){
				for(int j = 0 ; j < TracksCollection[closestFromRight][idxCoups].Count - i ; j++){
					idxList.Add(i);
				}
			}
			
			idxProb = idxList[UnityEngine.Random.Range(0, idxList.Count)];
			//Debug.Log("RIGHT NAME : " + closestFromRight.name + ", NB COUPS: " + nbCoups);
			//Debug.Log("COUNT : " + TracksCollection[closestFromRight][idxCoups].Count + ", IDX PROB : " + idxProb);

			setTrack2(TracksCollection[closestFromRight][idxCoups][idxProb]);
		}
		else
		{
			memRight++;
			if (memRight >= BEAT_MEMORY)
			tipRight.resetLastOldComponentMemory();
		}
		tipRight.resetLastComponentMemory();

	}
	
	bool getClosestDrumComponent(TipFollower tip,  out DrumComponent closest)
	{
		closest = tip.GetLastComponentHit();
		return (closest != null);
	}

	void setTrack1(AudioClip clip){
		if(trackBplaying){
			track1_A.clip = clip;
		}
		else{
			track1_B.clip = clip;
		}
	}

	void setTrack2(AudioClip clip){
		if(trackBplaying){
			track2_A.clip = clip;
		}
		else{
			track2_B.clip = clip;
		}
	}

	void resetCoupsEnregistres()
	{
		for(int i = 0 ; i < (int)DrumComponentIndexes.COUNT ; i++)
		{
			DrumComponentObjects[i].GetComponent<DrumComponent>().ResetCoupsDernierTemps();
		}
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
		for(int i = 0 ; i < 7 ; i++)
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
		for(int i = 0 ; i < 7 ; i++)
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
		ProbLists[4].Add ((AudioClip)Resources.Load("DrumTracks/Tom2/6b"));
		for(int i = 0 ; i < 5 ; i++)
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
		for(int i = 0 ; i < 7 ; i++)
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
		for(int i = 0 ; i < 1 ; i++)
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
		for(int i = 0 ; i < 3 ; i++)
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
		for(int i = 0 ; i < 7 ; i++)
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
