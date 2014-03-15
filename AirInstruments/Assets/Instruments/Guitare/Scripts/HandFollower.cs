using UnityEngine;
using System.Collections;

public class HandFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;
	public ColliderManager guitarCollider;
	public GuitarPlayer guitarPlayer;
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
		lastPosition = transform.position;
		
		// Suivre l'objet auquel on a été assigné.
		transform.position = objectToFollow.transform.position;
		
		// Si on a joue une note recemment, s'assurer qu'on s'en eloigne avant de le rejouer.
		if (guitarCollider != null) {
			float distance = guitarCollider.DistanceToPoint(transform.position);
			if (Mathf.Abs(distance) > kDistancePourRejouer) {
				collisionReady = true;
			}

			if (distance > kDistanceAmple && !dernierAuDessus) {
				++numMouvementsAmples;
				dernierAuDessus = true;
			} else if (distance < kDistanceAmple && dernierAuDessus) {
				++numMouvementsAmples;
				dernierAuDessus = false;
			}
		}

		if(collisionReady){
			// Verifier si on a fait une collision depuis la derniere fois.
			Vector3 direction = transform.position - lastPosition;
			RaycastHit hitInfo;
			//Physics.Raycast (
			if (Physics.Raycast (lastPosition, direction, out hitInfo,
			                     direction.magnitude + rayon * kMultiplicateurRayon,
			                     guitarPlayerLayer)) {
				if (hitInfo.collider.gameObject.tag == "GuitarPlayer"){	
					guitarPlayer.PlayNextNote();
					// Se rappeler que l'on a joue ce component. Il faut s'en eloigner
					// suffisamment avant de le rejouer.
					collisionReady = false;
				}
			}
		}
	}

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
}
