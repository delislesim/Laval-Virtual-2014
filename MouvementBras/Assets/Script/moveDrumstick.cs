using UnityEngine;
using System.Collections;

public class moveDrumstick : MonoBehaviour {
	public GameObject mainDroite;
	public GameObject mainGauche;
	public GameObject capsule;

	private int compteur;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			//capsule.rigidbody.AddRelativeForce(capsule.transform.up * 100, ForceMode.Acceleration);
			if(compteur%2 == 0){
				mainDroite.transform.Translate(0,1,0);
			} else {
				mainDroite.transform.Translate(0,-1,0);
			}
			compteur++;
		}
	}
}
