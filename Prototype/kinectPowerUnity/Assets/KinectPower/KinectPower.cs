﻿using UnityEngine;
using System.Collections;

public class KinectPower : MonoBehaviour {

	public bool showDepthMap;
	public bool showPiano;

	public Note a;
	public Note b;
	public Note c;
	public Note d;
	public Note e;
	public Note f;
	public Note g;
	public Note a1;
	public Note b1;
	public Note c1;
	public Note d1;
	public Note e1;
	public Note f1;
	public Note g1;


	// Use this for initialization
	void Start () {
    KinectPowerInterop.Initialize();

    depthTexture = new Texture2D(kDepthWidth, kDepthHeight);
    depthMapRect = new Rect(Screen.width, Screen.height - depthTexture.height,
                            -depthTexture.width, depthTexture.height);
	}
	
	void Update () {
		if (showDepthMap) {
		    KinectPowerInterop.GetNiceDepthMap(depthBuffer, (uint)depthBuffer.Length);
		    
			BytesToColors(depthBuffer, depthColors);
		    depthTexture.SetPixels32(depthColors);
		    depthTexture.Apply();
		} else if (showPiano) {
			KinectPowerInterop.GetPianoInfo(notes, (uint)notes.Length,
											depthBuffer, (uint)depthBuffer.Length);

			BytesToColors(depthBuffer, depthColors);			
			depthTexture.SetPixels32(depthColors);
			depthTexture.Apply();

			a.SetEnfoncee(notes[0] == 1);
			b.SetEnfoncee(notes[1] == 1);
			c.SetEnfoncee(notes[2] == 1);
			d.SetEnfoncee(notes[3] == 1);
			e.SetEnfoncee(notes[4] == 1);
			f.SetEnfoncee(notes[5] == 1);
			g.SetEnfoncee(notes[6] == 1);
			a1.SetEnfoncee(notes[7] == 1);
			b1.SetEnfoncee(notes[8] == 1);
			c1.SetEnfoncee(notes[9] == 1);
			d1.SetEnfoncee(notes[10] == 1);
			e1.SetEnfoncee(notes[11] == 1);
			f1.SetEnfoncee(notes[12] == 1);
			g1.SetEnfoncee(notes[13] == 1);
		}
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

	private int ReverseIndex(int src_index) {
		int row = src_index / kDepthWidth;
		int col = src_index % kDepthWidth;

		return (kDepthHeight - row - 1) * kDepthWidth +
			   (kDepthWidth - col - 1);
	}

	void OnGUI()
	{
		if (showDepthMap || showPiano)
			GUI.DrawTexture(depthMapRect, depthTexture);
	}

	void OnDestroy () {
 		KinectPowerInterop.Shutdown();
	}

	void OnApplicationQuit() {
		KinectPowerInterop.Shutdown();
	}

	// Depth stream.
	private Rect depthMapRect;
	private Texture2D depthTexture;
	private byte[] depthBuffer = new byte[kDepthWidth * kDepthHeight * 4];
	private Color32[] depthColors = new Color32[kDepthWidth * kDepthHeight];
	private const int kDepthWidth = 640;
	private const int kDepthHeight = 480;

	// Piano stream.
	private const int kPianoNumNotes = 20;
	private byte[] notes = new byte[kPianoNumNotes];
}
