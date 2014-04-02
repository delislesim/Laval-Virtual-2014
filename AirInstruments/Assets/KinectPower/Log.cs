using UnityEngine;
using System.Collections;

public class Log : MonoBehaviour {

	public static void Debug(string message) {
		if (file != null) {
			file.WriteLine(message);
			messageStatic = message;
		}
	}

	public static void Open() {
		file = new System.IO.StreamWriter("c:\\temp\\test.txt");
	}

	public static void Close() {
		if (file != null) {
			file.Close ();
			file = null;
		}
		return;
	}

	public void OnGUI() {
		GUI.skin = skin;
		GUI.Label (rect, messageStatic);
	}

	public GUISkin skin;

	private static string messageStatic;

	private Rect rect = new Rect(0, 0, 600, 200);

	private static System.IO.StreamWriter file;
}
