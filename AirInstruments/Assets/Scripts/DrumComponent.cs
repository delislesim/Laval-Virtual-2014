using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour {
	public string inputNote;


	public void PlaySound()
	{
		audio.Play();
	}

	// Use this for initialization
	void Start () {
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

	void OnMouseDown()
	{
		PlaySound();
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Tip")
			PlaySound();
	}
}