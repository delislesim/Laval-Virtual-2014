using UnityEngine;
using System.Collections;
using KinectHelpers;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

public class KinectPower : MonoBehaviour {

	// Show the depth stream.
	public bool showDepthImage;

	// Show the color stream.
	public bool showColorImage;

	// Show the skeleton stream.
	public bool showSkeletons;

	// Replay mode.
	public enum ReplayMode {
		NO_REPLAY,
		RECORD,
		REPLAY
	}
	public ReplayMode replay = ReplayMode.NO_REPLAY;
	public string replayFilename = "replay.boubou";

	// Indique si le squelette principal doit etre affiche.
	public static void SetAffichageSqueletteActive(bool active) {
		affichageSqueletteActive = active;
	}

	// Indique si le squelette principal doit etre affiche.
	private static bool affichageSqueletteActive = false;

	// Use this for initialization
	void Start () {
		KinectPowerInterop.Initialize(false /* pas de near mode */,
		                              replay != ReplayMode.REPLAY);

    	streamTexture = new Texture2D(kStreamWidth, kStreamHeight);

		float largeur = Screen.width / 12.0f;
		float hauteur = largeur * 480.0f / 640.0f;

    	streamRect = new Rect(Screen.width/2 - largeur/2,
		                      hauteur / 4.0f,
                              largeur,
		                      hauteur);

		if (replay == ReplayMode.RECORD) {
			KinectPowerInterop.RecordSensor(0, replayFilename);
		} else if (replay == ReplayMode.REPLAY) {
			KinectPowerInterop.StartPlaySensor(0, replayFilename);
		}
		initialized = true;
	}

	void Update () {
		if (replay == ReplayMode.REPLAY) {
			KinectPowerInterop.PlayNextFrame(0);
		}

		if (Input.GetButtonDown("SwitchSkeleton")) {
			KinectPowerInterop.AvoidCurrentSkeleton();
			UnityEngine.Debug.Log("avoid current skeleton");
		}

		/*
		if (showDepthImage) {
		    KinectPowerInterop.GetDepthImage(streamBuffer, (uint)streamBuffer.Length);
		    
			BytesToColors(streamBuffer, streamColors);
		    streamTexture.SetPixels32(streamColors);
		    streamTexture.Apply();
		} else if (showColorImage) {
			KinectPowerInterop.GetColorImage(streamBuffer, (uint)streamBuffer.Length);
			
			BytesToColors(streamBuffer, streamColors);
			streamTexture.SetPixels32(streamColors);
			streamTexture.Apply();
		}
		*/

		if (showSkeletons || affichageSqueletteActive) {
			timerSquelettes += Time.deltaTime;
			float periodeSquelette = 1.0f / (float)kNumSquelettesParSeconde;
			if (timerSquelettes >= periodeSquelette) {
				timerSquelettes = 0;

				if (!showDepthImage && !showColorImage) {
					streamTexture.SetPixels32(streamColors);
				}

				Skeleton first_skeleton = new Skeleton(0);
				if (first_skeleton.Exists()) {
					DrawSkeleton(streamTexture, first_skeleton);
				}

				// Guidage pour savoir que les gestes sont bloques.
				if (GestureRecognition.EstBloque()) {
					for (int i = 480 - 30; i < 480 - 5; ++i) {
						DrawLine(streamTexture, 320-12, i, 320+12, i, kColorBloque);
					}
				}

				// Appliquer les changements a la texture.
				streamTexture.Apply();
			}
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
		int row = src_index / kStreamWidth;
		int col = src_index % kStreamWidth;

		return (kStreamHeight - row - 1) * kStreamWidth +
			   (kStreamWidth - col - 1);
	}

	void OnGUI()
	{
		if (showDepthImage || showColorImage || showSkeletons || affichageSqueletteActive)
			GUI.DrawTexture(streamRect, streamTexture);
	}

	void OnDestroy () {
		if (initialized) {
			KinectPowerInterop.Shutdown ();
			initialized = false;
		}
	}

	void OnApplicationQuit() {
		if (initialized) {
			KinectPowerInterop.Shutdown ();
			initialized = false;
		}
	}

	// draws the skeleton in the given texture
	private void DrawSkeleton(Texture2D aTexture, Skeleton skeleton)
	{
		int jointsCount = (int)Skeleton.Joint.Count;
		int width = aTexture.width;
		int height = aTexture.height;

		for(int i = 0; i < jointsCount; i++)
		{
			Skeleton.Joint joint = (Skeleton.Joint)i;
			Skeleton.Joint parent_joint = Skeleton.GetSkeletonJointParent(joint);

			Vector3 joint_position;
			Vector3 parent_joint_position;
			
			if(skeleton.GetJointPositionDepth(joint, out joint_position) != Skeleton.JointStatus.NotTracked &&
			   skeleton.GetJointPositionDepth(parent_joint, out parent_joint_position) != Skeleton.JointStatus.NotTracked)
			{
				parent_joint_position.y = height - parent_joint_position.y - 1;
				joint_position.y = height - joint_position.y - 1;
				parent_joint_position.x = width - parent_joint_position.x - 1;
				joint_position.x = width - joint_position.x - 1;

				DrawLine(aTexture,
				         (int)parent_joint_position.x, (int)parent_joint_position.y,
				         (int)joint_position.x, (int)joint_position.y,
				         kColor);
			}
		}
	}

	// draws a line in a texture
	private void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
	{
		x1 = kStreamWidth - 1 - x1;
		x2 = kStreamWidth - 1 - x2;

		int width = a_Texture.width;
		int height = a_Texture.height;
		
		int dy = y2 - y1;
		int dx = x2 - x1;
		
		int stepy = 1;
		if (dy < 0) 
		{
			dy = -dy; 
			stepy = -1;
		}
		
		int stepx = 1;
		if (dx < 0) 
		{
			dx = -dx; 
			stepx = -1;
		}
		
		dy <<= 1;
		dx <<= 1;
		
		if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
			for(int x = -1; x <= 1; x++)
				for(int y = -1; y <= 1; y++)
					a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
		
		if (dx > dy) 
		{
			int fraction = dy - (dx >> 1);
			
			while (x1 != x2) 
			{
				if (fraction >= 0) 
				{
					y1 += stepy;
					fraction -= dx;
				}
				
				x1 += stepx;
				fraction += dy;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		else 
		{
			int fraction = dx - (dy >> 1);
			
			while (y1 != y2) 
			{
				if (fraction >= 0) 
				{
					x1 += stepx;
					fraction -= dy;
				}
				
				y1 += stepy;
				fraction += dx;
				
				if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					for(int x = -1; x <= 1; x++)
						for(int y = -1; y <= 1; y++)
							a_Texture.SetPixel(x1 + x, y1 + y, a_Color);
			}
		}
		
	}

	private bool initialized = false;

	// Nombre de rafraichissements du squelette par seconde.
	private const int kNumSquelettesParSeconde = 15;

	// Timer pour les squelettes par seconde.
	private float timerSquelettes = 0;

	// Depth and color stream.
	private Rect streamRect;
	private Texture2D streamTexture;
	private byte[] streamBuffer = new byte[kStreamWidth * kStreamHeight * 4];
	private Color32[] streamColors = new Color32[kStreamWidth * kStreamHeight];
	private const int kStreamWidth = 640;
	private const int kStreamHeight = 480;

	private Color kColor = new Color32 (153, 215, 254, 255);
	private Color kColorBloque = new Color32 (235, 235, 235, 255);
}
