using UnityEngine;
using System.Collections;

public class DrumComponent : MonoBehaviour, ComponentInterface {

	// Plan situe sur la surface du composant a jouer.
	public GameObject plane;

	// Spotlight qui eclaire ce composant.
	public DrumSpot spot;



	public void PlaySound()
	{
		audio.Play();
		if (spot != null) {
			spot.Play ();
		}
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
	
	void OnCollisionEnter(Collision col)
	{
		/*if(col.gameObject.tag == "Tip")
			PlaySound();*/
	}

	// Representation mathematique du plan situe sur sur la surface
	// du composant a jouer.
	private Plane planeMath;

	private int nbCoupsDernierTemps;
}