using System;
using UnityEngine;

public interface ComponentInterface
{
	void PlaySound ();
	float DistanceToPoint (Vector3 point);
	void DefinirLuminosite(float luminosite);
	void AjouterCoupAuTemps();
	int GetCoupsDernierTemps();
	void ResetCoupsDernierTemps();
}


