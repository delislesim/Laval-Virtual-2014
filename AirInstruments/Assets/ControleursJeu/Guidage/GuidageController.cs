using UnityEngine;
using System.Collections;

public enum typeGuidage
{
		AUCUN,
		GUITARE_DRUM,
		PIANO,
		MENU_PRINCIPAL,
		TUTORIEL,
}

public enum LastAssiste {
	ASSISTE,
	LIBRE,
	AUCUN
}

public class GuidageController : MonoBehaviour
{
		public CureMode cureMode;

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

	private bool isGesteOverriden = false;
	private GestureId gestureOverriden;
	private bool isOverridenAnimationFinished = false;
	private const float overridenAnimationTime = 1.0f;
	private float elapsedAnimationTime = 0;

	private LastAssiste lastAssiste = LastAssiste.AUCUN;

		public static GuidageController ObtenirInstance ()
		{
				return instance;
		}

		// Use this for initialization
		void Start ()
		{
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
						if (imageIndex < 10)
								image = "000" + imageIndex;
						else
								image = "00" + imageIndex;

						guitarGesture [i] = (Texture)Resources.Load ("GuitareAnimation/" + image);
						pianoGesture [i] = (Texture)Resources.Load ("PianoAnimation/" + image);
						drumGesture [i] = (Texture)Resources.Load ("DrumAnimation/" + image);
				}
		}

		void Update ()
		{
				timer += Time.deltaTime;
				timer = timer % kTempsAnimation;

				elapsedAnimationTime += Time.deltaTime;

				/*
		sumDeltaTime += 1.0f / Time.deltaTime;
		++compteurDeltaTime;
		if (compteurDeltaTime == 10) {
			fps = sumDeltaTime / 10.0f;
			compteurDeltaTime = 0;
			sumDeltaTime = 0;
		}
		*/
		}

		/*
	private float sumDeltaTime = 0;
	private int compteurDeltaTime = 0;
	private float fps = 0;
	*/

		// Update is called once per frame
		void OnGUI ()
		{
				LastAssiste nouvelAssiste;

				GUI.skin = skinGuidage;

				string mode;
				float rectHeightMode = rectHeight / 1.5f;
				float rectWidthMode = rectWidth / 1.5f;

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
						if (gestureEnCours == GestureId.GESTURE_MENU && completionMenu >= seuilChargement) {
								pasChargement = false;
								indexChargementAnimation = indexChargement (completionMenu);
						} else {
								pasChargement = true;
						}
						GUI.BeginGroup (rectangleGuitareDrum);
						GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
						if (!pasChargement)		
								GUI.DrawTexture (new Rect (0, -6, rectWidth, rectHeight * proportionPicto * 1.2f), chargement [indexChargementAnimation], ScaleMode.ScaleToFit);
						GUI.Label (new Rect (0, rectHeight * proportionPicto, rectWidth, rectHeight * (1 - proportionPicto)), "Menu");
						GUI.EndGroup ();

			//Afficher le mode (assité ou libre)
						GUI.BeginGroup (new Rect (Screen.width - rectWidthMode*1.2f, Screen.height - rectHeightMode, rectWidthMode, rectHeightMode));
						if (AssistedModeControllerGuitar.EstActive () || DrumAssistedController.EstActive ()) {
								nouvelAssiste = LastAssiste.ASSISTE;
								mode = "Mode assisté";
						} else {
								nouvelAssiste = LastAssiste.LIBRE;
								mode = "Mode libre";
						}
						if (nouvelAssiste != lastAssiste) {
							cureMode.Burst(kPositionBas);
							lastAssiste = nouvelAssiste;
						}


						GUI.Label (new Rect (0, 0, rectWidthMode, rectHeightMode), mode);
						GUI.EndGroup ();
						break;
				case typeGuidage.PIANO:
						if (gestureEnCours == GestureId.GESTURE_MENU && completionMenu >= seuilChargement) {
								pasChargement = false;
								indexChargementAnimation = indexChargement (completionMenu);
						} else {
								pasChargement = true;
						}
						GUI.BeginGroup (rectanglePiano);
						GUI.DrawTexture (new Rect (0, 0, rectWidth, rectHeight * proportionPicto), menu, ScaleMode.ScaleToFit);
						if (!pasChargement)		
								GUI.DrawTexture (new Rect (0, -6, rectWidth, rectHeight * proportionPicto), chargement [indexChargementAnimation], ScaleMode.ScaleToFit);
						GUI.Label (new Rect (0, rectHeight * proportionPicto, rectWidth, rectHeight * (1 - proportionPicto)), "Menu");
						GUI.EndGroup ();

			//Afficher le mode (assité ou libre)
						GUI.BeginGroup (new Rect (Screen.width - rectWidthMode*1.2f, 0, rectWidthMode, rectHeightMode));

						if (AssistedModeControllerPiano.EstActive ()) {
								nouvelAssiste = LastAssiste.ASSISTE;
								mode = "Mode assisté";
						} else {
								nouvelAssiste = LastAssiste.LIBRE;
								mode = "Mode libre";
						}

						if (nouvelAssiste != lastAssiste) {
							cureMode.Burst(kPositionHaut);
							lastAssiste = nouvelAssiste;
						}

						GUI.Label (new Rect (0, 0, rectWidthMode, rectHeightMode), mode);
						GUI.EndGroup ();
						break;
				case typeGuidage.MENU_PRINCIPAL:
						{
								int indexAnimation = (int)(timer * (guitarGesture.Length / kTempsAnimation));
								if (indexAnimation >= guitarGesture.Length) {
										indexAnimation = guitarGesture.Length - 1;
								}
			GUI.BeginGroup (rectangleMenu, skinGuidage.customStyles [0]);

			if(gestureOverriden == GestureId.GESTURE_PIANO && isGesteOverriden){
				if((elapsedAnimationTime/overridenAnimationTime+seuilChargement) >= 1.0f) {
					isGesteOverriden = false;
					isOverridenAnimationFinished = true;
					GestureRecognition.Bloquer (false);
					pasChargement = true;
					elapsedAnimationTime=0.0f;
				} else {
					indexChargementAnimation = indexChargement ((elapsedAnimationTime/overridenAnimationTime)+seuilChargement);
					pasChargement = false;
				}
			}
			else{
					// L'animation du piano
					if (gestureEnCours == GestureId.GESTURE_PIANO && completionPiano >= seuilChargement) {
							pasChargement = false;
							indexChargementAnimation = indexChargement (completionPiano);
					} else {
							pasChargement = true;
					}
			}

								GUI.BeginGroup (new Rect (rectWidthMenuPrincipal * 0.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), pianoGesture [indexAnimation], ScaleMode.ScaleToFit);
								if (!pasChargement)		
										GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement [indexChargementAnimation], ScaleMode.ScaleToFit);
								GUI.EndGroup ();
			if(gestureOverriden == GestureId.GESTURE_DRUM && isGesteOverriden){
				if((elapsedAnimationTime/overridenAnimationTime+seuilChargement) >= 1.0f) {
					isGesteOverriden = false;
					isOverridenAnimationFinished = true;
					GestureRecognition.Bloquer (false);
					pasChargement = true;
					elapsedAnimationTime=0.0f;
				} else {
					indexChargementAnimation = indexChargement ((elapsedAnimationTime/overridenAnimationTime)+seuilChargement);
					pasChargement = false;
				}
			}
			else{
								// L'animation du drum
								if (gestureEnCours == GestureId.GESTURE_DRUM && completionDrum >= seuilChargement) {
										pasChargement = false;
										indexChargementAnimation = indexChargement (completionDrum);
								} else {
										pasChargement = true;
								}
			}
								GUI.BeginGroup (new Rect ((rectangleMenu.width - rectWidthMenuPrincipal) / 2, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), drumGesture [indexAnimation], ScaleMode.ScaleToFit);
								if (!pasChargement)		
										GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement [indexChargementAnimation], ScaleMode.ScaleToFit);		
								GUI.EndGroup ();
			if(gestureOverriden == GestureId.GESTURE_GUITAR && isGesteOverriden){
				if((elapsedAnimationTime/overridenAnimationTime+seuilChargement) >= 1.0f) {
					isGesteOverriden = false;
					isOverridenAnimationFinished = true;
					GestureRecognition.Bloquer (false);
					pasChargement = true;
					elapsedAnimationTime=0.0f;
				} else {
					indexChargementAnimation = indexChargement ((elapsedAnimationTime/overridenAnimationTime)+seuilChargement);
					pasChargement = false;
				}
			}
			else{
								// L'animation de la guitare
								if (gestureEnCours == GestureId.GESTURE_GUITAR && completionGuitare >= seuilChargement) {
										pasChargement = false;
										indexChargementAnimation = indexChargement (completionGuitare);
								} else {
										pasChargement = true;
								}
			}
								GUI.BeginGroup (new Rect (rectangleMenu.width - rectWidthMenuPrincipal * 1.75f, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal));
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), menuBackground, ScaleMode.ScaleToFit);
								GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), guitarGesture [indexAnimation], ScaleMode.ScaleToFit);
								if (!pasChargement)
										GUI.DrawTexture (new Rect (0, 0, rectWidthMenuPrincipal, rectHeightMenuPrincipal), chargement [indexChargementAnimation], ScaleMode.ScaleToFit);		
								GUI.EndGroup ();

								GUI.EndGroup ();
								break;
						}
				}
		}

	public void overrideGeste(GestureId gesture){
		isOverridenAnimationFinished = false;
		gestureOverriden = gesture;
		isGesteOverriden = true;

		elapsedAnimationTime = gestureRecognition.GetGestureCompletion(gesture) * overridenAnimationTime;
		GestureRecognition.Bloquer (true);
	}

	public bool getAniamtionStatus(){
		return isOverridenAnimationFinished;
	}

		public void changerGuidage (typeGuidage type)
		{
				if (typeGuidage != type) {
					lastAssiste = LastAssiste.AUCUN;
					typeGuidage = type;
				}
		}

		private int indexChargement (float completion)//[0,1]
		{
				return (int)(((completion - seuilChargement) / (1.0f - seuilChargement)) * (chargement.Length - 1));
		}

	private Vector3 kPositionBas = new Vector3(0.9050451f,  -4.890831f,  3.052823f);
	private Vector3 kPositionHaut = new Vector3(1.884743f,  -0.069542f,  9.466908f);
}
