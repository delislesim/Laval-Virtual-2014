using UnityEngine;
using System.Collections;

public class FingerSphere : MonoBehaviour, HandJointSphereI {

	// Use this for initialization
	void Start () {
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsValid ()) {
			return;
		}

		// Trouver des objets en collision avec cette boule.
		Collider[] hitColliders = Physics.OverlapSphere (transform.position, rayon);
		for (int i = 0; i < hitColliders.Length; ++i) {
			Collider collider = hitColliders[i];
			PianoNote note = collider.GetComponent<PianoNote>();
			if (note != null) {
				note.ToucherAvecSphere(this, estDescenduSousBlanches);
			}
		}
	}

	public void Reset() {
		initialized = false;
	}

	public void SetTargetPosition(Vector3 targetPosition, bool valid) {
		// Gerer les etats invalides.
		this.valid = valid;
		if (!valid) {
			++compteurInvalide;
			if (compteurInvalide > kCompteurInvalideMax) {
				renderer.enabled = false;
			}
			return;
		}
		compteurInvalide = 0;
		renderer.enabled = true;
		
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

		// Noter si on est sous les blanches / au-desuss des noires.
		float basSphere = transform.position.y - ObtenirRayon ();
		if (basSphere < kHauteurBlanches) {
			estDescenduSousBlanches = true;
		} else if (basSphere > kHauteurNoires) {
			estDescenduSousBlanches = false;
		}
	}

	// Retourne le rayon de la sphere representant un doigt.
	public float ObtenirRayon() {
		return rayon;
	}

	public bool IsValid() {
		return valid && compteurInvalide <= kCompteurInvalideMax;
	}
	
	private bool initialized = false;
	
	private Kalman kalman = new Kalman(1.0f);

	// Rayon de la sphere en coordonnes du monde.
	private float rayon;

	// Indique que le doigt est descendu sous le niveau des notes
	// blanches sans aller au-dessus des notes noires depuis.
	private bool estDescenduSousBlanches = false; 

	// Hauteur des notes blanches.
	private const float kHauteurBlanches = 2.171061f;

	// Hauteur des notes noires.
	private const float kHauteurNoires = 2.33302f;

	// Gerer les donnees invalides.
	private bool valid = false;
	private int compteurInvalide = 0;
	private const int kCompteurInvalideMax = 10;
}
