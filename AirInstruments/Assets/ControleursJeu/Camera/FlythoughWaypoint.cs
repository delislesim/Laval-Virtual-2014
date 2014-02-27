/* Sam Cox - 2009 - FrictionPointStudios.com 
 */

using UnityEngine;
using System.Collections;

public class FlythoughWaypoint : MonoBehaviour {

    public Vector3 CurrentPosition
    {
        get
        {
            return transform.position;
        }
    }

	public Quaternion CurrentRotation
	{
		get
		{
			return transform.rotation;
		}
	}
}
