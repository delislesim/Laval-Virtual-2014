﻿using UnityEngine;
using System.Collections;

public class PianoNote : MonoBehaviour {

	// Objet affichant réellement la note.
	public GameObject noteObject;

	// Ecart demi-ton.
	public float ecartDemiTon;

	// Indique si c'est une note noire.
	public bool noire;

	// Materiel pour indiquer que la note doit etre jouee.
	public Material doitJouerMaterial;

	void Start () {
		// Calculer la position du point de rotation de la note.
		pointRotationLocal = new Vector3 (0, 0.5f, -0.5f);
		pointRotationWorld = transform.TransformPoint (pointRotationLocal);

		// Se souvenir du materiel par defaut.
		materialNormal = noteObject.renderer.material;
	}

	void Update () {
		if (estModeLibre) {
			// --------- Mode libre ---------

			// Appliquer la rotation a la note visible.
			AppliquerRotation (angleCourant);
			
			// Si l'angle n'est pas nul, on est en train de jouer la note.
			if (angleCourant >= kAngleCommencerSon) {
				if (!estJouee) {
					// Calculer le volume selon le déplacement dans les dernieres images.
					float dernierAngle = derniersAngles [indexDerniersAngles];
					float vitesse = angleCourant - dernierAngle / (float)derniersAngles.Length;
			
					vitesse -= kVitesseMinPourSon;
					if (vitesse < 0)
							vitesse = 0;
			
					float volume = vitesse / 2.2f;
					if (volume > 1.0f)
							volume = 1.0f;
			
					JouerSon (volume);
				}
			} else if (angleCourant == 0) {
				ArreterSon ();
			}
			
			// Toujours enregister les angles.
			derniersAngles [indexDerniersAngles] = angleCourant;
			indexDerniersAngles = (indexDerniersAngles + 1) % derniersAngles.Length;
			
			// Réinitialiser l'angle de la note.
			angleCourant = 0;
		}

	}

	public void AppliquerRotation(float angle) {
		noteObject.transform.localRotation = Quaternion.identity;
		noteObject.transform.localPosition = Vector3.zero;
		if (angle != 0) {
			noteObject.transform.RotateAround (pointRotationWorld, Vector3.left, angle);
		}
	}

	public void ToucherAvecSphere(FingerSphere sphere) {
		// Calculer la position du bas de la boule rouge.
		Vector3 spherePositionWorld = sphere.transform.position + Vector3.forward * sphere.transform.localScale.y * 0.5f;
		Vector3 spherePositionLocal = transform.InverseTransformPoint (spherePositionWorld);

		// Accepter la note seulement si le centre de la boule rouge est au-dessus de la note.
		if (spherePositionLocal.x > 0.5 || spherePositionLocal.x < -0.5) {
			return;
		}

		if (!noire && spherePositionLocal.y > 0.5f - kProportionNoteBlancheNonJouable) {
			return;
		}

		// Calculer l'angle que la note doit avoir pour ne pas toucher au doigt.
		float noteAngle = AnglePourSphereA (spherePositionWorld);
		if (noteAngle > kAngleMaxPermis) {
			angleCourant = kAngleMaxPermis;
		} else if (noteAngle > angleCourant) {
			angleCourant = noteAngle;
		}
	}

	// Calcule l'angle que doit avoir la note pour ne pas toucher
	// au doigt dont la position est spécifiée en parametre
	// (en coordonnées du monde).
	private float AnglePourSphereA(Vector3 fingerPositionWorld) {
		Vector3 direction = new Vector3 (0,
		                                 pointRotationWorld.y - fingerPositionWorld.y,
		                                 pointRotationWorld.z - fingerPositionWorld.z);
		if (direction.z > 0)
			return 0;
		
		return Vector3.Angle (Vector3.up, direction);
	}

	// ===========================================
	//          SON
	// ===========================================

	public void JouerSon(float volume) {
		if (!estJouee) {
			audio.pitch = Mathf.Pow (2.0f, ecartDemiTon / 12);
			audio.volume = volume;
			audio.Play ();
			estJouee = true;
		}
	}

	public void ArreterSon() {
		if (estJouee) {
			audio.Stop ();
			estJouee = false;
		}
	}

	// ===========================================
	//          MODE ASSISTE
	// ===========================================
	
	// Obtient l'enfoncement de la note en degres.
	public float ObtenirAngle() {
		return angleCourant;
	}

	public void DefinirAngle(float angle) {
		angleCourant = angle;
	}

	public Partition.StatutNote ObtenirStatut() {
		return statut;
	}

	// Joue la note de force (accompagnement).
	public void DefinirStatut(Partition.StatutNote statut) {
		if (statut == this.statut) {
			// Le statut n'a pas change.
			return;
		}
		Partition.StatutNote dernierStatut = this.statut;
		this.statut = statut;

		if (statut == Partition.StatutNote.Accompagnement) {

			// La note est jouee automatiquement par le mode assiste.
			noteObject.renderer.material = materialNormal;
			tempsEnfonceeParErreur = 0;

			JouerSon(1.0f);

		} else if (statut == Partition.StatutNote.Joueur) {

			// La note doit etre jouee par le joueur.
			noteObject.renderer.material = doitJouerMaterial;

		} else if (statut == Partition.StatutNote.Muette) {

			// La note ne doit pas etre jouee.
			noteObject.renderer.material = materialNormal;
			tempsEnfonceeParErreur = 0;

		}

		// Arreter le son quand la note d'accompagnement cesse.
		if (dernierStatut == Partition.StatutNote.Accompagnement) {
			ArreterSon();
		}
	}

	// Met a jour le timer qui avance quand la note est enfoncee alors qu'elle ne
	// devrait pas l'etre.
	public void MettreAJourTimerEnfonceeParErreur() {
		if (statut != Partition.StatutNote.Muette)
			return;

		if (angleCourant >= kAngleCommencerSon) {
			tempsEnfonceeParErreur += Time.deltaTime;
			if (tempsEnfonceeParErreur > kTempsEnfonceeParErreurMax) {
				// Appliquer l'angle et jouer la note.
				AppliquerRotation(angleCourant);
				JouerSon(0.6f);  // on ne joue pas le son trop fort.

			} else {
				angleCourant = 0.0f;
			}
		} else {
			ArreterSon();
			AppliquerRotation(angleCourant);
			tempsEnfonceeParErreur = 0.0f;
		}
	}

	public void ObtenirInfoPourCubesTombants(out float positionHorizontale, out float largeur) {
		positionHorizontale = transform.localPosition.x;
		largeur = transform.localScale.x;
	}

	// --- Constantes ---
	
	// Angle pour commencer a jouer la note.
	private const float kAngleCommencerSon = 4.0f;
	
	// Angle maximal permis.
	private const float kAngleMaxPermis = 6.0f;
	
	// Vitesse a laquelle il faut appuyer la note pour faire un son (degres par seconde).
	private const float kVitesseMinPourSon = 2.0f;
	
	// Proportion des notes blanches qui ne peuvent pas etre jourées (réservées aux notes noire)
	private const float kProportionNoteBlancheNonJouable = 0.45f;

	// Temps que la note doit etre enfoncee par erreur avant qu'on entende un son.
	private const float kTempsEnfonceeParErreurMax = 0.3f;

	// --- Parametres qui sont definis lors de la creation de la note ---

	// Coordonnées du point autour duquel la note tourne, en coordonnées locales.
	Vector3 pointRotationLocal;

	// Coordonnées du point autour duquel la note tourne, en coordonnées du monde.
	Vector3 pointRotationWorld;

	// Indique si on est dans le mode libre <------------------------
	bool estModeLibre = true;
	
	// Materiel par defaut.
	Material materialNormal;

	// --- Parametres de l'angle de la note ---

	// Angle que la note doit avoir pour ne pas toucher aux doigts a cette frame.
	float angleCourant = 0;
	
	// Index du prochain angle a mettre dans le tableau dernierAngles (tableau circulaire).
	int indexDerniersAngles = 0;

	// Angles aux dernieres images.
	float[] derniersAngles = new float[2];
	
	// --- Parametres du son de la note ---

	// Indique si la note est en train de produire un son.
	bool estJouee = false;

	// --- Parametres pour le mode assiste ---

	// Statut de la note.
	Partition.StatutNote statut = Partition.StatutNote.Muette;

	// Temps pendant lequel la note a ete enfoncee alors qu'elle ne devrait pas l'etre.
	float tempsEnfonceeParErreur = 0.0f;
}
