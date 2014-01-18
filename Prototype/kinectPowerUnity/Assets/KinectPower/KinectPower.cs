using UnityEngine;
using System.Collections;

public class KinectPower : MonoBehaviour {

  public bool showDepthMap;

	// Use this for initialization
	void Start () {
    KinectPowerInterop.Initialize();

    depthTexture = new Texture2D(kDepthWidth, kDepthHeight);
    depthMapRect = new Rect(Screen.width, Screen.height - depthTexture.height,
                            -depthTexture.width, depthTexture.height);
	}
	
	void Update () {
    KinectPowerInterop.GetNiceDepthMap(depthBuffer, (uint)depthBuffer.Length);
    for (int i = 0; i < depthColors.Length; ++i) {
      int redIndex = i*4 + 2;
      int greenIndex = i*4 + 1;
      int blueIndex = i*4 + 0;
      int alphaIndex = i*4 + 3;

      depthColors[i] = new Color32(depthBuffer[redIndex],
                                   depthBuffer[greenIndex],
                                   depthBuffer[blueIndex],
                                   depthBuffer[alphaIndex]);
    }

    depthTexture.SetPixels32(depthColors);
    depthTexture.Apply();
	}

  void OnGUI()
  {
    if (showDepthMap)
      GUI.DrawTexture(depthMapRect, depthTexture);
  }

  void OnDestroy () {
    KinectPowerInterop.Shutdown();
  }

  // Depth stream.
  private Rect depthMapRect;
  private Texture2D depthTexture;
  private byte[] depthBuffer = new byte[kDepthWidth * kDepthHeight * 4];
  private Color32[] depthColors = new Color32[kDepthWidth * kDepthHeight];

  private const int kDepthWidth = 640;
  private const int kDepthHeight = 480;

}
