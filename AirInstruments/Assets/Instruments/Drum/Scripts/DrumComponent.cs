using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour {

	public void PlaySound()
	{
		audio.Play();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		PlaySound();
	}
	
	void OnCollisionEnter(Collision col)
	{
		/*if(col.gameObject.tag == "Tip")
			PlaySound();*/
	}
}