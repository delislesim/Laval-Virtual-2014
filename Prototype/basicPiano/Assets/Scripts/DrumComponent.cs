using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseDown()
	{
		audio.Play();
		Debug.Log ("YOLOS");
	}
	
	void OnMouseUp()
	{
		//audio.Stop();
	}
}