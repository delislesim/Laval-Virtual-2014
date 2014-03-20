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

	private Texture[] pianoGesture = new Texture[60];
	private Texture[] drumGesture = new Texture[60];
	private Texture[] guitarGesture = new Texture[60];
	public Texture menu;
	public Texture menuBackground;

	public float rectWidth;
	public float rectHeight;

	private float rectWidthMenuPrincipal;
	private float rectHeightMenuPrincipal;

	private Rect rectangleMenu;
	private Rect rectangleGuitareDrum;
	private Rect rectanglePiano;
	private float proportionPicto = 0.75f;
	private float tailleEcranSimon = 1061f;
	/*private float groupHeight;
	private float groupWidth;
	private int nbGroup;*/

	private float timer = 0;
	private const float kTempsAnimation = 1.0f;

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
		rectWidthMenuPrincipal = rectWidth * 1.5f;
		rectHeightMenuPrincipal = rectHeight * 1.5f;
		rectangleMenu = new Rect (0, Screen.height - rectHeightMenuPrincipal, Screen.width, rectHeightMenuPrincipal);
		rectangleGuitareDrum = new Rect (0, Screen.height - rectHeight, rectWidth, rectHeight);
		rectanglePiano = new Rect (0, 0, rectWidth, rectHeight);

		for (int i = 0; i < guitarGesture.Length; i++) {
			string image;
			int imageIndex = i + 1;
			if(imageIndex < 10)
				image = "000" + imageIndex; //+ ".png";
			else
				image = "00" + imageIndex; //+ ".png";

			//string path = ".\\Assets\\ControleursJeu\\Guidage\\Animation\\Guitare\\" + image +".png";
			//string path = "Assets/Resources/image";

			guitarGesture[i] = (Texture)Resources.Load("GuitareAnimation/" +image);
			pianoGesture[i] = (Texture)Resources.Load("PianoAnimation/" +image);
			drumGesture[i] = (Texture)Resources.Load("DrumAnimation/" +image);
		}
	}

	void Update () {
		timer += Time.deltaTime;
		timer = timer % kTempsAnimation;
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
			case typeGuidage.MENU_PRINCIPAL: {
					int indexAnimation = (int)(timer * (guitarGesture.Length / kTempsAnimation));
					if (indexAnimation >= guitarGesture.Length) {
						indexAnimation = guitarGesture.Length - 1;
					}

					GUI.BeginGroup (rectangleMenu, skinGuidage.customStyles[0]);

			// L'animation du piano
					GUI.BeginGroup (new Rect (rectWidthMenuPrincipal*0.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Piano");
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), pianoGesture[indexAnimation], ScaleMode.ScaleToFit);
					//GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight), pianoGesture, ScaleMode.ScaleToFit);
					GUI.EndGroup ();

			// L'animation du drum
					GUI.BeginGroup (new Rect ((rectangleMenu.width - rectWidthMenuPrincipal)/2, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Drum");
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), drumGesture[indexAnimation], ScaleMode.ScaleToFit);
					GUI.EndGroup ();

			// L'animation de la guitare
					GUI.BeginGroup (new Rect (rectangleMenu.width - rectWidthMenuPrincipal*1.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					//GUI.DrawTexture (new Rect (textureOffset, textureOffset, rectWidth / 2 - 2*textureOffset, rectHeight - 2*textureOffset), menu);
					//GUI.Label (new Rect (rectWidth / 2, 0, rectWidth / 2, rectHeight), "Guitare");
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), guitarGesture[indexAnimation], ScaleMode.ScaleToFit);
					GUI.EndGroup ();

					GUI.EndGroup ();
					break;
				}
			}
	}

	public void changerGuidage(typeGuidage type) {
		typeGuidage = type;
	}
}
