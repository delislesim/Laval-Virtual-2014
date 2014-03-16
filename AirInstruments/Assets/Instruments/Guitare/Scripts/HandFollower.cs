using UnityEngine;
using System.Collections;

public class HandFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;
	public ColliderManager guitarCollider;
	public GuitarPlayer guitarPlayer;

	// Lumiere verte indiquant quand les notes sont jouees.
	public GameObject greenLight;

	// Use this for initialization
	void Start () {
		// Calculer le rayon du tip.
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
		collisionReady = true;
		// Layer des colliders de drum components.
		guitarPlayerLayer = 1 << LayerMask.NameToLayer ("GuitarPlayer");
	}
	
	// Update is called once per frame
	void Update () {
		// --- Animer la lumiere verte. ---
		if (greenLight.activeSelf) {
			tempsDepuisDerniereNote += Time.deltaTime;
			if (tempsDepuisDerniereNote > 0.25f) {
				Color32 color = greenLight.renderer.material.color;
				Color32 nextColor = Color.Lerp(color, Color.black, Time.deltaTime * 4.0f);
				greenLight.renderer.material.color = nextColor;

				if (nextColor == Color.black) {
					greenLight.SetActive (false);
				}
			}
		}


		// --- Gerer les sons de guitare. ---
		lastPosition = transform.position;
		
		// Suivre l'objet auquel on a été assigné.
		transform.position = objectToFollow.transform.position;
		
		// Si on a joue une note recemment, s'assurer qu'on s'en eloigne avant de le rejouer.
		float distance = guitarCollider.DistanceToPoint(transform.position);
		if (Mathf.Abs(distance) > kDistancePourRejouer) {
			collisionReady = true;
		}

		// Compter les mouvements amples.
		if (distance > kDistanceAmple && !dernierAuDessus) {
			++numMouvementsAmples;
			dernierAuDessus = true;
		} else if (distance < kDistanceAmple && dernierAuDessus) {
			++numMouvementsAmples;
			dernierAuDessus = false;
		}

		// Inscrire les distance par rapport aux cordes.
		compteurDistance += Time.deltaTime;
		if (compteurDistance >= kTempsPourEcrireDistance) {
			compteurDistance = compteurDistance % kTempsPourEcrireDistance;

			indexDernieresDistance = (indexDernieresDistance + 1) % dernieresDistances.Length;
			dernieresDistances [indexDernieresDistance] = Mathf.Abs(distance);
		}
		tempsDepuisDerniereNoteAutomatique += Time.deltaTime;

		// Reduire le temps pour la prochaine note.
		tempsProchaineNote -= Time.deltaTime;

		if(collisionReady) {
			// Verifier si on a fait une collision depuis la derniere fois.
			Vector3 direction = transform.position - lastPosition;
			RaycastHit hitInfo;
			if (Physics.Raycast (lastPosition, direction, out hitInfo,
			                     direction.magnitude + rayon * kMultiplicateurRayon,
			                     guitarPlayerLayer)) {
				if (hitInfo.collider.gameObject.tag == "GuitarPlayer"){
					if (AssistedModeControllerGuitar.EstActive() &&
					    tempsProchaineNote < 0.15f) {
						// Ne pas jouer la note tout de suite.
						aJoueBienDepuisDerniereFois = true;

						Debug.Log("temps prochaine note: " + tempsProchaineNote);
					} else {
						if (tempsDepuisDerniereNoteAutomatique < kTempsRejouerApresAutomatique) {
							tempsDepuisDerniereNoteAutomatique = 1000.0f;
						} else {
							PlayNote ();
							Debug.Log("en trop");
							aJoueMalDepuisDerniereFois = true;
						}
					}
				}
			}
		}
	}

	void PlayNote() {
		guitarPlayer.PlayNextNote();
		// Se rappeler que l'on a joue ce component. Il faut s'en eloigner
		// suffisamment avant de le rejouer.
		collisionReady = false;
		
		// Allumer la lumiere verte.
		greenLight.renderer.material.color = Color.white;
		greenLight.SetActive (true);
		tempsDepuisDerniereNote = 0;
	}

	void OnEnable() {
		// Desactiver la lumiere verte.
		greenLight.SetActive (false);
		greenLight.renderer.material.color = Color.black;

		for (int i = 0; i < dernieresDistances.Length; ++i) {
			dernieresDistances[i] = 0;
		}
		indexDernieresDistance = 1;
		tempsDepuisDerniereNoteAutomatique = 1000.0f;
		aJoueMalDepuisDerniereFois = false;
		aJoueBienDepuisDerniereFois = false;
	}

	// Indique dans combien de temps la prochaine note du mode assisté
	// doit etre jouée.
	public void DefinirTempsProchaineNote(float temps) {
		Debug.Log ("definir " + temps);
		tempsProchaineNote = temps;
	}

	// Utilise par le mode assiste pour indiquer quand une note doit
	// etre jouee immediatement selon la partition. La note est jouee
	// si la main du guitariste semble etre en voie de jouer la note.
	public void JouerNoteMaintenant() {
		// On accepte de jouer la note si la main est suffisamment pres
		// des cordes et qu'elle se dirige vers celles-ci.
		float vitesse = dernieresDistances [indexDernieresDistance] -
			dernieresDistances [(indexDernieresDistance + 1) % dernieresDistances.Length];

		if ((dernieresDistances[indexDernieresDistance] < 1.0f && vitesse < -0.01f) ||
		     aJoueBienDepuisDerniereFois) {
			PlayNote();

			if (!aJoueBienDepuisDerniereFois) {
				tempsDepuisDerniereNoteAutomatique = 0;
				Debug.Log("auto-play proximite");
			} else {
				Debug.Log("auto-play en avance");
				tempsDepuisDerniereNoteAutomatique = 0;
			}
		} else {
			tempsDepuisDerniereNoteAutomatique = 1000.0f;
			Debug.Log("miss");
		}
		aJoueMalDepuisDerniereFois = false;
		aJoueBienDepuisDerniereFois = false;
	}

	private bool aJoueMalDepuisDerniereFois = false;
	private bool aJoueBienDepuisDerniereFois = false;

	public void ReinitialiserMouvementsAmples() {
		numMouvementsAmples = 0;
		dernierAuDessus = false;
	}

	public int ObtenirNumMouvementsAmples() {
		return numMouvementsAmples;
	}
	
	// Derniere position du tip.
	private Vector3 lastPosition;
	
	// Rayon du tip.
	float rayon;
	
	// Multiplicateur du rayon pour produire un son.
	const float kMultiplicateurRayon = 1.0f;
	
	// Distance dont il faut s'eloigner du GuitPlayer pour le rejouer.
	const float kDistancePourRejouer = 0.35f;

	// Dernier drum component a avoir ete joue.
	GuitarPlayer guitarPLayerInterface;
	
	// Layer des colliders de guitar components.
	int guitarPlayerLayer;

	// Indique si on s'est eloigne suffisamment de la note
	// avant de la rejouer.
	bool collisionReady;

	// Compteur de mouvements amples.
	int numMouvementsAmples = 0;

	// Distance dont il faut s'eloigner de la guitare pour que le mouvement soit ample.
	const float kDistanceAmple = 1.0f;

	// Indique si le dernier mouvement ample etait au-dessus ou en-dessous de la guitare.
	bool dernierAuDessus = false;

	// Temps écoulé depuis la derniere note.
	float tempsDepuisDerniereNote = 0;

	// Dernieres distances entre le collider et la main.
	float[] dernieresDistances = new float[2];

	// Index de la derniere donnee ecrite dans le tableau de dernieres distances.
	int indexDernieresDistance = 1;

	// Compteur depuis la derniere inscription d'une distance.
	float compteurDistance = 0;

	// Temps nécessaire pour inscrire une nouvelle distance.
	const float kTempsPourEcrireDistance = 1.0f / 30.0f;

	// Indique le temps depuis la derniere note automatique.
	float tempsDepuisDerniereNoteAutomatique = 1000.0f;

	// Temps pour rejouer une note apres une note automatique.
	float kTempsRejouerApresAutomatique = 0.4f;

	// Temps avant de jouer la prochaine note.
	float tempsProchaineNote = 0;
}
