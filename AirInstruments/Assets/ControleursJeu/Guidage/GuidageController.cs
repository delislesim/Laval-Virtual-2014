using UnityEngine;
using System.Collections;

public enum typeGuidage{
	INSTRUMENTS = 0,
	MENU_PRINCIPAL = 1,
	TUTORIEL = 2
}

public class GuidageController : MonoBehaviour {
	private typeGuidage typeGuidage;
	public GUISkin skinGuidage;

	public MovieTexture pianoGesture;
	public MovieTexture drumGesture;
	public MovieTexture guitarGesture;
	public Texture menu;

	public float rectWidth;
	public float rectHeight;

	private float textureOffset = 12f;//2.5f;

	private Rect rectangleMenu;
	private Rect rectangleInstruments;
	/*private float groupHeight;
	private float groupWidth;
	private int nbGroup;*/

	private static GuidageController instance;

	public static GuidageController ObtenirInstance() {
		return instance;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		//rectangleMenu = new Rect (Screen.width / 2 - 3*rectWidth / 2, Screen.height - rectHeight, 3*rectWidth, rectHeight);
		rectangleMenu = new Rect (0, Screen.height - rectHeight, Screen.width, rectHeight);
		rectangleInstruments = new Rect (0, Screen.height - rectHeight, rectWidth, rectHeight);
	}
	
	// Update is called once per frame
	void OnGUI () {
		GUI.skin = skinGuidage;

		switch (typeGuidage) {
				case typeGuidage.INSTRUMENTS:
						GUI.BeginGroup (rectangleInstruments, skinGuidage.customStyles[0]);
						GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
						GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Menu");
						GUI.EndGroup ();
						break;
				case typeGuidage.MENU_PRINCIPAL:
						GUI.BeginGroup (rectangleMenu);

						GUI.BeginGroup (new Rect (0, 0, rectWidth, rectHeight), skinGuidage.customStyles[0]);
						GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
						GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Piano");
						GUI.EndGroup ();
			
						GUI.BeginGroup (new Rect ((rectangleMenu.width - rectWidth)/2, 0, rectWidth, rectHeight), skinGuidage.customStyles[0]);
						GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
						GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Drum");
						GUI.EndGroup ();
			
						GUI.BeginGroup (new Rect (rectangleMenu.width - rectWidth, 0, rectWidth, rectHeight), skinGuidage.customStyles[0]);
						GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
						GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Guitare");
						GUI.EndGroup ();

						GUI.EndGroup ();
						break;
				}
	}

	public void changerGuidage(typeGuidage type) {
		typeGuidage = type;
	}

	/*private void afficherGuidageGlobal(){
		int groupHeight = 100;
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

		GUI.EndGroup ();
	}*/

	/*public void initialiserGuidage(float width, float height, int nbGroup, typeGuidage instrument) {
		rectWidth = width;
		rectHeight = height;
		groupWidth = width/nbGroup;
		groupHeight = height;
		nbGroup = nbGroup;
		type = instrument;

		rectangleMenu = new Rect (Screen.width / 2 - rectWidth / 2, Screen.height - rectHeight, rectWidth, rectHeight);
	}*/
}
