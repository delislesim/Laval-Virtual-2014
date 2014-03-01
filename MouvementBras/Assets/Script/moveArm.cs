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
		if(Input.GetKey(KeyCode.Space)){
			//Debug.Log("Application de la force");
		    main.rigidbody.AddForce (main.transform.forward * 1000, ForceMode.Acceleration);
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
						epaule.transform.position = epaule.transform.forward + epaule.transform.position;
						bicep.transform.position = bicep.transform.forward + bicep.transform.position;
						coude.transform.position = coude.transform.forward + coude.transform.position;
				}

		if (Input.GetKey (KeyCode.DownArrow)) {
			epaule.transform.position = -epaule.transform.forward + epaule.transform.position;
			bicep.transform.position = -bicep.transform.forward + bicep.transform.position;
			coude.transform.position = -coude.transform.forward + coude.transform.position;
		}
	}
}
