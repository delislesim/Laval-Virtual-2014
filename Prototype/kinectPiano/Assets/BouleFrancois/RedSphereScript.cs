using UnityEngine;
using System.Collections;

public class RedSphereScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		KinectManager manager = KinectManager.Instance;
		if (manager == null)
		  return;

		Vector3 handPosition = manager.GetJointPosition (manager.GetPlayer1ID (), (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
		if (handPosition == Vector3.zero)
	      return;

		//transform.position = new Vector3 (handPosition.x * 6, transform.position.y, transform.position.z);

		transform.position = new Vector3 (handPosition.x * 6, handPosition.y * 4, transform.position.z);

		//Debug.Log (handPosition);
	}
}
