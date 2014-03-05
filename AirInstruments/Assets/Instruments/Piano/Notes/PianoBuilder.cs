using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoBuilder : MonoBehaviour, Instrument {

	// Clips audio.
	public List<AudioClip> clips;

	// Prefab de note blanche.
	public GameObject blanchePrefab;

	// Prefab de note noire.
	public GameObject noirePrefab;

	// Notes du piano.
	private List<PianoNote> notes = new List<PianoNote>();

	// Game objects des notes du piano.
	private List<GameObject> notesGameObject = new List<GameObject>();


	void Start () {
		// Créer le piano.
		CreerBlanche (-7, clips [0], 0);  // Do
		CreerNoire   (-7, clips [0], 1);  // Do#
		CreerBlanche (-6, clips [0], 2);  // Re
		CreerNoire   (-6, clips [0], 3);  // Re#
		CreerBlanche (-5, clips [0], 4);  // Mi
		CreerBlanche (-4, clips [1], 0);  // Fa
		CreerNoire   (-4, clips [1], 1);  // Fa#
		CreerBlanche (-3, clips [1], 2);  // Sol
		CreerNoire   (-3, clips [1], 3);  // Sol#
		CreerBlanche (-2, clips [2], -2);  // La
		CreerNoire   (-2, clips [2], -1);  // La#
		CreerBlanche (-1, clips [2], 0);  // Si

		CreerBlanche (0, clips [2], 1);  // Do
		CreerNoire   (0, clips [2], 2);  // Do#
		CreerBlanche (1, clips [3], 0);  // Re
		CreerNoire   (1, clips [3], 1);  // Re#
		CreerBlanche (2, clips [3], 2);  // Mi
		CreerBlanche (3, clips [3], 3);  // Fa
		CreerNoire   (3, clips [4], 0);  // Fa#
		CreerBlanche (4, clips [4], 1);  // Sol
		CreerNoire   (4, clips [4], 2);  // Sol#
		CreerBlanche (5, clips [5], 0);  // La
		CreerNoire   (5, clips [5], 1);  // La#
		CreerBlanche (6, clips [5], 2);  // Si

		CreerBlanche (7, clips [6], -1);  // Do
		CreerNoire   (7, clips [6], 0);  // Do#
		CreerBlanche (8, clips [6], 1);  // Re
		CreerNoire   (8, clips [6], 2);  // Re#
		CreerBlanche (9, clips [7], 0);  // Mi
		CreerBlanche (10, clips [7], 1);  // Fa
		CreerNoire   (10, clips [7], 2);  // Fa#
		CreerBlanche (11, clips [7], 3);  // Sol
		CreerNoire   (11, clips [8], -1);  // Sol#
		CreerBlanche (12, clips [8], 0);  // La
		CreerNoire   (12, clips [8], 1);  // La#
		CreerBlanche (13, clips [8], 2);  // Si

		CreerBlanche (14, clips [9], -1);  // Do
		CreerNoire   (14, clips [9], 0);  // Do#
		CreerBlanche (15, clips [9], 1);  // Re
		CreerNoire   (15, clips [9], 2);  // Re#
		CreerBlanche (16, clips [10], -1);  // Mi
		CreerBlanche (17, clips [10], 0);  // Fa
		CreerNoire   (17, clips [10], 1);  // Fa#
		CreerBlanche (18, clips [11], -1);  // Sol
		CreerNoire   (18, clips [11], 0);  // Sol#
		CreerBlanche (19, clips [11], 1);  // La
		CreerNoire   (19, clips [11], 2);  // La#
		CreerBlanche (20, clips [11], 3);  // Si

		CreerBlanche (21, clips [12], 1);  // Do
		CreerNoire   (21, clips [12], 2);  // Do#
		CreerBlanche (22, clips [12], 3);  // Re
		CreerNoire   (22, clips [9], 2);  // Re#
		CreerBlanche (23, clips [10], -1);  // Mi
		CreerBlanche (24, clips [10], 0);  // Fa
		CreerNoire   (24, clips [10], 1);  // Fa#
		CreerBlanche (25, clips [11], -1);  // Sol
		CreerNoire   (25, clips [11], 0);  // Sol#
		CreerBlanche (26, clips [11], 1);  // La
		CreerNoire   (26, clips [11], 2);  // La#
		CreerBlanche (27, clips [11], 3);  // Si

		CreerBlanche (28, clips [9], -1);  // Do
		CreerNoire   (28, clips [9], 0);  // Do#
		CreerBlanche (29, clips [9], 1);  // Re
		CreerNoire   (29, clips [9], 2);  // Re#
		CreerBlanche (30, clips [10], -1);  // Mi
		CreerBlanche (31, clips [10], 0);  // Fa
		CreerNoire   (31, clips [10], 1);  // Fa#
		CreerBlanche (32, clips [11], -1);  // Sol
		CreerNoire   (32, clips [11], 0);  // Sol#
		CreerBlanche (33, clips [11], 1);  // La
		CreerNoire   (33, clips [11], 2);  // La#
		CreerBlanche (34, clips [11], 3);  // Si
	}

	private int countUpdate = 0;

	void Update () {
		// Hack pour eviter d'avoir des notes deformees.
		++countUpdate;
		if (countUpdate == 2) {
			for (int i = 0; i < notesGameObject.Count; ++i) {
				notesGameObject[i].SetActive(true);
			}
		}
	}

	void CreerBlanche(int position, AudioClip clip, float ecartDemiTon) {
		GameObject note = (GameObject)Instantiate (blanchePrefab);
		note.transform.parent = this.gameObject.transform;
		note.transform.localPosition = new Vector3 (position * spaceBetweenWhiteNotes, 
		                                            blanchePrefab.transform.position.y,
		                                            blanchePrefab.transform.position.z);
		note.transform.localScale = blanchePrefab.transform.localScale;
		note.transform.localRotation = blanchePrefab.transform.localRotation;
		note.SetActive (false);
		notesGameObject.Add (note);

		AudioSource audioSource = (AudioSource)note.GetComponent (typeof(AudioSource));
		audioSource.clip = clip;

		PianoNote pianoNote = (PianoNote)note.GetComponent (typeof(PianoNote));
		pianoNote.ecartDemiTon = ecartDemiTon;

		notes.Add (pianoNote);
		/*
		if (position < 7) {
			// Rendre la note invisible.
			note.transform.localPosition = kPositionInvalide;
		}
		*/
	}

	void CreerNoire(int position, AudioClip clip, float ecartDemiTon) {
		GameObject note = (GameObject)Instantiate (noirePrefab);
		note.transform.parent = this.gameObject.transform;
		note.transform.localPosition = new Vector3 (((position * 2) + 1) * spaceBetweenBlackNotes,
		                                            noirePrefab.transform.position.y,
		                                            noirePrefab.transform.position.z);
		note.transform.localScale = noirePrefab.transform.localScale;
		note.transform.localRotation = noirePrefab.transform.localRotation;
		note.SetActive (false);
		notesGameObject.Add (note);
		
		AudioSource audioSource = (AudioSource)note.GetComponent (typeof(AudioSource));
		audioSource.clip = clip;
		
		PianoNote pianoNote = (PianoNote)note.GetComponent (typeof(PianoNote));
		pianoNote.ecartDemiTon = ecartDemiTon;
		
		notes.Add (pianoNote);

		/*
		if (position < 7) {
			// Rendre la note invisible.
			note.transform.localPosition = kPositionInvalide;
		}
		*/
	}

	public void DefinirStatutNote(int index, PartitionPiano.StatutNote statut) {
		notes [index].DefinirStatut (statut);
	}

	public void ObtenirInfoNotePourCubesTombants(int index, out float positionHorizontale, out float largeur) {
		notes [index].ObtenirInfoPourCubesTombants (out positionHorizontale, out largeur);
	}

	public PianoNote ObtenirNote(int index) {
		return notes [index];
	}

	// Espace horizontal entre le centre de 2 notes blanches.
	private const float spaceBetweenWhiteNotes = 1.1f;

	// Espace horizontal entre le centre de 2 notes noires.
	private const float spaceBetweenBlackNotes = spaceBetweenWhiteNotes / 2.0f;

	// Position invalide.
	private Vector3 kPositionInvalide = new Vector3(0, 100.0f, 0);
}
