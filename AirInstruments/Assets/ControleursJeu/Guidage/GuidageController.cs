using UnityEngine;
using System.Collections;

public enum typeGuidage{
	AUCUN,
	GUITARE_DRUM,
	PIANO,
	MENU_PRINCIPAL,
	TUTORIEL,
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
	private Rect rectangleGuitareDrum;
	private Rect rectanglePiano;
	private float proportionPicto = 0.75f;
	private float tailleEcranSimon = 1061f;
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
		rectWidth = rectWidth * Screen.width / tailleEcranSimon;
		rectHeight = rectHeight * Screen.width / tailleEcranSimon;
		rectangleMenu = new Rect (0, Screen.height - rectHeight, Screen.width, rectHeight);
		rectangleGuitareDrum = new Rect (0, Screen.height - rectHeight, rectWidth, rectHeight);
		rectanglePiano = new Rect (0, 0, rectWidth, rectHeight);
	}

	// Update is called once per frame
	void OnGUI () {
		GUI.skin = skinGuidage;

		switch (typeGuidage) {
			case typeGuidage.GUITARE_DRUM:
					GUI.BeginGroup (rectangleGuitareDrum);
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Menu");
					GUI.Label (new Rect (0, rectHeight * proportionPicto, rectWidth, rectHeight * (1 - proportionPicto)), "Menu");
					GUI.EndGroup ();
					break;
			case typeGuidage.PIANO:
					GUI.BeginGroup (rectanglePiano);
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Menu");
					GUI.Label (new Rect (0, rectHeight * proportionPicto, rectWidth, rectHeight * (1 - proportionPicto)), "Menu");
					GUI.EndGroup ();
					break;
			case typeGuidage.MENU_PRINCIPAL:
					GUI.BeginGroup (rectangleMenu, skinGuidage.customStyles[0]);

					GUI.BeginGroup (new Rect (0, 0, rectWidth, rectHeight));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Piano");
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight), menu, ScaleMode.ScaleToFit);	
					GUI.EndGroup ();
			
					GUI.BeginGroup (new Rect ((rectangleMenu.width - rectWidth)/2, 0, rectWidth, rectHeight));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Drum");
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight), menu, ScaleMode.ScaleToFit);
					GUI.EndGroup ();
			
					GUI.BeginGroup (new Rect (rectangleMenu.width - rectWidth, 0, rectWidth, rectHeight));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Guitare");
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight), menu, ScaleMode.ScaleToFit);
					GUI.EndGroup ();

					GUI.EndGroup ();
					break;
			}
	}

	public void changerGuidage(typeGuidage type) {
		typeGuidage = type;
	}
}
