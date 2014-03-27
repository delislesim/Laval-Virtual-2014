using UnityEngine;
using System.Collections;

public class Log {

	public static void Debug(string message) {
		if (file != null) {
			file.WriteLine(message);
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

	private static System.IO.StreamWriter file;
}
