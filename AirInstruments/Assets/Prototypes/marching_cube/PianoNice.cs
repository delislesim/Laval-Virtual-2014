using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoNice : MonoBehaviour {

	public List<GameObject> boules;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		KinectPowerInterop.GetPianoFingers (buffer, known);

		for (int i = 0; i < 25; ++i) {
			if (known[i] != 0) {
				int x = (int)buffer[i*3 + 0];
				int y = (int)buffer[i*3 + 1];
				int depth = (int)buffer[i*3 + 2];

				boules[i].transform.localPosition = new Vector3(
					((float)x/640.0f)*32f - 16f,
					-8-(((float)y/640.0f)*32f - 16f),
					0f);
			} else {
				boules[i].transform.localPosition = new Vector3(
					-100f,
					-100f,
					0f);
			}
		}
	}

	private uint[] buffer = new uint[25 * 3];
	private byte[] known = new byte[25];
}
