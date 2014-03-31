using UnityEngine;
using System.Collections;

public class TipFollower : MonoBehaviour {

	// Objet que le tip doit suivre.
	public GameObject objectToFollow;
	public DrumComponent Crash;

	// Use this for initialization
	void Start () {
		// Calculer le rayon du tip.
		Vector3 worldScale = VectorConversions.CalculerWorldScale (transform);
		rayon = worldScale.x / 2.0f;
		// Layer des colliders de drum components.
		drumComponentLayer = 1 << LayerMask.NameToLayer ("DrumComponent");
		composanteVisee = null;
		timeSinceLastHit = 0;
		resetLastComponentMemory();
		resetLastOldComponentMemory();
	}
	
	// Update is called once per frame
	void Update () {
		beforeLastPos = lastPosition;
		lastPosition = transform.position;

		currentSpeed = Vector3.Distance (transform.position , beforeLastPos) / Time.deltaTime;
		timeSinceLastHit = timeSinceLastHit + Time.deltaTime;

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
			composanteVisee = drumComponentGameObject;
			DrumComponent componentInterface = drumComponentGameObject.GetComponent<DrumComponent>();

			float distanceReelle = hitInfo.distance - direction.magnitude;
			if (distanceReelle < 0)
				distanceReelle = 0;

			//Coup valide
			if (componentInterface != dernierDrumComponent && distanceReelle < kDistancePourJouer) {
				timeSinceLastHit = 0 ;
				if(componentInterface == Crash)
				{
					componentInterface.PlaySoundWhenAssisted();
					FeuDrum.BurstCrash();
				}
				else if(DrumAssistedController.EstActive() && dernierLongMemory != componentInterface)
				{
					if(componentInterface !=null)
					{
						//Debug.Log ("componentInterface: " + componentInterface.name);
						dernierLongMemory = componentInterface;
						if(dernierLongMemory != dernierVeryLongMemory)
							componentInterface.PlaySoundWhenAssisted();

						dernierVeryLongMemory = dernierLongMemory;
					}
				}
				else
				{
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

	public void resetLastComponentMemory()
	{
		dernierLongMemory = null;
	}

	public void resetLastOldComponentMemory()
	{
		dernierVeryLongMemory = null;
	}

	public float GetSpeed()
	{
		return 	currentSpeed;
	}

	public GameObject GetAimedComponent()
	{
		return composanteVisee;
	}

	public DrumComponent GetLastComponentHit()
	{
		return dernierLongMemory;
	}

	public float GetTimeSinceLastHit()
	{
		return timeSinceLastHit;
	}

	// Derniere position du tip.
	private Vector3 lastPosition;
	private Vector3 beforeLastPos;

	//Vitesse
	private float currentSpeed;

	//DrumComponent visée
	private GameObject composanteVisee;
	
	// Rayon du tip.
	float rayon;

	// Dernier drum component a avoir ete joue.
	DrumComponent dernierDrumComponent;

	// Dernier drum component a avoir ete joué et persistent!
	DrumComponent dernierLongMemory;

	// Dernier drum component a avoir ete joué et tres persistent!
	DrumComponent dernierVeryLongMemory;

	// Distance entre le bout de la baguette et le drum component lors de l'impact.
	float distanceDernierDrumComponent;

	// Temps accumulé depuis la derniere collision avec un DrumCompnent.
	float timeSinceLastHit;

	// Layer des colliders de drum components.
	int drumComponentLayer;

	// Distance dont il faut s'eloigner du drum component pour le rejouer.
	const float kDistancePourRejouer = 0.1f;

	// Distance pour le raycast.
	const float kDistanceRaycast = 7.0f * 0.1f;

	// Distance pour jouer un instrument du drum.
	const float kDistancePourJouer = 4.0f * 0.1f;

}
