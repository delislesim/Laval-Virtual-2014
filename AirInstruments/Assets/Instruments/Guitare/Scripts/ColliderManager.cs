using UnityEngine;
using System.Collections.Generic;

public class ColliderManager : MonoBehaviour {

	//rightHandPos
	public Transform RightHand;

	//Keep track of right hand speed
	private Queue<Vector3> last_positions;

	private BoxCollider box_collider;

	// Use this for initialization
	void Start () {
		last_positions = new Queue<Vector3>();
		last_positions.Enqueue(new Vector3(0.0f, 0.0f, 0.0f));
		last_positions.Enqueue(new Vector3(0.0f, 0.0f, 0.0f));
		//HandCylinder handCylinder = (HandCylinder)cylindres [index].GetComponent (typeof(HandCylinder));
		box_collider = (BoxCollider)this.GetComponent(typeof(BoxCollider));
	}
	
	// Update is called once per frame
	void Update () {
		/*Vector3 speed = RightHand.position - last_positions.Dequeue();
		last_positions.Enqueue(RightHand.position);

		if(box_collider != null){
			box_collider.transform.localScale = new Vector3(box_collider.transform.localScale.x, speed.magnitude, box_collider.transform.localScale.z);
			Debug.Log ("Speed :  " + speed);
		}*/
	}
}
