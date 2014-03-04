using UnityEngine;
using System.Collections;

public class TipFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;

	// Use this for initialization
	void Start () {
		// Calculer le rayon du tip.
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;

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
			float distance = (pointCollision - transform.position).magnitude;

			if (distance > distanceDernierDrumComponent + kDistancePourRejouer) {
				dernierDrumComponent = null;
			} else if (distance < distanceDernierDrumComponent) {
				distance = distanceDernierDrumComponent;
			}

			//Debug.Log("dist: " + distance + "pos: " + transform.position + " plusPres: " + pointLePlusPres);
		}

		// Verifier si on a fait une collision depuis la derniere fois.
		Vector3 direction = transform.position - lastPosition;
		RaycastHit hitInfo;
		if (Physics.Raycast (lastPosition, direction, out hitInfo, direction.magnitude + rayon * kMultiplicateurRayon, drumComponentLayer)) {
			// Retrouver la game object et le script du drum component touché.
			GameObject drumComponentGameObject = hitInfo.collider.gameObject;
			DrumComponent drumComponent = (DrumComponent)drumComponentGameObject.GetComponent(typeof(DrumComponent));


			if (drumComponent != dernierDrumComponent) {
				drumComponent.PlaySound();

				// Se rappeler que l'on a joue ce component.
				dernierDrumComponent = drumComponent;
				pointCollision = hitInfo.point;
				distanceDernierDrumComponent = (pointCollision - transform.position).magnitude;
			}
		}
	}

	void OnEnable () {
	}

	// Derniere position du tip.
	private Vector3 lastPosition;
	
	// Rayon du tip.
	float rayon;

	// Multiplicateur du rayon pour produire un son.
	const float kMultiplicateurRayon = 4.0f;

	// Dernier drum component a avoir ete joue.
	DrumComponent dernierDrumComponent;

	// Point de collision avec le dernier drum component.
	Vector3 pointCollision;

	// Distance lorsque le dernier drum component a ete joue.
	float distanceDernierDrumComponent;

	// Distance dont il faut s'eloigner du drum component pour le rejouer.
	const float kDistancePourRejouer = 0.05f;

	// Layer des colliders de drum components.
	int drumComponentLayer;
}
