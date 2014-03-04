using UnityEngine;
using System.Collections.Generic;

public class ColliderManager : MonoBehaviour {

	// Plan situe sur la surface du composant a jouer.
	public GameObject plane;
	
	public float DistanceToPoint (Vector3 point) {
		Vector3 normal = plane.transform.rotation * Vector3.up;
		Plane planeMath = new Plane (normal.normalized, plane.transform.position);
		return planeMath.GetDistanceToPoint (point);
	}
}
