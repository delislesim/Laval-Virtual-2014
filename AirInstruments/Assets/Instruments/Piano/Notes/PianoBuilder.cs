using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoBuilder : MonoBehaviour {

	// Clips audio.
	public List<AudioClip> clips;

	// Prefab de note blanche.
	public GameObject blanchePrefab;

	// Prefab de note noire.
	public GameObject noirePrefab;

	// Notes blanches du piano.
	private List<GameObject> blanches = new List<GameObject>();

	// Notes noires du piano.
	private List<GameObject> noires = new List<GameObject>();


	void Start () {
		// Créer le piano.
		CreerBlanche (0, clips [0], 1);  // Do
		CreerNoire   (0, clips [0], 2);  // Do#
		CreerBlanche (1, clips [1], 0);  // Re
		CreerNoire   (1, clips [1], 1);  // Re#
		CreerBlanche (2, clips [1], 2);  // Mi
		CreerBlanche (3, clips [1], 3);  // Fa
		CreerNoire   (3, clips [2], 0);  // Fa#
		CreerBlanche (4, clips [2], 1);  // Sol
		CreerNoire   (4, clips [2], 2);  // Sol#
		CreerBlanche (5, clips [3], 0);  // La
		CreerNoire   (5, clips [3], 1);  // La#
		CreerBlanche (6, clips [3], 2);  // Si

		CreerBlanche (7, clips [4], -1);  // Do
		CreerNoire   (7, clips [4], 0);  // Do#
		CreerBlanche (8, clips [4], 1);  // Re
		CreerNoire   (8, clips [4], 2);  // Re#
		CreerBlanche (9, clips [5], 0);  // Mi
		CreerBlanche (10, clips [5], 1);  // Fa
		CreerNoire   (10, clips [5], 2);  // Fa#
		CreerBlanche (11, clips [5], 3);  // Sol
		CreerNoire   (11, clips [6], -1);  // Sol#
		CreerBlanche (12, clips [6], 0);  // La
		CreerNoire   (12, clips [6], 1);  // La#
		CreerBlanche (13, clips [6], 2);  // Si

		CreerBlanche (14, clips [7], -1);  // Do
		CreerNoire   (14, clips [7], 0);  // Do#
		CreerBlanche (15, clips [7], 1);  // Re
		CreerNoire   (15, clips [7], 2);  // Re#
		CreerBlanche (16, clips [8], -1);  // Mi
		CreerBlanche (17, clips [8], 0);  // Fa
		CreerNoire   (17, clips [8], 1);  // Fa#
		CreerBlanche (18, clips [9], -1);  // Sol
		CreerNoire   (18, clips [9], 0);  // Sol#
		CreerBlanche (19, clips [9], 1);  // La
		CreerNoire   (19, clips [9], 2);  // La#
		CreerBlanche (20, clips [9], 3);  // Si
	}

	void Update () {
	
	}

	void CreerBlanche(int position, AudioClip clip, float ecartDemiTon) {
		GameObject note = (GameObject)Instantiate (blanchePrefab);
		note.transform.parent = this.gameObject.transform;
		note.transform.localPosition = new Vector3 (position * spaceBetweenWhiteNotes, 
		                                            blanchePrefab.transform.position.y,
		                                            blanchePrefab.transform.position.z);
		note.transform.localScale = blanchePrefab.transform.localScale;

		AudioSource audioSource = (AudioSource)note.GetComponent (typeof(AudioSource));
		audioSource.clip = clip;

		PianoNote pianoNote = (PianoNote)note.GetComponent (typeof(PianoNote));
		pianoNote.ecartDemiTon = ecartDemiTon;

		blanches.Add (note);
	}

	void CreerNoire(int position, AudioClip clip, float ecartDemiTon) {
		GameObject note = (GameObject)Instantiate (noirePrefab);
		note.transform.parent = this.gameObject.transform;
		note.transform.localPosition = new Vector3 (((position * 2) + 1) * spaceBetweenBlackNotes,
		                                            noirePrefab.transform.position.y,
		                                            noirePrefab.transform.position.z);
		note.transform.localScale = noirePrefab.transform.localScale;
		
		AudioSource audioSource = (AudioSource)note.GetComponent (typeof(AudioSource));
		audioSource.clip = clip;
		
		PianoNote pianoNote = (PianoNote)note.GetComponent (typeof(PianoNote));
		pianoNote.ecartDemiTon = ecartDemiTon;
		
		noires.Add (note);
	}

	// Espace horizontal entre le centre de 2 notes blanches.
	private const float spaceBetweenWhiteNotes = 1.15f;

	// Espace horizontal entre le centre de 2 notes noires.
	private const float spaceBetweenBlackNotes = 0.575f;
}
