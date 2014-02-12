using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoNice : MonoBehaviour {

	public GameObject plane;

	public List<GameObject> boules;


	// Use this for initialization
	void Start () {
		contoursTexture = new Texture2D(640, 280);
		for (int i = 0; i < 640 * 280; ++i) {
			streamColors[i] = new Color32();
		}
		contoursTexture.SetPixels32(streamColors);
		contoursTexture.Apply();
		
		plane.renderer.material.mainTexture = contoursTexture;
	}
	
	// Update is called once per frame
	void Update () {
		// Mettre a jour les boules rouges.
		KinectPowerInterop.GetPianoFingers (buffer, known);

		for (int i = 0; i < 25; ++i) {
			if (known[i] != 0) {
				int x = (int)buffer[i*3 + 0];
				int y = (int)buffer[i*3 + 1];
				int depth = (int)buffer[i*3 + 2];

				boules[i].transform.localPosition = new Vector3(
					((float)x/640.0f)*32f - 16f,
					-10-(((float)y/640.0f)*32f - 16f),
					- ((float)depth / 50.0f) + 20.0f);
			} else {
				boules[i].transform.localPosition = new Vector3(
					-100f,
					-100f,
					0f);
			}
		}

		// Mettre a jour l'affichage de contours.
		KinectPowerInterop.GetPianoImage(contoursBuffer, (uint)contoursBuffer.Length);
		for (int i = 0; i < 640 * 280; ++i) {
			byte value = contoursBuffer[ReverseIndex(i) * 4];
			streamColors[i].r = 0;
			streamColors[i].g = 0;
			streamColors[i].b = 255;
			streamColors[i].a = value;
		}
		contoursTexture.SetPixels32(streamColors);
		contoursTexture.Apply();
	}

	private int ReverseIndex(int src_index) {
		int row = src_index / 640;
		int col = src_index % 640;
		
		return (/*280 - row - 1*/ row) * 640 +
			(640 - col - 1);
	}

	// Buffers pour les boules rouges.
	private uint[] buffer = new uint[25 * 3];
	private byte[] known = new byte[25];

	// Buffers pour l'image de contours.
	private Texture2D contoursTexture;
	private byte[] contoursBuffer = new byte[640 * 280 * 4];
	private Color32[] streamColors = new Color32[640 * 280];
}
