using UnityEngine;
using System.Collections;

public class MarchingHandScript : MonoBehaviour {

	public MeshRenderer plane;

	// Use this for initialization
	void Start () {
		streamTexture = new Texture2D(640, 280);

		for (int i = 0; i < colors.Length; ++i) {
			colors[i] = new Color32(0, 0, 0, 0);
		}

		streamTexture.SetPixels32(colors);
		streamTexture.Apply();

		plane.material.SetTexture (0, streamTexture);
	}
	
	// Update is called once per frame
	void Update () {
		KinectPowerInterop.GetPianoImage (buffer, (uint)buffer.Length);
		BytesToColors (buffer, colors);

		streamTexture.SetPixels32(colors);
		streamTexture.Apply();
	}

	private int ReverseIndex(int src_index) {
		int row = src_index / 640;
		int col = src_index % 640;
		
		return (row) * 640 +
			(640 - col - 1);
	}

	void BytesToColors(byte[] bytes, Color32[] colors) {
		for (int i = 0; i < colors.Length; ++i) {
			int redIndex = i*4 + 2;
			int greenIndex = i*4 + 1;
			int blueIndex = i*4 + 0;
			int alphaIndex = i*4 + 3;
			
			int dst_index = ReverseIndex(i);
			colors[dst_index] = new Color32(bytes[redIndex],
			                                bytes[greenIndex],
			                                bytes[blueIndex],
			                                bytes[alphaIndex]);
		}
	}

	private byte[] buffer = new byte[640 * 280 * 4];
	private Color32[] colors = new Color32[640 * 280];
	private Texture2D streamTexture;

}
