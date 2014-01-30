using UnityEngine;
using System.Collections;
using KinectHelpers;

public class KinectPower : MonoBehaviour {

	// Show the depth stream.
	public bool showDepthMap;

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

	// Use this for initialization
	void Start () {
		if (replay != ReplayMode.REPLAY) {
			KinectPowerInterop.Initialize(false /* pas de near mode */);
		}

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

			Skeleton first_skeleton = new Skeleton(0);
			if (first_skeleton.Exists()) {
				DrawSkeleton(depthTexture, first_skeleton);
			}
			Skeleton second_skeleton = new Skeleton(1);
			if (second_skeleton.Exists()) {
				DrawSkeleton(depthTexture, second_skeleton);
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
		int row = src_index / kDepthWidth;
		int col = src_index % kDepthWidth;

		return (kDepthHeight - row - 1) * kDepthWidth +
			   (kDepthWidth - col - 1);
	}

	void OnGUI()
	{
		if (showDepthMap)
			GUI.DrawTexture(depthMapRect, depthTexture);
	}

	void OnDestroy () {
 		KinectPowerInterop.Shutdown();
	}

	void OnApplicationQuit() {
		KinectPowerInterop.Shutdown();
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
				         Color.yellow);
			}
		}
		
		aTexture.Apply();
	}

	// draws a line in a texture
	private void DrawLine(Texture2D a_Texture, int x1, int y1, int x2, int y2, Color a_Color)
	{
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

	// Depth stream.
	private Rect depthMapRect;
	private Texture2D depthTexture;
	private byte[] depthBuffer = new byte[kDepthWidth * kDepthHeight * 4];
	private Color32[] depthColors = new Color32[kDepthWidth * kDepthHeight];
	private const int kDepthWidth = 640;
	private const int kDepthHeight = 480;
}
