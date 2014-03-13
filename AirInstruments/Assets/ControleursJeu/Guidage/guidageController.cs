using UnityEngine;
using System.Collections;

public class guidageController : MonoBehaviour {

	public Texture menu;
	private Rect retourMenu;
	public GUISkin skin;

	// Use this for initialization
	void Start () {
		retourMenu = new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100);
	}
	
	// Update is called once per frame
	void OnGUI () {
		afficherGuidageGlobal ();
		//GUI.Button (new Rect (0, Screen.height-50, 200, 50), "Put your hand in the air !", "button");
		//GUI.Button (new Rect (0, Screen.height - 50, 200, 50), menu, "button");
		//GUI.TextArea (new Rect (Screen.width/2-100, Screen.height/2-25, 200, 50), "Choisissez votre instrument");
		/*if(Time.time %2 > 1){
			Mathf.Sin(
			retourMenu.center = retourMenu.center
		}*/
		/*float sin = Mathf.Sin (Time.time*10);
		retourMenu.center = new Vector2(retourMenu.center.x, retourMenu.center.y + sin);

		GUI.DrawTexture (retourMenu, menu);*/
	}

	private void afficherGuidageGlobal(){
		GUI.skin = skin;
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

		/*GUILayout.BeginArea (new Rect (Screen.width/2-250,Screen.height-50,500,50));

		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical ();
		//GUILayout.Button ("Test1");
		GUILayout.Label ("Piano", "label");
		GUILayout.EndVertical ();

		GUILayout.BeginVertical ();
		//GUILayout.Button ("Test3");
		GUILayout.Label ("Drum", "label");
		GUILayout.EndVertical ();

		GUILayout.BeginVertical ();
		//GUILayout.Button ("Test3");
		GUILayout.Label ("Guitare", "label");
		GUILayout.EndVertical ();*/
		/*if (GUILayout.RepeatButton ("Increase max\nSlider Value"))
		{
			maxSliderValue += 3.0f * Time.deltaTime;
		}

		GUILayout.BeginVertical();
		GUILayout.Box("Slider Value: " + Mathf.Round(sliderValue));
		sliderValue = GUILayout.HorizontalSlider (sliderValue, 0.0f, maxSliderValue);*/

		//GUILayout.EndVertical();
		//GUILayout.EndHorizontal();
		//GUILayout.EndArea();
	}
}
