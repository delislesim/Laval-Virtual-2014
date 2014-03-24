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
	public GUISkin skinGuidage;
	public float rectWidth;
	public float rectHeight;

	private typeGuidage typeGuidage;
	private GestureRecognition gestureRecognition;

	private Texture[] pianoGesture = new Texture[60];
	private Texture[] drumGesture = new Texture[60];
	private Texture[] guitarGesture = new Texture[60];
	private Texture[] chargement = new Texture[12];
	public Texture menu;
	public Texture menuBackground;

	private float rectWidthMenuPrincipal;
	private float rectHeightMenuPrincipal;

	private Rect rectangleMenu;
	private Rect rectangleGuitareDrum;
	private Rect rectanglePiano;
	private float proportionPicto = 0.75f;
	private float tailleEcranSimon = 1061f;
	private const float seuilChargement = 0.2f;

	private float timer = 0;
	private const float kTempsAnimation = 1.0f;

	private static GuidageController instance;

	public static GuidageController ObtenirInstance() {
		return instance;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		rectWidth = rectWidth * Screen.width / tailleEcranSimon;
		rectHeight = rectHeight * Screen.width / tailleEcranSimon;
		rectWidthMenuPrincipal = rectWidth * 1.5f;
		rectHeightMenuPrincipal = rectHeight * 1.5f;
		rectangleMenu = new Rect (0, Screen.height - rectHeightMenuPrincipal, Screen.width, rectHeightMenuPrincipal);
		rectangleGuitareDrum = new Rect (0, Screen.height - rectHeight, rectWidth, rectHeight);
		rectanglePiano = new Rect (0, 0, rectWidth, rectHeight);

		gestureRecognition = GestureRecognition.ObtenirInstance ();

		for (int i = 0; i < chargement.Length; i++) {
			string imageChargement;
			int indeximageChargment = i + 1;
			imageChargement = indeximageChargment + "_12";
			chargement [i] = (Texture)Resources.Load ("ChargementAnimation/" + imageChargement);
		}

		for (int i = 0; i < guitarGesture.Length; i++) {
			string image;
			int imageIndex = i + 1;
			if(imageIndex < 10)
				image = "000" + imageIndex;
			else
				image = "00" + imageIndex;

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

		float completionPiano = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_PIANO);
		float completionDrum = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_DRUM);
		float completionGuitare = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_GUITAR);
		float completionMenu = gestureRecognition.GetGestureCompletion (GestureId.GESTURE_MENU);

		GestureId gestureEnCours;
		if (completionPiano > completionDrum && completionPiano > completionGuitare)
						gestureEnCours = GestureId.GESTURE_PIANO;
				else if (completionDrum > completionPiano && completionDrum > completionGuitare)
						gestureEnCours = GestureId.GESTURE_DRUM;
				else if (completionGuitare > completionPiano && completionGuitare > completionDrum)
						gestureEnCours = GestureId.GESTURE_GUITAR;
				else
						gestureEnCours = GestureId.GESTURE_MENU;

		int indexChargementAnimation = 0;
		bool pasChargement = true;

		switch (typeGuidage) {
			case typeGuidage.GUITARE_DRUM:
			if(gestureEnCours == GestureId.GESTURE_MENU && completionMenu >= seuilChargement){
				pasChargement = false;
				indexChargementAnimation = indexChargement(completionMenu);
			} else {
				pasChargement = true;
			}
					GUI.BeginGroup (rectangleGuitareDrum);
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
					if(!pasChargement)		
						GUI.DrawTexture(new Rect (0, 0, rectWidth, rectHeight * proportionPicto), chargement[indexChargementAnimation], ScaleMode.ScaleToFit);
					GUI.Label (new Rect (0, rectHeight * proportionPicto, rectWidth, rectHeight * (1 - proportionPicto)), "Menu");
					GUI.EndGroup ();
					break;
			case typeGuidage.PIANO:
			if(gestureEnCours == GestureId.GESTURE_MENU && completionMenu >= seuilChargement){
				pasChargement = false;
				indexChargementAnimation = indexChargement(completionMenu);
			} else {
				pasChargement = true;
			}
					GUI.BeginGroup (rectanglePiano);
					GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
					if(!pasChargement)		
						GUI.DrawTexture(new Rect (0, 0, rectWidth, rectHeight * proportionPicto), chargement[indexChargementAnimation], ScaleMode.ScaleToFit);
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
			if(gestureEnCours == GestureId.GESTURE_PIANO && completionPiano >= seuilChargement){
				pasChargement = false;
				indexChargementAnimation = indexChargement(completionPiano);
			} else {
				pasChargement = true;
			}
					GUI.BeginGroup (new Rect (rectWidthMenuPrincipal*0.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), pianoGesture[indexAnimation], ScaleMode.ScaleToFit);
					if(!pasChargement)		
						GUI.DrawTexture(new Rect(0,0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement[indexChargementAnimation], ScaleMode.ScaleToFit);
					GUI.EndGroup ();

			// L'animation du drum
			if(gestureEnCours == GestureId.GESTURE_DRUM && completionDrum >= seuilChargement){
				pasChargement = false;
				indexChargementAnimation = indexChargement(completionDrum);
			} else {
				pasChargement = true;
			}
					GUI.BeginGroup (new Rect ((rectangleMenu.width - rectWidthMenuPrincipal)/2, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), drumGesture[indexAnimation], ScaleMode.ScaleToFit);
					if(!pasChargement)		
						GUI.DrawTexture(new Rect(0,0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement[indexChargementAnimation], ScaleMode.ScaleToFit);		
					GUI.EndGroup ();

			// L'animation de la guitare
			if(gestureEnCours == GestureId.GESTURE_GUITAR && completionGuitare >= seuilChargement){
				pasChargement = false;
				indexChargementAnimation = indexChargement(completionGuitare);
			} else {
				pasChargement = true;
			}
					GUI.BeginGroup (new Rect (rectangleMenu.width - rectWidthMenuPrincipal*1.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
					GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), guitarGesture[indexAnimation], ScaleMode.ScaleToFit);
					if(!pasChargement)
						GUI.DrawTexture(new Rect(0,0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement[indexChargementAnimation], ScaleMode.ScaleToFit);		
					GUI.EndGroup ();

					GUI.EndGroup ();
					break;
				}
			}
	}

	public void changerGuidage(typeGuidage type) {
		typeGuidage = type;
	}

	private int indexChargement(float completion){
				return (int)(((completion - seuilChargement) / (1.0f - seuilChargement)) * (chargement.Length - 1));
		}
}
