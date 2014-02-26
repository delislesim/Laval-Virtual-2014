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
				pos = i + 1;
			} else {
				break;
			}
		}

		float numeric_val;
		if (float.TryParse (val, out numeric_val)) {
			return numeric_val;
		} else {
			return -1.0f;
		}
	}

	private bool ReadNoteDePartition(string ligne, ref int pos, List<Playable> partition) {
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

			if (caractere == ' ') {
				if (hasSeenNoteBeginning) {
					break;
				} else {
					pos = i + 1;
				}
			} else if (Char.IsLetterOrDigit(caractere) || caractere == '#') {
				val += caractere;
				pos = i + 1;
			} else if (caractere == '{') {
				hasSeenNoteBeginning = true;
				style = GuitarPlayer.Style.NOTE;
				pos = i + 1;
			} else if (caractere == '[') {
				hasSeenNoteBeginning = true;
				style = GuitarPlayer.Style.CHORD;
				pos = i + 1;
			} else {
				break;
			}
		}

		if (!hasSeenNoteBeginning)
			return false;

		// Lire le caractere de fin.
		for (int i = pos; i < ligne.Length; ++i) {
			Char caractere = ligne[i];

			if (caractere == ' ' || caractere == '}' || caractere == ']') {
				pos = i + 1;
			} else {
				break;
			}
		}

		// Ajouter au tableau de prochaines notes.
		partition.Add (new Playable(duree, notToTone[val], style));
		                
		return true;
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
