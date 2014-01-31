using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	
	public float ecartDemiTon = 0;
	public string inputNote;
	public Material neutre;
	public Material touchee;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown(inputNote)){
			transform.Translate(0,-0.35f,-0.1f);
			audio.pitch = Mathf.Pow(2.0f, ecartDemiTon/12);
			audio.Play();
			renderer.material=touchee;
		}
		
		if(Input.GetButtonUp(inputNote)){
			transform.Translate(0,0.35f,0.1f);
			audio.Stop();
			renderer.material=neutre;
		}
		
	}
	
	void OnMouseDown()
	{
		transform.Translate(0,-0.35f,-0.1f);
		audio.pitch = Mathf.Pow(2.0f, ecartDemiTon/12);
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