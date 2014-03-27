using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KinectHelpers;

public class SkeletonDrawer : MonoBehaviour {
	// Prefab de cylindre.
	public GameObject cylindrePrefab;

	// Indique si on est la guitare.
	public bool estGuitare;

	// Joints, dans l'ordre declare dans KinectPowerInterop.
	public List<GameObject> joints;

	// Liste des cylindres affiches dans la scene.
	private List<GameObject> cylindres = new List<GameObject> ();
	
	// Nombre de boules rouges a créer.
	private const int NB_SPHERES = (int)Skeleton.Joint.Count;
	
	// Nombre de cylindre a créer par main.
	private const int NB_CYLINDRES = 19;

	// Layer d'affichage prioritaire.
	private int kLayerPrioritaire;

	// Use this for initialization
	void Start () {
		kLayerPrioritaire = LayerMask.NameToLayer ("AffichagePrioritaireExtra");

		// Creer les cylindres.
		for (int i = 0; i < NB_CYLINDRES; ++i) {
			GameObject cylindre = (GameObject)Instantiate (cylindrePrefab);
			cylindre.transform.parent = this.transform;
			cylindre.transform.localScale = cylindrePrefab.transform.localScale;
			cylindre.SetActive (false);
			cylindres.Add (cylindre);
		}

		// Ajuster les layers.
		int indexCylindre = 0;
		for (int i = 0; i < (int)Skeleton.Joint.Count; ++i) {
			Skeleton.Joint start = (Skeleton.Joint)i;
			Skeleton.Joint end = Skeleton.GetSkeletonJointParent(start);
			if (start != end) {
				if (estGuitare && (start == Skeleton.Joint.HandRight ||
				                   start == Skeleton.Joint.WristRight ||
				                   start == Skeleton.Joint.ElbowRight )) {
					cylindres[indexCylindre].layer = kLayerPrioritaire;
				}
				++indexCylindre;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (KinectPowerInterop.IsKinect1) {
			// Rotation des mains.
			joints[(int)Skeleton.Joint.HandLeft].transform.rotation = cylindres[5].transform.rotation;
			joints[(int)Skeleton.Joint.HandRight].transform.rotation = cylindres[5].transform.rotation;
		}
	}

	public void PlacerCylindres()
	{
		int indexCylindre = 0;
		for (int i = 0; i < (int)Skeleton.Joint.Count; ++i) {
			Skeleton.Joint start = (Skeleton.Joint)i;
			Skeleton.Joint end = Skeleton.GetSkeletonJointParent(start);
			if (start != end) {
				HandCylinder cylindre = cylindres[indexCylindre].GetComponent<HandCylinder>();
				cylindre.DefinirExtremites(joints[(int)start].transform.position, joints[(int)end].transform.position);
				++indexCylindre;
			}
		}
	}
}
