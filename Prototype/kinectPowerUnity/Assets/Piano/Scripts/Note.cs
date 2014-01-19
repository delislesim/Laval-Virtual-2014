using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	
	public float ecartOctave = 0;
	public string inputNote;
	public Material neutre;
	public Material touchee;

	// Indique si la note vient d'etre enfoncée.
	private bool enfoncee_start = false;

	// Indique si la note est déja enfoncée.
	private bool enfoncee = false;

	public void SetEnfoncee(bool v) {
		if (v) {
			if (!enfoncee) {
				this.enfoncee_start = true;
			}
		} else {
			this.enfoncee_start = false;
			this.enfoncee = false;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(enfoncee_start){
			audio.pitch = Mathf.Pow(2.0f, ecartOctave);
			audio.Play();
			enfoncee_start = false;
			enfoncee = true;
		}
		
		if(!enfoncee){
			audio.Stop();
		}
		
	}
	
	void OnMouseDown()
	{
		transform.Translate(0,-0.35f,-0.1f);
		audio.pitch = Mathf.Pow(2.0f, ecartOctave);
		audio.Play();
		renderer.material=touchee;
	}
	
	void OnMouseUp()
	{
		audio.Stop();
		transform.Translate(0,0.35f,0.1f);
		renderer.material=neutre;
	}
}