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

	void OnCollisionEnter(Collision collide)
	{
		if(collide.collider.gameObject.tag == "tip")
			isCollided = true;
	}
	
	void OnCollisionExit(Collision collide)
	{
		if(collide.collider.gameObject.tag == "tip")
			isCollided = false;
	}
}
