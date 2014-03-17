using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour, ComponentInterface {

	// Plan situe sur la surface du composant a jouer.
	public GameObject plane;

	// Spotlight qui eclaire ce composant.
	public DrumSpot spot;

	// Materiel pour indiquer que le composant doit etre joue.
	public Material materielDoitEtreJoue;

	// GameObject qui doit changer de couleur pour indiquer que
	// ce drum component doit etre joue.
	public GameObject gameObjectVisible;

	public void PlaySound()
	{
		//audio.Play();
		if (spot != null) {
			spot.Play ();
		}
		aEteJoue = true;
		gameObjectVisible.renderer.material = materialDefaut;
	}

	public float DistanceToPoint (Vector3 point) {
		return planeMath.GetDistanceToPoint (point);
	}

	public void DefinirLuminosite(float luminosite) {
		spot.DefinirLuminosite (luminosite);
	}

	// Use this for initialization
	void Start () {
		// Calculer le plan situe sur la surface a jouer.
		if (plane != null) {
			Vector3 normal = plane.transform.rotation * Vector3.up;
			planeMath = new Plane (normal.normalized, plane.transform.position);
		}

		// Sauvegarder le materiel par defaut.
		materialDefaut = gameObjectVisible.renderer.material;
	}

	public void AjouterCoupAuTemps()
	{
		nbCoupsDernierTemps ++;
	}
	public int GetCoupsDernierTemps()
	{
		return nbCoupsDernierTemps;
	}
	public void ResetCoupsDernierTemps()
	{
		nbCoupsDernierTemps = 0;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown()
	{
		PlaySound();
	}

	// Indique que le composant doit etre joue.
	public void DoitEtreJoue() {
		aEteJoue = false;
		gameObjectVisible.renderer.material = materielDoitEtreJoue;
	}
	
	// Indique si le composant a ete joue depuis la derniere 
	// fois qu'on a demande qu'il soit joue.
	public bool AEteJoue() {
		return aEteJoue;
	}

	private bool aEteJoue = false;

	// Representation mathematique du plan situe sur sur la surface
	// du composant a jouer.
	private Plane planeMath;

	// Materiel par defaut.
	private Material materialDefaut;

	private int nbCoupsDernierTemps;
}