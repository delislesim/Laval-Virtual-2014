using UnityEngine;
using System.Collections;

public class LineWaveGuitar : MonoBehaviour {

	public Transform target;

	public void SetLength(float length) {
		Vector3 localPosition = target.localPosition;
		localPosition.x = length;
		target.localPosition = localPosition;
	}

	public float GetLength() {
		return target.localPosition.x;
	}
}
