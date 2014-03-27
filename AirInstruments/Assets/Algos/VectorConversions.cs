using System;
using UnityEngine;

public class VectorConversions
{
	public static Vector3 CalculerWorldScale(Transform transform) {
		Vector3 worldScale = transform.localScale;
		Transform parent = transform.parent;
		while (parent != null) {
			worldScale = Vector3.Scale(worldScale,parent.localScale);
			parent = parent.parent;
		}
		return worldScale;
	}

	public static Quaternion RotationDroite(Vector3 extA, Vector3 extB) {
		Vector3 direction = extB - extA;
		Quaternion rotation = Quaternion.FromToRotation (Vector3.up, direction);
		return rotation;
	}
}

