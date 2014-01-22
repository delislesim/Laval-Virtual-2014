using UnityEngine;
using System.Collections;

public class KinectPower : MonoBehaviour {

	// Show the streams.
	public bool showDepthMap;

	// Replay mode.
	public enum ReplayMode {
		NO_REPLAY,
		RECORD,
		REPLAY
	}
	public ReplayMode replay = ReplayMode.NO_REPLAY;
	public string replayFilename = "replay.boubou";

	// Piano.
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
		if (replay != ReplayMode.REPLAY)
    		KinectPowerInterop.Initialize();

    	depthTexture = new Texture2D(kDepthWidth, kDepthHeight);
    	depthMapRect = new Rect(Screen.width, Screen.height - depthTexture.height,
                                -depthTexture.width, depthTexture.height);

		if (replay == ReplayMode.RECORD) {
			KinectPowerInterop.RecordSensor(0, replayFilename);
		} else if (replay == ReplayMode.REPLAY) {
			KinectPowerInterop.StartPlaySensor(0, replayFilename);
		}
	}
	
	void Update () {

		if (replay == ReplayMode.REPLAY) {
			KinectPowerInterop.PlayNextFrame(0);
		}

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

			a.SetEnfoncee(notes[0]);
			b.SetEnfoncee(notes[1]);
			c.SetEnfoncee(notes[2]);
			d.SetEnfoncee(notes[3]);
			e.SetEnfoncee(notes[4]);
			f.SetEnfoncee(notes[5]);
			g.SetEnfoncee(notes[6]);
			a1.SetEnfoncee(notes[7]);
			b1.SetEnfoncee(notes[8]);
			c1.SetEnfoncee(notes[9]);
			d1.SetEnfoncee(notes[10]);
			e1.SetEnfoncee(notes[11]);
			f1.SetEnfoncee(notes[12]);
			g1.SetEnfoncee(notes[13]);
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
