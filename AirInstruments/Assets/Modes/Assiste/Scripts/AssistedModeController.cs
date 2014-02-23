using UnityEngine;
using System.Collections;

public class AssistedModeController : MonoBehaviour {

	public GameObject instrument;

	private Instrument instrumentScript;

	void Start () {
		partition = new Partition ();
		partition.ChargerFichier ("C:\\piano\\scientist.txt");

		tempsDebut = Time.time;

		instrumentScript = (Instrument)(instrument.GetComponent (typeof(PianoBuilder)));
	}

	void Update () {
		partition.JouerTemps (Time.time - tempsDebut, instrumentScript);
	}

	// Temps de debut de la musique.
	private float tempsDebut;

	// Partition a jouer.
	private Partition partition;
}
