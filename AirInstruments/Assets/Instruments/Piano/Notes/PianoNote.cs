using UnityEngine;
using System.Collections;

public class PianoNote : MonoBehaviour {

	// Objet affichant réellement la note.
	public GameObject noteObject;

	// Ecart demi-ton.
	public float ecartDemiTon;

	// Indique si c'est une note noire.
	public bool noire;

	void Start () {
		// Calculer la position du point de rotation de la note.
		pointRotationLocal = new Vector3 (0, 0.5f, -0.5f);
		pointRotationWorld = transform.TransformPoint (pointRotationLocal);
	}

	void Update () {
		// Appliquer la rotation a la note visible.
		noteObject.transform.localRotation = Quaternion.identity;
		noteObject.transform.localPosition = Vector3.zero;
		if (noteAngleMax != 0) {
			noteObject.transform.RotateAround (pointRotationWorld, Vector3.left, noteAngleMax);
		}

		// Si l'angle n'est pas nul, on est en train de jouer la note.
		// TODO(fdoray): Le son de la note pourrait dépendre de la vitesse de changement de l'angle.
		if (noteAngleMax >= noteAngleStart) {
			if (!isPlaying) {
				// Calculer le volume selon le déplacement dans les dernieres images.
				float dernierAngle = derniersAngles[indexDerniersAngles];
				float vitesse = noteAngleMax - dernierAngle / (float)derniersAngles.Length;

				vitesse -= vitesseNoSound;
				if (vitesse < 0)
					vitesse = 0;

				float volume = vitesse / 2.2f;
				if (volume > 1.0f)
					volume = 1.0f;

				PlaySound(volume, false);
			}
		} else if (noteAngleMax == 0) {
			StopSound(false);
		}

		// Toujours enregister les angles.
		derniersAngles [indexDerniersAngles] = noteAngleMax;
		indexDerniersAngles = (indexDerniersAngles + 1) % derniersAngles.Length;
		
		// Réinitialiser l'angle de la note.
		noteAngleMax = 0;
	}

	public void TouchWithSphere(FingerSphere sphere) {
		// Calculer la position du bas de la boule rouge.
		Vector3 spherePositionWorld = sphere.transform.position + Vector3.forward * sphere.transform.localScale.y * 0.5f;
		Vector3 spherePositionLocal = transform.InverseTransformPoint (spherePositionWorld);

		// Accepter la note seulement si le centre de la boule rouge est au-dessus de la note.
		if (spherePositionLocal.x > 0.5 || spherePositionLocal.x < -0.5) {
			return;
		}

		if (!noire && spherePositionLocal.y > 0.5f - whiteNoteNotPlayable) {
			return;
		}

		// Calculer l'angle que la note doit avoir pour ne pas toucher au doigt.
		float noteAngle = AngleWithFingerAt (spherePositionWorld);
		if (noteAngle > noteAngleMaxAllowed) {
			noteAngleMax = noteAngleMaxAllowed;
		} else if (noteAngle > noteAngleMax) {
			noteAngleMax = noteAngle;
		}
	}

	public void PlaySound(float volume, bool with_override) {
		if (!isPlaying) {
			audio.pitch = Mathf.Pow (2.0f, ecartDemiTon / 12);
			audio.volume = volume;
			audio.Play ();
			isPlaying = true;

			if (with_override)
				isOverride = true;
		}
	}

	public void StopSound(bool with_override) {
		if (isPlaying && (with_override || !isOverride)) {
			audio.Stop ();
			isPlaying = false;
			isOverride = false;
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

	// Angle que la note doit avoir pour ne pas toucher aux doigts a cette frame.
	float noteAngleMax = 0;
	
	// Angle pour commencer a jouer la note.
	float noteAngleStart = 4.0f;

	// Angle maximal permis.
	float noteAngleMaxAllowed = 6.0f;

	// Index du prochain angle a mettre dans le tableau dernierAngles (tableau circulaire).
	int indexDerniersAngles = 0;

	// Angles aux dernieres images.
	float[] derniersAngles = new float[2];

	// Vitesse a laquelle il faut appuyer la note pour faire un son (degres par seconde).
	float vitesseNoSound = 2.0f;

	// Proportion des notes blanches qui ne peuvent pas etre jourées (réservées aux notes noire)
	const float whiteNoteNotPlayable = 0.45f;

	// Indique si on est en train de jouer la note.
	bool isPlaying = false;

	// Indique si la note est en train d'etre joue par un systeme automatique.
	bool isOverride = false;

}
