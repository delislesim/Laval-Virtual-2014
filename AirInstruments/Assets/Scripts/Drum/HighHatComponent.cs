using UnityEngine;
using System.Collections;

public class HighHatComponent : MonoBehaviour {
	public bool opened;
	public AudioClip soundOpened;
	public AudioClip soundClosed;
	
	public void PlaySound()
	{
		if(opened){
			audio.clip = soundOpened;
			audio.Play();
		}
		else{
			audio.clip = soundClosed;
			audio.Play();
		}
	}
	
	// Use this for initialization
	void Start () {
		opened = false;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Tip")
			PlaySound();
	}
	
	void OnMouseDown()
	{
		PlaySound();
	}
}