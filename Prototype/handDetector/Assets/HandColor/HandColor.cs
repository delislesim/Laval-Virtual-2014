using UnityEngine;
using System.Collections;
using Kinect;

public class HandColor : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Initialize color map related stuff
        depthImageTex = new Texture2D(Wrapper.GetDepthWidth(), Wrapper.GetDepthHeight());
        //usersClrColors = new Color[usersMapSize];
        depthImageRect = new Rect(Screen.width, Screen.height - depthImageTex.height, -depthImageTex.width, depthImageTex.height);

        depthImageArray = new Color32[Wrapper.GetDepthWidth() * Wrapper.GetDepthHeight()];
	}
	
	// Update is called once per frame
	void Update () {
        Manager manager = Manager.Instance;
        if (manager == null)
            return;
        initialized = true;

        
        DepthImageFrame depthImage = manager.GetDepthImageFrame();
		int totalDepth = 0;
		int totalPixels = 0;

        for (int i = 0; i < depthImage.GetWidth(); ++i) {
            for (int j = 0; j < depthImage.GetHeight(); ++j) { 
                
                int depth = depthImage.GetDepthAtPosition(i, j);
                byte normalizedDepth = (byte)(depth * 255 / 1800);

                int index = (depthImage.GetHeight() - j - 1) * depthImage.GetWidth() + i;

                depthImageArray[index] = new Color32(normalizedDepth, normalizedDepth, normalizedDepth, 255);
            }
        }

        depthImageTex.SetPixels32(depthImageArray);
        depthImageTex.Apply();

	}

    void OnGUI()
    {
        if (initialized)
        {
            GUI.DrawTexture(depthImageRect, depthImageTex);
        }
    }

    private bool initialized = false;

    private Rect depthImageRect;
    private Texture2D depthImageTex;
    private Color32[] depthImageArray;
}
