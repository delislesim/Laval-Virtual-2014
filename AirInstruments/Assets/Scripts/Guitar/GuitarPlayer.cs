using UnityEngine;
using System.Collections;

public class GuitarPlayer : MonoBehaviour {

	public AudioClip[] HighVelocityNotes;
	public AudioClip[] LowVelocityNotes;
	public AudioClip[] MedVelocityNotes;

	public enum Joint : int
	{
		E = 0, F, Gb, G, Ab, A, Bb, B, C, Db, D
	}

	private int dummy_counter;

	// Use this for initialization
	void Start () {
		dummy_counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
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

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Trigger with guitar player! : " + col.gameObject.tag);
		if(col.gameObject.tag == "PlayHand")
			PlayNextRandomNote();
	}
}
