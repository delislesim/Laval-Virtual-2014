using UnityEngine;
using System.Collections;

public class TipCollider : MonoBehaviour {

	private bool isCollided;
	// Use this for initialization
	void Start () {
		isCollided = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsCollided()
	{
		return isCollided;
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.collider.gameObject.tag == "DrumComponent")
		{
			//Debug.Log ("tip entered collision");
			isCollided = true;
		}
	}
	
	void OnCollisionExit(Collision col)
	{
		if(col.collider.gameObject.tag == "DrumComponent")
		{
			//Debug.Log ("tip EXITED collision");
			isCollided = false;
		}
	}
}
