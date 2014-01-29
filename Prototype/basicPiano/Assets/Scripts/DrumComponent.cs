using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour {
	public string inputNote;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown(inputNote)){
			audio.Play();
			//renderer.material=touchee;
		}
		
		if(Input.GetButtonUp(inputNote)){
			//transform.Translate(0,0.35f,0.1f);
			//audio.Stop();
			//renderer.material=neutre;
		}
		
	}

	void OnTriggerEnter()
	{
		audio.Play();
	}

	void OnMouseDown()
	{
		audio.Play();
	}
	
	void OnMouseUp()
	{
		//audio.Stop();
	}
}