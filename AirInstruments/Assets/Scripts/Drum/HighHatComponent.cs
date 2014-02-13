using UnityEngine;
using System.Collections;

public class HighHatComponent : MonoBehaviour {
	public string inputNote;
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
		if(Input.GetButtonDown(inputNote)){
			PlaySound();
			//renderer.material=touchee;
		}
		
		if(Input.GetButtonUp(inputNote)){
			//transform.Translate(0,0.35f,0.1f);
			//audio.Stop();
			//renderer.material=neutre;
		}
		
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