using UnityEngine;
using System.Collections;

public enum typeGuidage{
	PIANO = 0,
	DRUM = 1,
	GUITARE = 2,
	MENU = 3
}

public class guidageController : MonoBehaviour {

	public MovieTexture pianoGesture;
	public MovieTexture drumGesture;
	public MovieTexture guitarGesture;
	public Texture test;
	public GUISkin skinGuidage;

	private Rect rectangleMenu;
	private float rectWidth;
	private float rectHeight;
	private float groupHeight;
	private float groupWidth;
	private int nbGroup;
	private typeGuidage type; 

	private static guidageController instance;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnGUI () {
		GUI.skin = skinGuidage;
		
		GUI.BeginGroup (rectangleMenu);
		for (int i = 0; i<nbGroup; i++) {
			Rect rectangleGroup = new Rect(i*rectWidth/nbGroup, 0, groupWidth, groupHeight);
			GUI.BeginGroup (rectangleGroup);
			GUI.DrawTexture (new Rect(0,0,groupWidth, groupHeight), test);
			GUI.EndGroup();
		}
		GUI.EndGroup ();
	}

	private void afficherGuidageGlobal(){
		/*int groupHeight = 100;
		int groupWidth = 600;
		GUI.BeginGroup(new Rect (Screen.width/2-groupWidth/2,Screen.height-groupHeight,groupWidth,groupHeight));

		GUI.BeginGroup (new Rect (0, 0, groupWidth/3, groupHeight));
		GUI.DrawTexture (new Rect (0, 0, groupWidth/3, groupHeight*0.75f), menu);
		GUI.Label(new Rect(0,groupHeight-groupHeight*0.25f,groupWidth/3,groupHeight*0.25f), "Piano");
		GUI.EndGroup ();

		GUI.BeginGroup (new Rect (groupWidth/3, 0, groupWidth/3, groupHeight));
		GUI.DrawTexture (new Rect (0, 0, groupWidth/3, groupHeight*0.75f), menu);
		GUI.Label(new Rect(0,groupHeight-groupHeight*0.25f,groupWidth/3,groupHeight*0.25f), "Drum");
		GUI.EndGroup ();

		GUI.BeginGroup (new Rect (2*groupWidth/3, 0, groupWidth/3, groupHeight));
		GUI.DrawTexture (new Rect (0, 0, groupWidth/3, groupHeight*0.75f), menu);
		GUI.Label(new Rect(0,groupHeight-groupHeight*0.25f,groupWidth/3,groupHeight*0.25f), "Guitare");
		GUI.EndGroup ();

		GUI.EndGroup ();*/
	}

	public void initialiserGuidage(float width, float height, int nbGroup, typeGuidage instrument) {
		rectWidth = width;
		rectHeight = height;
		groupWidth = width/nbGroup;
		groupHeight = height;
		nbGroup = nbGroup;
		type = instrument;

		rectangleMenu = new Rect (Screen.width / 2 - rectWidth / 2, Screen.height - rectHeight, rectWidth, rectHeight);
	}
}
