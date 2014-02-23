using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class Partition {

	public Partition() {
		// Remplir la table faisant le lien entre les notes et leur index.
		noteToIndex.Add ("Do1", 0);
		noteToIndex.Add ("Do#1", 1);
		noteToIndex.Add ("Re1", 2);
		noteToIndex.Add ("Re#1", 3);
		noteToIndex.Add ("Mi1", 4);
		noteToIndex.Add ("Fa1", 5);
		noteToIndex.Add ("Fa#1", 6);
		noteToIndex.Add ("Sol1", 7);
		noteToIndex.Add ("Sol#1", 8);
		noteToIndex.Add ("La1", 9);
		noteToIndex.Add ("La#1", 10);
		noteToIndex.Add ("Si1", 11);

		noteToIndex.Add ("Do2", 12);
		noteToIndex.Add ("Do#2", 13);
		noteToIndex.Add ("Re2", 14);
		noteToIndex.Add ("Re#2", 15);
		noteToIndex.Add ("Mi2", 16);
		noteToIndex.Add ("Fa2", 17);
		noteToIndex.Add ("Fa#2", 18);
		noteToIndex.Add ("Sol2", 19);
		noteToIndex.Add ("Sol#2", 20);
		noteToIndex.Add ("La2", 21);
		noteToIndex.Add ("La#2", 22);
		noteToIndex.Add ("Si2", 23);

		noteToIndex.Add ("Do3", 24);
		noteToIndex.Add ("Do#3", 25);
		noteToIndex.Add ("Re3", 26);
		noteToIndex.Add ("Re#3", 27);
		noteToIndex.Add ("Mi3", 28);
		noteToIndex.Add ("Fa3", 29);
		noteToIndex.Add ("Fa#3", 30);
		noteToIndex.Add ("Sol3", 31);
		noteToIndex.Add ("Sol#3", 32);
		noteToIndex.Add ("La3", 33);
		noteToIndex.Add ("La#3", 34);
		noteToIndex.Add ("Si3", 35);

		noteToIndex.Add ("Do4", 36);
		noteToIndex.Add ("Do#4", 37);
		noteToIndex.Add ("Re4", 38);
		noteToIndex.Add ("Re#4", 39);
		noteToIndex.Add ("Mi4", 40);
		noteToIndex.Add ("Fa4", 41);
		noteToIndex.Add ("Fa#4", 42);
		noteToIndex.Add ("Sol4", 43);
		noteToIndex.Add ("Sol#4", 44);
		noteToIndex.Add ("La4", 45);
		noteToIndex.Add ("La#4", 46);
		noteToIndex.Add ("Si4", 47);

		// Initialiser la mémoire pour la partition.
		for (int i = 0; i < dureePartition; ++i) {
			List<StatutNote> echantillon = new List<StatutNote>();
			for (int j = 0; j < noteToIndex.Count; ++j) {
				echantillon.Add(StatutNote.Muette);
			}
			partition.Add(echantillon);
		}
	}

	public void JouerTemps(float temps, Instrument instrument) {
		int indexTemps = (int)(temps * 1.75f / resolution);
		if (indexTemps > dureePartition)
			return;

		for (int i = 0; i < noteToIndex.Count; ++i) {
			if (partition[indexTemps][i] == StatutNote.Muette) {
				instrument.StopNote(i);
			} else {
				instrument.PlayNote(i);
			}
		}
	}

	public void ChargerFichier(string nomFichier) {
		StreamReader streamReader = new StreamReader (nomFichier);
		string ligne;

		while ((ligne = streamReader.ReadLine()) != null) {
			int pos = 0;

			// Lire le temps.
			float tempsDebut = ReadNumber(ligne, ref pos);

			// Lire les notes.
			while (ReadNoteDePartition(tempsDebut, ligne, ref pos)) {
			}
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

	private bool ReadNoteDePartition(float tempsDebut, string ligne, ref int pos) {
		// Lire la note.
		string val = "";
		bool hasSeenNoteBeginning = false;
		StatutNote statut = StatutNote.Muette;

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
				statut = StatutNote.Joueur;
				pos = i + 1;
			} else if (caractere == '[') {
				hasSeenNoteBeginning = true;
				statut = StatutNote.Accompagnement;
				pos = i + 1;
			} else {
				break;
			}
		}

		if (!hasSeenNoteBeginning)
			return false;

		int noteIndex = 0;
		try {
			noteIndex = noteToIndex [val];
		} catch (Exception e) {
			Debug.Log("Une note invalide a ete trouvee: " + val + "  " + e.Message);
			return false;
		}

		// Lire la duree.
		float duree = ReadNumber (ligne, ref pos);
		if (duree < 0)
			duree = dureeParDefaut;

		// Lire le caractere de fin.
		for (int i = pos; i < ligne.Length; ++i) {
			Char caractere = ligne[i];

			if (caractere == ' ' || caractere == '}' || caractere == ']') {
				pos = i + 1;
			} else {
				break;
			}
		}

		// Ajouter la note a la partition.
		int debut = (int)(tempsDebut / resolution);
		int fin = (int)((tempsDebut + duree) / resolution);

		for (int i = debut; i < fin; ++i) {
			partition[i][noteIndex] = statut;
		}

		return true;
	}

	// Represente une note de la paritition, dans un echantillon de temps.
	enum StatutNote {
		Accompagnement,
		Joueur,
		Muette
	}

	// Tableau qui contient toute la musique.
	private List<List<StatutNote> > partition = new List<List<StatutNote> > ();

	// Resolution de la parititon.
	private const float resolution = 0.1f;

	// Duree de la partition (en nombre d'elements de resolution |resolution|).
	private const int dureePartition = 50 * 10;

	// Duree des notes pour lesquelles aucune duree n'est specifiee.
	private const float dureeParDefaut = 0.9f;

	// Table qui fait le lien entre les notes et leur index.
	private Dictionary<String, int> noteToIndex = new Dictionary<String, int>();

}
