using UnityEngine;
using System.Collections;

public class HandFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;
	public ColliderManager guitarCollider;
	
	// Use this for initialization
	void Start () {
		// Calculer le rayon du tip.
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
		
		// Layer des colliders de drum components.
		//drumComponentLayer = 1 << LayerMask.NameToLayer ("DrumComponent");
	}
	
	// Update is called once per frame
	void Update () {
		lastPosition = transform.position;
		
		// Suivre l'objet auquel on a été assigné.
		transform.position = objectToFollow.transform.position;
		
		// Si on a joue une note recemment, s'assurer qu'on s'en eloigne avant de le rejouer.
		if (guitarCollider != null) {
			float distance = guitarCollider.DistanceToPoint(transform.position);
			//Debug.Log(distance);
			
			if (distance > distanceDernierDrumComponent + kDistancePourRejouer) {
				guitarCollider = null;
			} else if (distance < distanceDernierDrumComponent && distance >= 0) {
				distance = distanceDernierDrumComponent;
			}
			
		}
		
		// Verifier si on a fait une collision depuis la derniere fois.
		Vector3 direction = transform.position - lastPosition;
		RaycastHit hitInfo;
		/*
		if (Physics.Raycast (lastPosition, direction, out hitInfo, direction.magnitude + rayon * kMultiplicateurRayon, drumComponentLayer)) {
			
			GameObject possibleGuitarColliderObject = hitInfo.collider.gameObject;	
			if (componentInterface != guitarCollider) {
				// Jouer le son.
				componentInterface.PlaySound();
				
				// Se rappeler que l'on a joue ce component.
				distanceDernierDrumComponent = componentInterface.DistanceToPoint(transform.position);
			}
		}
		*/
	}
	
	// Derniere position du tip.
	private Vector3 lastPosition;
	
	// Rayon du tip.
	float rayon;
	
	// Multiplicateur du rayon pour produire un son.
	const float kMultiplicateurRayon = 4.0f;
	
	// Distance entre le bout de la baguette et le drum component lors de l'impact.
	float distanceDernierDrumComponent;
	
	// Distance dont il faut s'eloigner du drum component pour le rejouer.
	const float kDistancePourRejouer = 0.1f;
	
	// Layer des colliders de drum components.
	int drumComponentLayer;
}
