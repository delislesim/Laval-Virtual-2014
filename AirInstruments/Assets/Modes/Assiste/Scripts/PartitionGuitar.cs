using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class PartitionGuitar {

	public PartitionGuitar() {
		// Remplir la table faisant le lien entre les notes et le tone.
		notToTone.Add ("E", GuitarPlayer.Tone.E);
		notToTone.Add ("F", GuitarPlayer.Tone.F);
		notToTone.Add ("F#", GuitarPlayer.Tone.Gb);
		notToTone.Add ("G", GuitarPlayer.Tone.G);
		notToTone.Add ("G#", GuitarPlayer.Tone.Ab);
		notToTone.Add ("A", GuitarPlayer.Tone.A);
		notToTone.Add ("A#", GuitarPlayer.Tone.Bb);
		notToTone.Add ("B", GuitarPlayer.Tone.B);
		notToTone.Add ("C", GuitarPlayer.Tone.C);
		notToTone.Add ("C#", GuitarPlayer.Tone.Db);
		notToTone.Add ("D", GuitarPlayer.Tone.D);
		notToTone.Add ("D#", GuitarPlayer.Tone.Eb);
	}

	public void ChargerFichier(string nomFichier) {
		streamReader = new StreamReader (nomFichier);
	}

	// Retourne vrai quand il reste des notes, faux quand la musique est finie.
	public void RemplirPartition(List<Playable> partition) {

		string ligne;
		while ((ligne = streamReader.ReadLine()) != null) {
			int pos = 0;

			// Lire les notes.
			while (ReadNoteDePartition(ligne, ref pos, partition)) {}

		}
	}

	private float ReadNumber(string ligne, ref int pos) {
		string val = "";
		for (int i = pos; i < ligne.Length; ++i) {
			Char caractere = ligne[i];
			if (Char.IsDigit(caractere) || caractere == '.') {
				val += caractere;
				pos = i + 1;
			} else if (caractere == ' ') {
				break;
			} else {
				pos = i + 1;
			}
		}

		float numeric_val;
		if (float.TryParse (val, out numeric_val)) {
			//Debug.Log ("Time : " + numeric_val + " at line : " + ligne);
			return numeric_val;
		} else {
			return -1.0f;
		}
	}

	private bool SkipFirstTime(string ligne, ref int pos) {
		bool tFound = false;
		for (int i = pos; i < ligne.Length; ++i) {
			Char caractere = ligne[i];
			if (caractere == 't') {
				tFound = true;
				pos = i + 1;
			} else if (caractere == '1' && tFound) {
				//Debug.Log ("Found good time");
				pos = i + 1;
				return true;
			} else if (caractere != '1' && tFound){
				tFound = false;
				pos = i + 1;
			}
		}
		return false;
	}

	private bool FindLabel(string ligne, ref int pos)
	{
		string val = "";
		for (int i = pos; i < ligne.Length; ++i) {
			Char caractere = ligne[i];
			if (Char.IsLetter(caractere) || caractere == '<') {
				val += caractere;
				pos = i + 1;
			} else if (caractere == '\t') {
				pos = i + 1;
			} else if (caractere == ' '){
				break;
			}
		}

		if(val == "<label"){
			//Debug.Log ("Label trouvé!!");
			return true;
		}
		return false;
	}

	/// <summary>
	/// Lit une ligne d'un fichier audicity (aup) et trouve un label avec un temps et une note.
	/// </summary>
	/// <returns><c>true</c>, if note de partition was  read, <c>false</c> otherwise.</returns>
	/// <param name="ligne">Ligne.</param>
	/// <param name="pos">Position.</param>
	/// <param name="partition">Partition.</param>
	private bool ReadNoteDePartition(string ligne, ref int pos, List<Playable> partition) {

		if(FindLabel(ligne, ref pos))
		{
			if(SkipFirstTime(ligne, ref pos))
			{
				// Lire la duree.
				float duree = ReadNumber (ligne, ref pos);
				if (duree < 0)
					duree = dureeParDefaut;

				// Lire la note. val = Tone {} ou [] = Style
				string val = "";
				bool hasSeenNoteBeginning = false;
				GuitarPlayer.Style style = GuitarPlayer.Style.NOTE;
				for (int i = pos; i < ligne.Length; ++i) {
					Char caractere = ligne[i];

					if (caractere == '>') {
						break;
					} else if ((caractere=='A' || caractere=='B' ||caractere=='C' ||caractere=='D' ||caractere=='E' ||
					            caractere=='F' ||caractere=='G'  || caractere == '#') && hasSeenNoteBeginning ) {
						val += caractere;
						pos = i + 1;
					} else if (caractere == '-') {
						hasSeenNoteBeginning = true;
						style = GuitarPlayer.Style.NOTE;
						pos = i + 1;
					} else if (caractere == '+') {
						hasSeenNoteBeginning = true;
						style = GuitarPlayer.Style.CHORD;
						pos = i + 1;
					} else {
						pos= i+1;
					}
				}

				if (!hasSeenNoteBeginning)
					return false;

				// Ajouter au tableau de prochaines notes.
				partition.Add (new Playable(duree, notToTone[val], style));
				//Debug.Log ("Found note : " + val);               
				return true;
			}
		}
		return false;
	}

	public struct Playable{
		public float time;
		public GuitarPlayer.Tone note;
		public GuitarPlayer.Style style;

		// Constructor:
		public Playable(float duree, GuitarPlayer.Tone tone, GuitarPlayer.Style style) 
		{
			this.time = duree;
			this.note = tone;
			this.style = style;
		}
	}

	// Stream du fichier de partition.
	private StreamReader streamReader;

	// Duree des notes pour lesquelles aucune duree n'est specifiee.
	private const float dureeParDefaut = 1.0f;

	// Table qui fait le lien entre les notes et leur index.
	private Dictionary<String, GuitarPlayer.Tone> notToTone = new Dictionary<String, GuitarPlayer.Tone>();

}
