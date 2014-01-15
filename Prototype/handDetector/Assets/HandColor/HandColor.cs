using UnityEngine;
using System.Collections;
using Kinect;

public class HandColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Manager manager = Manager.Instance;
        if (manager == null)
            return;

        short[] depthStream = manager.GetUsersDepthMap();
	}
}
