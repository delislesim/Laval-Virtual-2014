using UnityEngine;
using System.Collections;

public class TipFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;
	private bool isAssisted;

	// Use this for initialization
	void Start () {
		// Calculer le rayon du tip.
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
		isAssisted = true;
		// Layer des colliders de drum components.
		drumComponentLayer = 1 << LayerMask.NameToLayer ("DrumComponent");
	}
	
	// Update is called once per frame
	void Update () {
		lastPosition = transform.position;

		// Suivre l'objet auquel on a été assigné.
		transform.position = objectToFollow.transform.position;

		// Si on a joue un drum component recemment, s'assurer qu'on s'en eloigne avant de le rejouer.
		if (dernierDrumComponent != null) {
			float distance = dernierDrumComponent.DistanceToPoint(transform.position);

			if (distance > distanceDernierDrumComponent + kDistancePourRejouer) {
				dernierDrumComponent = null;
			} else if (distance < distanceDernierDrumComponent && distance >= 0) {
				distance = distanceDernierDrumComponent;
			}

		}

		// Verifier si on a fait une collision depuis la derniere fois.
		Vector3 direction = transform.position - lastPosition;
		RaycastHit hitInfo;
		if (Physics.Raycast (lastPosition, direction, out hitInfo, direction.magnitude + kDistanceRaycast, drumComponentLayer)) {

			GameObject drumComponentGameObject = hitInfo.collider.gameObject;
			ComponentInterface componentInterface = drumComponentGameObject.GetComponent<DrumComponent>();
			if (componentInterface == null) {
				componentInterface = drumComponentGameObject.GetComponent<HighHatComponent>();
			}

			float distanceReelle = hitInfo.distance - direction.magnitude;
			if (distanceReelle < 0)
				distanceReelle = 0;

			if (componentInterface != dernierDrumComponent && distanceReelle < kDistancePourJouer) {

				if(!isAssisted){
					// Jouer le son.
					componentInterface.PlaySound();
				}
				//On enregistre le coup
				componentInterface.AjouterCoupAuTemps();

				// Se rappeler que l'on a joue ce component.
				dernierDrumComponent = componentInterface;
				distanceDernierDrumComponent = componentInterface.DistanceToPoint(transform.position);
			}
		}
	}

	// Derniere position du tip.
	private Vector3 lastPosition;
	
	// Rayon du tip.
	float rayon;

	// Dernier drum component a avoir ete joue.
	ComponentInterface dernierDrumComponent;

	// Distance entre le bout de la baguette et le drum component lors de l'impact.
	float distanceDernierDrumComponent;

	// Layer des colliders de drum components.
	int drumComponentLayer;

	// Distance dont il faut s'eloigner du drum component pour le rejouer.
	const float kDistancePourRejouer = 0.1f;

	// Distance pour le raycast.
	const float kDistanceRaycast = 7.0f * 0.1f;

	// Distance pour jouer un instrument du drum.
	const float kDistancePourJouer = 4.0f * 0.1f;
}
