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
		Skeleton playerOne = new Skeleton(0);
		//moveDrumsticks (playerOne);
    }

	void moveDrumsticks(Skeleton player)
	{
		drumstickRight.transform.position = rightHand.transform.position;
		drumstickLeft.transform.position = leftHand.transform.position;
		
		Quaternion rotRightDrumstick;
		Quaternion rotLeftDrumstick;
		rotRightDrumstick = player.GetBoneOrientation(Skeleton.Joint.HandRight);
		rotLeftDrumstick = player.GetBoneOrientation (Skeleton.Joint.HandLeft);

		drumstickRight.transform.localRotation = rotRightDrumstick;
		drumstickLeft.transform.localRotation = rotLeftDrumstick;
	}
}
