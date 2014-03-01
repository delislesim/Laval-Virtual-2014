using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	public GameObject objet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			objet.transform.position = -objet.transform.forward + objet.transform.position;
						//objet.transform.Translate (-objet.transform.forward);
				}
		if (Input.GetKey (KeyCode.DownArrow)) {
			objet.transform.position = objet.transform.forward + objet.transform.position;
		}
		if (Input.GetKey (KeyCode.Space)) {
			objet.rigidbody.AddRelativeForce (-objet.transform.forward * 100, ForceMode.Acceleration);
		}
	}
}
