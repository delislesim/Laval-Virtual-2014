using UnityEngine;
using System.Collections;

public class HandCylinder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 worldScaleVector = VectorConversions.CalculerWorldScale (transform.parent);
		worldScale = worldScaleVector.x;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Ajuste la position, taille et rotation du cylindre pour que ses extremites
	// touchent les points specifies.
	public void DefinirExtremites(Vector3 extA, Vector3 extB) {
		Vector3 centre = (extA + extB) / 2.0f;
		Vector3 direction = extB - extA;
		if (direction == Vector3.zero)
			return;

		Quaternion rotation = Quaternion.FromToRotation (Vector3.up, direction);
		Vector3 scale = new Vector3(transform.localScale.x,
		                            direction.magnitude / (2.0f * worldScale),
		                            transform.localScale.z);
		if (scale.y == float.PositiveInfinity || scale.y == float.NegativeInfinity) {
			// Eviter division par zero.
			scale.y = 0.0f;
		}

		// Appliquer les transformations.
		transform.position = centre;
		transform.rotation = rotation;
		transform.localScale = scale;
	}

	// Facteur de multiplication de notre scale par rapport au monde, au repos.
	private float worldScale;
}
