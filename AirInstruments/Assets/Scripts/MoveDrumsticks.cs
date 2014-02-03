using UnityEngine;
using System.Collections;
using KinectHelpers;

public class MoveDrumsticks : MonoBehaviour
{
    public GameObject drumstickRight;
    public GameObject drumstickLeft;

	public GameObject rightHand;
	public GameObject leftHand;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		//Create valid skeleton with joints positions/rotations
		//Skeleton playerOne = new Skeleton(0);

		drumstickRight.transform.position = rightHand.transform.position;
		drumstickLeft.transform.position = leftHand.transform.position;
    }    
}
