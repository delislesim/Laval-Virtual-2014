using UnityEngine;
using System;

public interface HandJointSphereI
{
	void SetTargetPosition (Vector3 targetPosition, bool valid);
	bool IsValid();
}

