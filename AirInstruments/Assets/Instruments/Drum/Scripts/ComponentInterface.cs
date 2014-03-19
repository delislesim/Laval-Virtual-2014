using System;
using UnityEngine;

public interface ComponentInterface
{
	void PlaySound ();
	void PlaySoundWhenAssisted();
	float DistanceToPoint (Vector3 point);
	void DefinirLuminosite(float luminosite);
	void AjouterCoupAuTemps();
	int GetCoupsDernierTemps();
	void ResetCoupsDernierTemps();

	// Indique que le composant doit etre joue.
	void DoitEtreJoue();

	// Indique si le composant a ete joue depuis la derniere 
	// fois qu'on a demande qu'il soit joue.
	bool AEteJoue();
}


