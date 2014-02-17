using UnityEngine;
using System.Collections;

public class FingerSphere : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Trouver des objets en collision avec cette boule.
		Collider[] hitColliders = Physics.OverlapSphere (transform.localPosition, 0.5f);
		for (int i = 0; i < hitColliders.Length; ++i) {
			Collider collider = hitColliders[i];
			PianoNote note = collider.GetComponent<PianoNote>();
			if (note != null) {
				note.TouchWithSphere(this);
			}
		}
	}
}
