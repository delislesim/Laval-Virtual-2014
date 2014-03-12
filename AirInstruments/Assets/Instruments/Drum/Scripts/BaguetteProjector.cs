using UnityEngine;
using System.Collections;

public class BaguetteProjector : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.rotation = kRotationWorld;
	}

	// Quaternion de la rotation des projecteur par rapport au monde.
	private Quaternion kRotationWorld = Quaternion.Euler (new Vector3 (90.0f, 0, 0));
}
