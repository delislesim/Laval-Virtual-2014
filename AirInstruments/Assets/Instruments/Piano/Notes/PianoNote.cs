using UnityEngine;
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
		if (!AssistedModeControllerPiano.EstActive()) {
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

		// Si on est en train de faire le fade out du son.
		if (estFadeout) {
			audio.volume -= kVitesseDiminutionVolume * Time.deltaTime;
			if (audio.volume <= 0) {
				audio.volume = 0;
				estFadeout = false;
			}
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
		Vector3 spherePositionWorld = sphere.transform.position + Vector3.down * sphere.transform.localScale.y * 0.5f;
		Vector3 spherePositionLocal = transform.InverseTransformPoint (spherePositionWorld);

		// Accepter la note seulement si le centre de la boule rouge est au-dessus de la note.
		float scaleXCollider = ((BoxCollider)collider).size.x;
		if (spherePositionLocal.x > 0.5f * scaleXCollider || spherePositionLocal.x < -0.5 * scaleXCollider) {
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
		if (direction.y < 0) {
			return 0;
		}
		
		return Vector3.Angle (Vector3.forward, direction);
	}

	// ===========================================
	//          SON
	// ===========================================

	public void JouerSon(float volume) {
		if (!estJouee) {
			audio.volume = volume;
			if (!estFadeout || audio.volume < 0.5f) {
				audio.pitch = Mathf.Pow (2.0f, ecartDemiTon / 12);
				audio.Play ();
			}
			estJouee = true;
			estFadeout = false;
		}
	}

	public void ArreterSon() {
		if (estJouee) {
			estJouee = false;
			estFadeout = true;
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

	public PartitionPiano.StatutNote ObtenirStatut() {
		return statut;
	}

	// Joue la note de force (accompagnement).
	public void DefinirStatut(PartitionPiano.StatutNote statut) {
		if (statut == this.statut) {
			// Le statut n'a pas change.
			return;
		}
		PartitionPiano.StatutNote dernierStatut = this.statut;
		this.statut = statut;

		// Arreter le son quand la note d'accompagnement cesse.
		if (dernierStatut == PartitionPiano.StatutNote.Accompagnement) {
			ArreterSon();
		}

		if (dernierStatut == PartitionPiano.StatutNote.Joueur) {
			// Mettre le timer au maximum pour permettre a la note de continuer a jouer.
			tempsEnfonceeParErreur = kTempsEnfonceeParErreurMax;
		}

		// Preparer le nouvel etat!
		if (statut == PartitionPiano.StatutNote.Accompagnement) {

			// La note est jouee automatiquement par le mode assiste.
			noteObject.renderer.material = materialNormal;
			tempsEnfonceeParErreur = 0;

			JouerSon(1.0f);

		} else if (statut == PartitionPiano.StatutNote.Joueur) {

			// La note doit etre jouee par le joueur.
			noteObject.renderer.material = doitJouerMaterial;

			aEteJouee = false;

		} else if (statut == PartitionPiano.StatutNote.Muette) {

			// La note ne doit pas etre jouee.
			noteObject.renderer.material = materialNormal;
			tempsEnfonceeParErreur = 0;

		}
	}

	public bool PeutContinuer() {
		if (statut != PartitionPiano.StatutNote.Joueur)
			return true;

		if (aEteJouee || angleCourant >= kAngleCommencerSon)
			return true;

		return false;
	}

	// Retourne vrai si la note est dans un etat qui permet a la partie de continuer.
	public bool GererNoteQuiDoitEtreJouee() {
		if (statut != PartitionPiano.StatutNote.Joueur)
			return true;

		AppliquerRotation(angleCourant);

		if (aEteJouee) {
			// Une fois que la note a ete jouee, elle joue automatiquement pour le reste de la duree requise.
			return true;
		}

		if (angleCourant >= kAngleCommencerSon) {
			JouerSon(1.0f);
			aEteJouee = true;
			return true;
		}

		return false;
	}

	// Met a jour le timer qui avance quand la note est enfoncee alors qu'elle ne
	// devrait pas l'etre.
	public void MettreAJourTimerEnfonceeParErreur() {
		if (statut != PartitionPiano.StatutNote.Muette)
			return;

		if (angleCourant >= kAngleCommencerSon) {
			tempsEnfonceeParErreur += Time.deltaTime;
			if (tempsEnfonceeParErreur >= kTempsEnfonceeParErreurMax) {
				// Appliquer l'angle et jouer la note.
				AppliquerRotation(angleCourant);
				JouerSon(0.8f);  // on ne joue pas le son trop fort.

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
	private const float kProportionNoteBlancheNonJouable = 0.8f;

	// Temps que la note doit etre enfoncee par erreur avant qu'on entende un son.
	private const float kTempsEnfonceeParErreurMax = 0.3f;

	// Vitesse de diminution du volume.
	private const float kVitesseDiminutionVolume = 4.0f;

	// --- Parametres qui sont definis lors de la creation de la note ---

	// Coordonnées du point autour duquel la note tourne, en coordonnées locales.
	Vector3 pointRotationLocal;

	// Coordonnées du point autour duquel la note tourne, en coordonnées du monde.
	Vector3 pointRotationWorld;
	
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

	// Indique si on est en train de faire le fade out du son.
	bool estFadeout = false;

	// --- Parametres pour le mode assiste ---

	// Statut de la note.
	PartitionPiano.StatutNote statut = PartitionPiano.StatutNote.Muette;

	// Temps pendant lequel la note a ete enfoncee alors qu'elle ne devrait pas l'etre.
	float tempsEnfonceeParErreur = 0.0f;

	// Indique si la note a ete jouee par le joueur depuis l'instant ou il doit la jouer.
	bool aEteJouee = false;
}
