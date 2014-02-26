using UnityEngine;
using System.Collections;

public class moveArm : MonoBehaviour {
	public GameObject epaule;
	public GameObject main;
	public GameObject coude;
	public GameObject avantBras;
	public GameObject bicep;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			Debug.Log("Application de la force");
		    main.rigidbody.AddForce (main.transform.up * 1000, ForceMode.Acceleration);
		}
	}
}
