using UnityEngine;
using System.Collections;

public class PianoNote : MonoBehaviour {

	// Objet affichant réellement la note.
	public GameObject noteObject;

	void Start () {
		// Calculer la position du point de rotation de la note.
		pointRotationLocal = new Vector3 (0, 0.5f, -0.5f);
		pointRotationWorld = transform.TransformPoint (pointRotationLocal);
	}

	void Update () {
		// Appliquer la rotation a la note visible.
		noteObject.transform.localRotation = Quaternion.identity;
		noteObject.transform.localPosition = Vector3.zero;
		noteObject.transform.RotateAround (pointRotationWorld, Vector3.left, noteAngleMax);

		// Réinitialiser l'angle de la note.
		noteAngleMax = 0;
	}

	public void TouchWithSphere(FingerSphere sphere) {
		// Calculer la position de la boule rouge selon nos coordonnées.
		Vector3 spherePositionWorld = sphere.transform.position + Vector3.forward * sphere.transform.localScale.y * 0.5f;

		// Calculer l'angle que la note doit avoir pour ne pas toucher au doigt.
		float noteAngle = AngleWithFingerAt (spherePositionWorld);
		if (noteAngle > noteAngleMaxAllowed) {
			noteAngleMax = noteAngleMaxAllowed;
		} else if (noteAngle > noteAngleMax) {
			noteAngleMax = noteAngle;
		}
	}

	// Calcule l'angle que doit avoir la note pour ne pas toucher
	// au doigt dont la position est spécifiée en parametre
	// (en coordonnées du monde).
	private float AngleWithFingerAt(Vector3 fingerPositionWorld) {
		Vector3 direction = new Vector3 (0,
		                                 pointRotationWorld.y - fingerPositionWorld.y,
		                                 pointRotationWorld.z - fingerPositionWorld.z);
		if (direction.z > 0)
			return 0;

		return Vector3.Angle (Vector3.up, direction);
	}

	// Coordonnées du point autour duquel la note tourne, en coordonnées locales.
	Vector3 pointRotationLocal;

	// Coordonnées du point autour duquel la note tourne, en coordonnées du monde.
	Vector3 pointRotationWorld;

	// Angle que la note doit avoir pour ne pas toucher aux doigts.
	float noteAngleMax = 0;

	// Angle maximal permis.
	float noteAngleMaxAllowed = 6.0f;

}
