using UnityEngine;
using System.Collections;

public class FingerSphere : MonoBehaviour, HandJointSphereI {

	// Projecteur générant l'ombre du doigt.
	public GameObject projector;

	private IntelHandController handController;

	// Use this for initialization
	void Start () {
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;

		kalman.SetForce (kForcesKalmanDefaut);
	}

	public void FournirHandController(IntelHandController handController) {
		this.handController = handController;
	}

	// Update is called once per frame
	void Update () {
		if (!IsValid ()) {
			return;
		}

		// Verifier si on est encore sur une note.
		bool estSurNoires = EstSurNoires ();
		bool estSurBlanches = EstSurBlanches ();
		if (!estSurNoires && !estSurBlanches) {
			noteJouee = null;
		} else {
			// Trouver des objets en collision avec cette boule.
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, rayon);
			for (int i = 0; i < hitColliders.Length; ++i) {
				Collider collider = hitColliders[i];
				PianoNote note = collider.GetComponent<PianoNote>();
				if (note != null) {
					bool peutJouerCetteNote = true;

					// Empecher de jouer les mauvaises notes.
					if (AssistedModeControllerPiano.EstActive() &&
					    note.ObtenirStatut() != PartitionPiano.StatutNote.Joueur &&
					    note.AppuieSurNote(this, estDescenduSousBlanches)) {

						if (note.noire) {
							Vector3 targetPosition = transform.position;

							// Pour les noires, on y va tranquillement pour éviter les
							// mouvements brusques.
							float targetY = kHauteurNoires + ObtenirRayon();
							targetPosition.y = Lerp.LerpFloat(targetPosition.y, targetY, 0.6f * Time.deltaTime);

							SetTargetPosition(transform.parent.InverseTransformPoint(targetPosition), valid);
						} else {
							Vector3 targetPosition = transform.position;
							targetPosition.y = kHauteurBlanches + ObtenirRayon();
							transform.position = targetPosition;
							SetTargetPosition(transform.parent.InverseTransformPoint(targetPosition), valid);
						}

						peutJouerCetteNote = false;
					} 

					if (note.ToucherAvecSphere(this, estDescenduSousBlanches)) {
						if (!AssistedModeControllerPiano.EstActive() ||
						    note.ObtenirStatut() == PartitionPiano.StatutNote.Joueur) {
							noteJouee = note;
						}
					}

					// Si on a un allongement et que la note n'est pas a joue, desactiver l'allongement.
					if (!peutJouerCetteNote || !AssistedModeControllerPiano.EstActive()) {
						estAssisteAllongement = false;
					}

					// Animer l'augmentation de l'allongement.
					if (estAssisteAllongement && peutJouerCetteNote && !note.noire) {
						allongement = Lerp.LerpFloat(allongement, kAllongementMax, kAllongementParSeconde * Time.deltaTime); 
					}
				}
			}
		}

		// Animer la diminution de l'allongement.
		if (!estAssisteAllongement) {
			allongement = Lerp.LerpFloat(allongement, 0, kRaccourcissementParSeconde * Time.deltaTime);
		}
	}

	void OnEnable() {
		noteJouee = null;
		estAssisteAllongement = false;
		allongement = 0;
	}

	public void SetAllongementAssiste(bool estAssisteAllongement) {
		this.estAssisteAllongement = estAssisteAllongement;
	}

	public void SetTargetPosition(Vector3 targetPosition, bool valid) {

		// Gerer les etats invalides.
		this.valid = valid;
		if (!valid) {
			++compteurInvalide;
			if (compteurInvalide > kCompteurInvalideMax) {
				renderer.enabled = false;
				projector.SetActive (false);
			}
			return;
		}
		compteurInvalide = 0;
		renderer.enabled = true;
		projector.SetActive (true);

		// Empecher de glisser en x si on est en train de jouer une note.
		/*
		if (noteJouee != null) {
			Vector3 positionNote = transform.parent.InverseTransformPoint(noteJouee.transform.position);
			Vector3 positionDoigt = transform.localPosition;

			// Centrer le doigt sur le centre de la note.
			if (Mathf.Abs(positionNote.x - targetPosition.x) < 0.95f) {
				targetPosition.x = positionNote.x;
			}
		}
		*/

		// Mettre a jour la position.
		if (!initialized) {
			kalman.SetInitialObservation(new Vector4(targetPosition.x,
			                                         targetPosition.y,
			                                         targetPosition.z));
			initialized = true;
		} else {
			kalman.Update(new Vector4(targetPosition.x,
			                          targetPosition.y,
			                          targetPosition.z));
		}

		// Bouger selon le filtre de Kalman.
		Vector4 kalmanPosition = kalman.GetFilteredVector ();
		transform.localPosition = new Vector3 (kalmanPosition.x,
		                       		           kalmanPosition.y,
		                            	       kalmanPosition.z);

		// Noter si on est sous les blanches / au-dessus des noires.
		float basSphere = transform.position.y - ObtenirRayon ();
		if (basSphere < kHauteurBlanches) {
			estDescenduSousBlanches = true;
		} else if (basSphere > kHauteurNoires) {
			estDescenduSousBlanches = false;
		}

		// Ajuster filtre de Kalman selon notre position.
		Vector3 position = transform.position;
		float yBas = position.y - ObtenirRayon ();
		if (EstSurNoires() || EstSurBlanches()) {
			kalman.SetForce(kForcesKalmanTouche);
		} else {
			kalman.SetForce(kForcesKalmanDefaut);
		}

		// Appliquer l'allongement assiste.
		if (allongement != 0) {
			// Ne pas permettre a l'allongement de faire trop descendre le bras.
			float yBasAllonge = yBas - allongement;
			if (yBasAllonge < kHauteurJouerBlanches) {
				allongement += (yBasAllonge - kHauteurJouerBlanches);
				if (allongement < 0) {
					allongement = 0;
				}
			}

			// Appliquer l'allongement.
			Vector3 positionWorld = transform.position;
			positionWorld.y -= allongement;
			transform.position = positionWorld;
		}

		// Replacer le cylindre.
		handController.PlacerCylindres ();
	}

	private bool EstSurNoires() {
		Vector3 position = transform.position;
		float yBas = position.y - ObtenirRayon ();
		return (position.z > kZNoires && yBas < kHauteurNoires);
	}

	private bool EstSurBlanches() {
		Vector3 position = transform.position;
		float yBas = position.y - ObtenirRayon ();
		return (position.z > kZBlanches && yBas < kHauteurBlanches);
	}

	// Retourne le rayon de la sphere representant un doigt.
	public float ObtenirRayon() {
		return rayon;
	}

	public bool IsValid() {
		return valid && compteurInvalide <= kCompteurInvalideMax;
	}


	// Indique si la premiere position a ete definie.
	private bool initialized = false;

	// Filtre de Kalman pour eviter les mouvements brusques du doigt.
	private Kalman kalman = new Kalman(0.0f);

	// Indique la note jouee par ce doigt.
	private PianoNote noteJouee;

	// Forces par defaut pour le filtre de Kalman.
	private Vector3 kForcesKalmanDefaut = new Vector3(1.0f, 1.0f, 1.0f);

	// Forces quand on touche une note pour le filtre de Kalman.
	private Vector3 kForcesKalmanTouche = new Vector3 (15.0f, 1.0f, 1.0f);

	// Rayon de la sphere en coordonnes du monde.
	private float rayon;

	// Indique que le doigt est descendu sous le niveau des notes
	// blanches sans aller au-dessus des notes noires depuis.
	private bool estDescenduSousBlanches = false;

	// Allongement assiste des doigts par seconde.
	private const float kAllongementParSeconde = 4.0f;

	// Diminution de l'allongement des doigts par seconde.
	private const float kRaccourcissementParSeconde = 2.0f;

	// Allongement maximal possible pour les doigts.
	private const float kAllongementMax = 0.65f;

	// Hauteur des notes blanches.
	private const float kHauteurBlanches = 2.171061f;

	// Hauteur pour jouer les notes blanches.
	private const float kHauteurJouerBlanches = 2.02f;

	// Hauteur des notes noires.
	private const float kHauteurNoires = 2.33302f;

	// Coordonnee en z du bout des notes noires.
	private const float kZNoires = -18.19202f;

	// Coordonnee en z du bout des notes blanches.
	private const float kZBlanches = -18.84687f;

	// Indique si le doigt est en train de s'allonger pour assister le jouer a toucher la note.
	private bool estAssisteAllongement = false;

	// Allongement courant du doigt
	private float allongement = 0;

	// Gerer les donnees invalides.
	private bool valid = false;
	private int compteurInvalide = 0;
	private const int kCompteurInvalideMax = 10;
}
