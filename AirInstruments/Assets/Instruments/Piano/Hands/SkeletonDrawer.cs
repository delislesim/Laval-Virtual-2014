using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KinectHelpers;

public class SkeletonDrawer : MonoBehaviour {
	// Prefab de cylindre.
	public GameObject cylindrePrefab;

	// Indique si on est la guitare.
	public bool estGuitare;

	// Joints
	public List<GameObject> spine_spheres = new List<GameObject>();
	public List<GameObject> left_arm_spheres = new List<GameObject>();
	public List<GameObject> right_arm_spheres = new List<GameObject>();
	public List<GameObject> left_leg_spheres = new List<GameObject>();
	public List<GameObject> right_leg_spheres = new List<GameObject>();
	
	// Liste des cylindres affiches dans la scene.
	private List<GameObject> cylindres = new List<GameObject> ();
	
	// Nombre de boules rouges a créer.
	private const int NB_SPHERES = (int)Skeleton.Joint.Count;
	
	// Nombre de cylindre a créer par main.
	private const int NB_CYLINDRES = 19;

	// Layer d'affichage prioritaire.
	private int kLayerPrioritaire;

	// Layer d'affichage par défaut.
	private int kLayerDefault;

	// Use this for initialization
	void Start () {
		kLayerPrioritaire = LayerMask.NameToLayer ("AffichagePrioritaireExtra");
		kLayerDefault = LayerMask.NameToLayer ("Default");

		// Creer les cylindres.
		for (int i = 0; i < NB_CYLINDRES; ++i) {
			GameObject cylindre = (GameObject)Instantiate (cylindrePrefab);
			cylindre.transform.parent = this.transform;
			cylindre.transform.localScale = cylindrePrefab.transform.localScale;
			cylindre.SetActive (false);
			cylindres.Add (cylindre);
		}
	}
	
	// Update is called once per frame
	void Update () {
		PlacerCylindres();

		// Rotation des mains.
		left_arm_spheres[3].transform.rotation = cylindres[5].transform.rotation;
		right_arm_spheres[3].transform.rotation = cylindres[8].transform.rotation;
	}

	//TODO : avoir un tableau de listes pour avoir une seule boucle for double (imbriquée)
	void PlacerCylindres()
	{
		int i = 0;
		int boneIdx = 0;

		//Spine
		for(i = 1 ; i < spine_spheres.Count ; i++)
		{	
			if (spine_spheres[i-1]!=null && spine_spheres[i]!=null &&
			    spine_spheres[i-1].renderer.enabled && spine_spheres[i].renderer.enabled) {
				HandCylinder handCylinder = (HandCylinder)cylindres[boneIdx].GetComponent (typeof(HandCylinder));
				handCylinder.DefinirExtremites (spine_spheres[i-1].transform.position,
				                                spine_spheres[i].transform.position);
				cylindres[boneIdx].SetActive(true);
			} else {
				cylindres[boneIdx].SetActive(false);
			}
			boneIdx++;
		}
		//LeftArm
		for(i = 1 ; i < left_arm_spheres.Count ; i++)
		{	
			if (left_arm_spheres[i-1]!=null && left_arm_spheres[i]!=null &&
			    left_arm_spheres[i-1].renderer.enabled && left_arm_spheres[i].renderer.enabled) {
				HandCylinder handCylinder = (HandCylinder)cylindres[boneIdx].GetComponent (typeof(HandCylinder));
				handCylinder.DefinirExtremites (left_arm_spheres[i-1].transform.position,
				                                left_arm_spheres[i].transform.position);
				cylindres[boneIdx].SetActive(true);

				if (estGuitare) {
					if (GuitareController.JoueurEstVisible()) {
						cylindres[boneIdx].layer = kLayerPrioritaire;
					} else {
						cylindres[boneIdx].layer = kLayerDefault;
					}
				}
			} else {
				cylindres[boneIdx].SetActive(false);
			}
			boneIdx++;
		}

		//RightArm
		for(i = 1 ; i < right_arm_spheres.Count ; i++)
		{	
			if (right_arm_spheres[i-1]!=null && right_arm_spheres[i]!=null &&
			    right_arm_spheres[i-1].renderer.enabled && right_arm_spheres[i].renderer.enabled) {
				HandCylinder handCylinder = (HandCylinder)cylindres[boneIdx].GetComponent (typeof(HandCylinder));
				handCylinder.DefinirExtremites (right_arm_spheres[i-1].transform.position,
				                                right_arm_spheres[i].transform.position);
				cylindres[boneIdx].SetActive(true);

				if (estGuitare) {
					if (GuitareController.JoueurEstVisible()) {
						cylindres[boneIdx].layer = kLayerPrioritaire;
					} else {
						cylindres[boneIdx].layer = kLayerDefault;
					}
				}
			} else {
				cylindres[boneIdx].SetActive(false);
			}
			boneIdx++;
		}

		//LeftLeg
		for(i = 1 ; i < left_leg_spheres.Count ; i++)
		{	
			if (left_leg_spheres[i-1]!=null && left_leg_spheres[i]!=null &&
			    left_leg_spheres[i-1].renderer.enabled && left_leg_spheres[i].renderer.enabled) {
				HandCylinder handCylinder = (HandCylinder)cylindres[boneIdx].GetComponent (typeof(HandCylinder));
				handCylinder.DefinirExtremites (left_leg_spheres[i-1].transform.position,
				                                left_leg_spheres[i].transform.position);
				cylindres[boneIdx].SetActive(true);
			} else {
				cylindres[boneIdx].SetActive(false);
			}
			boneIdx++;
		}

		//RightLeg
		for(i = 1 ; i < right_leg_spheres.Count ; i++)
		{	
			if (right_leg_spheres[i-1]!=null && right_leg_spheres[i]!=null  &&
			    right_leg_spheres[i-1].renderer.enabled && right_leg_spheres[i].renderer.enabled) {
				HandCylinder handCylinder = (HandCylinder)cylindres[boneIdx].GetComponent (typeof(HandCylinder));
				handCylinder.DefinirExtremites (right_leg_spheres[i-1].transform.position,
				                                right_leg_spheres[i].transform.position);
				cylindres[boneIdx].SetActive(true);
			} else {
				cylindres[boneIdx].SetActive(false);
			}
			boneIdx++;
		}


	}
}
