using UnityEngine;
using System.Collections;

public class Langue : MonoBehaviour {

	public static bool isEnglish = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("English"))
			isEnglish = true;
		else
		{
			if(Input.GetButtonDown("Francais"))
			   isEnglish = false;
		}
	}
}
