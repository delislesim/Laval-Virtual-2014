using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

	public float ecartOctave = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	//	if(Input.GetButton(key)){
	//		Debug.Log ("Key pressed!");
	//	}
	}

	void OnMouseDown()
	{
		audio.pitch = Mathf.Pow(2.0f, ecartOctave);
		audio.Play();
	}

	void OnTriggerEnter(Collider other) {
		audio.pitch = Mathf.Pow(2.0f, ecartOctave);
		audio.Play();
		Debug.Log (ecartOctave);
	}
}