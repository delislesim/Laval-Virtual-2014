using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pointeur : MonoBehaviour {

	// Images de main, a plusieurs stades de remplissage.
	public List<Texture> imagesMain;

	public Pointeur() {
		// Singleton.
		instance = this;
	}

	public static Pointeur obtenirInstance() {
		return instance;
	}

	public void AppliquerDecalageVertical(float decalageVertical) {
		this.decalageVertical = decalageVertical;
	}

	public void RemoveAllTargets() {
		targets.Clear ();
		indexCibleActuelle = kIndexCibleInvalide;
		tempsCibleActuelle = 0;
		indexImageMain = 0;
		decalageVertical = 0;
	}

	public void AddTarget(int targetId,
	                      Vector2 targetCenter,
	                      Vector2 targetSize) {
		Target target = new Target ();
		target.id = targetId;
		target.center = targetCenter;
		target.size = targetSize;
		targets.Add (target);
	}

	public int GetCurrentTargetId() {
		if (indexCibleActuelle != kIndexCibleInvalide &&
		    tempsCibleActuelle > kTempsCibleChoisie) {
			return targets[indexCibleActuelle].id;
		}
		return kIndexCibleInvalide;
	}

	void Update () {
		KinectPowerInterop.NuiHandPointerInfo[] hands = new KinectPowerInterop.NuiHandPointerInfo[2];

		float pressExtent = 0;

		// Positionner la main.
		if (KinectPowerInterop.GetHandsInteraction (0, hands)) {
			if ((hands[0].State & KinectPowerInterop.NuiHandPointerStatePrimaryForUser) != 0) {
				handPosition.x = hands[0].X;
				handPosition.y = hands[0].Y;
				pressExtent = hands[0].PressExtent;
				handIsActive = true;
			} else if ((hands[1].State & KinectPowerInterop.NuiHandPointerStatePrimaryForUser) != 0) {
				handPosition.x = hands[1].X;
				handPosition.y = hands[1].Y;
				pressExtent = hands[1].PressExtent;
				handIsActive = true;
			} else {
				handIsActive = false;
			}
		} else {
			handIsActive = false;
		}

		if (!handIsActive)
			return;

		handPosition.y += decalageVertical;

		// Si la main est deja sur une cible, augmenter son compteur.
		if (indexCibleActuelle != kIndexCibleInvalide) {
			// Verifier si on est encore sur la cible.
			Target target = targets[indexCibleActuelle];
			if (!EstPresDeCible(target)) {
				indexCibleActuelle = kIndexCibleInvalide;
				tempsCibleActuelle = 0;
				indexImageMain = 0;
				return;
			}

			// Augmenter le compteur de la cible.
			if (pressExtent > 0) {
				tempsCibleActuelle += Time.deltaTime * 2.5f;
			} else {
				tempsCibleActuelle += Time.deltaTime;
			}

			// Determiner quelle image de main afficher.
			indexImageMain = (int)(tempsCibleActuelle * imagesMain.Count / kTempsCibleChoisie);
			if (indexImageMain >= imagesMain.Count) {
				indexImageMain = imagesMain.Count - 1;
			}
		} else {
			// Trouver une nouvelle cible.
			for (int i = 0; i < targets.Count; ++i) {
				if (EstPresDeCible(targets[i])) {
					indexCibleActuelle = i;
				}
			}
		}
	}

	bool EstPresDeCible(Target target) {
		Vector2 distances = target.center - handPosition;
		if (Mathf.Abs(distances.x) > target.size.x ||
		    Mathf.Abs(distances.y) > target.size.y) {
			return false;
		}
		return true;
	}
	
	void OnGUI () {
		if (handIsActive) {
			// S'assurer qu'on est en avant de tout.
			// (Petite valeur = plus vers l'avant)
			GUI.depth = 0; 

			// Dessiner la main.
			GUI.DrawTexture (new Rect ((handPosition.x * 250 + 300) * Screen.width / 1080,
			                           (handPosition.y * 350 + 300) * Screen.height / 768,
			                           70 * Screen.width / 1080,
			                           70 * Screen.height / 768),
			                 imagesMain[indexImageMain]);
		}
	}

	// Structure décrivant une cible potentiel pour le pointeur.
	private struct Target {
			public int id;
			public Vector2 center;
			public Vector2 size;
	}

	// Unique instance du pointeur dans le jeu.
	private static Pointeur instance;

	// Index de la cible actuelle dans le tableau de cibles.
	private int indexCibleActuelle = kIndexCibleInvalide;

	// Temps passé sur la cible actuelle.
	private float tempsCibleActuelle = 0;

	// Index de l'image de main a afficher.
	private int indexImageMain = 0;

	// Liste de cibles sur lesquelles la main peut "cliquer".
	private List<Target> targets = new List<Target> ();

	// Indique s'il y a une main active pour le squelette principale.
	private bool handIsActive = false;

	// Position de la main.
	private Vector2 handPosition = new Vector2();

	// Index de cible invalide.
	private const int kIndexCibleInvalide = -1;

	// Temps nécessaire pour qu'une cible soit considérée choisie.
	private const float kTempsCibleChoisie = 3.0f;

	// Decalage vertical.
	private float decalageVertical = 0;
}
