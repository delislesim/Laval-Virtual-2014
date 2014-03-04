using UnityEngine;
using System.Collections;

public class HighHatComponent : MonoBehaviour, ComponentInterface {
	public bool opened;
	public AudioClip soundOpened;
	public AudioClip soundClosed;

	// Plan situe sur la surface du composant a jouer.
	public GameObject plane;

	// Spotlight qui eclaire ce composant.
	public DrumSpot spot;

	public void PlaySound()
	{
		if(opened){
			audio.clip = soundOpened;
			audio.Play();
		}
		else{
			audio.clip = soundClosed;
			audio.Play();
		}
		spot.Play ();
	}

	public float DistanceToPoint (Vector3 point) {
		return planeMath.GetDistanceToPoint (point);
	}

	public void DefinirLuminosite(float luminosite) {
		spot.DefinirLuminosite (luminosite);
	}
	
	// Use this for initialization
	void Start () {
		opened = false;

		// Calculer le plan situe sur la surface a jouer.
		Vector3 normal = plane.transform.rotation * Vector3.up;
		planeMath = new Plane (normal.normalized, plane.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Tip")
			PlaySound();
	}
	
	void OnMouseDown()
	{
		PlaySound();
	}

	// Representation mathematique du plan situe sur sur la surface
	// du composant a jouer.
	private Plane planeMath;
}