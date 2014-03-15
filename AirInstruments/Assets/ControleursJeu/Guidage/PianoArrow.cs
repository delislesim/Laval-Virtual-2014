using UnityEngine;
using System.Collections;

public class PianoArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		// Activer / desactiver tous les enfants.
		foreach (Transform child in transform) {
			child.gameObject.SetActive(afficherGuidage);
		}

		// Pas de guidage: oublier l'axe du dernier guidage.
		if (!afficherGuidage) {
			axeGuidageCourant = -1;
			axeDernierGuidage = -1;
		} else {
			axeDernierGuidage = axeGuidageCourant;
			axeGuidageCourant = -1;
		}

		// Desactiver le guidage et laisser le controleur du piano le
		// reactiver pour la prochaine frame si necessaire.
		afficherGuidage = false;
	}

	// Activer le guidage. Le parametre axe est 0 pour x, 1 pour y, 2 pour z.
	// Le parametre direction indique la direction de la fleche par rapport a l'axe.
	public void AfficherGuidage(int axe, int direction, Vector3 minMain, Vector3 maxMain) {

		// Prendre en compte seulement si on n'a pas encore de guidage pour
		// cette frame ou que le guidage etait present a la frame precedente.
		if (afficherGuidage && axe != axeDernierGuidage)
			return;

		// Position du centre de la main.
		Vector3 centreMain = (minMain + maxMain) / 2.0f;
		Vector3 targetPosition = centreMain;
		Vector3 targetRotation = Vector3.zero;

		// Guidage en x: se positionner a cote de la main en x.
		if (axe == 0) {
			if (direction < 0) {
				targetPosition.x = maxMain.x;
			} else {
				targetPosition.x = minMain.x;
				targetRotation.z = 180;
			}
		} else if (axe == 1) {
			targetPosition.y = maxMain.y + 2.0f;
			targetPosition.z -= 2.0f;

			if (direction < 0) {
				targetRotation.z = 90;
			} else {
				targetRotation.z = 270;
			}
		} else if (axe == 2) {
			targetPosition.y = maxMain.y + 2.0f;
			targetPosition.z -= 2.0f;

			targetRotation.x = 90;

			if (direction < 0) {
				targetRotation.y = 270;
			} else {
				targetRotation.y = 90;
			}
		} else {
			Debug.Log("Axe invalide dans PianoArrow.");
		}

		// Placer la fleche au bon endroit.
		transform.localPosition = targetPosition;
		transform.localEulerAngles = targetRotation;

		// Le guidage doit etre affiche.
		afficherGuidage = true;
	}

	// Axe du guidage courant.
	private int axeGuidageCourant = -1;

	// Axe du dernier guidage.
	private int axeDernierGuidage = -1;

	// Indique si la fleche doit etre affichee.
	private bool afficherGuidage = false;

}
