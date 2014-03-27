using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KinectHelpers;

public class SkeletonDrawer : MonoBehaviour {
	// Prefab de cylindre.
	public GameObject cylindrePrefab;

	// Joints.
	public GameObject Spine_Base;
	public GameObject Spine_Mid;
	public GameObject Neck;
	public GameObject Head;
	public GameObject Shoulder_Left;
	public GameObject Elbow_Left;
	public GameObject Wrist_Left;
	public GameObject Hand_Left;
	public GameObject Shoulder_Right;
	public GameObject Elbow_Right;
	public GameObject Wrist_Right;
	public GameObject Hand_Right;
	public GameObject Hip_Left;
	public GameObject Knee_Left;
	public GameObject Ankle_Left;
	public GameObject Foot_Left;
	public GameObject Hip_Right;
	public GameObject Knee_Right;
	public GameObject Ankle_Right;
	public GameObject Foot_Right;
	public GameObject Spine_Shoulder;
	public GameObject Hand_Tip_Left;
	public GameObject Thumb_Left;
	public GameObject Hand_Tip_Right;
	public GameObject Thumb_Right;

	// Indique si on est la guitare.
	public bool estGuitare;

	// Joints, dans l'ordre declare dans KinectPowerInterop.
	private GameObject[] joints;

	// Liste des cylindres affiches dans la scene.
	private List<GameObject> cylindres = new List<GameObject> ();
	
	// Nombre de boules rouges a créer.
	private const int NB_SPHERES = (int)Skeleton.Joint.Count;
	
	// Nombre de cylindre a créer par main.
	private const int NB_CYLINDRES = (int)Skeleton.Joint.Count - 1;

	// Layer d'affichage prioritaire.
	private int kLayerPrioritaire;

	// Use this for initialization
	void Start () {
		kLayerPrioritaire = LayerMask.NameToLayer ("AffichagePrioritaireExtra");

		// Creer une liste de joints.
		joints = new GameObject[(int)Skeleton.Joint.Count] {
			Spine_Base, Spine_Mid, Neck, Head, Shoulder_Left, Elbow_Left,
			Wrist_Left, Hand_Left, Shoulder_Right, Elbow_Right, Wrist_Right,
			Hand_Right, Hip_Left, Knee_Left, Ankle_Left, Foot_Left, Hip_Right,
			Knee_Right, Ankle_Right, Foot_Right, Spine_Shoulder, Hand_Tip_Left,
			Thumb_Left, Hand_Tip_Right, Thumb_Right
		};

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
		/*
		if (KinectPowerInterop.IsKinect1) {
			// Rotation des mains.
			joints[(int)Skeleton.Joint.HandLeft].transform.rotation = cylindres[5].transform.rotation;
			joints[(int)Skeleton.Joint.HandRight].transform.rotation = cylindres[5].transform.rotation;
		}
		*/
	}

	public void PlacerCylindres()
	{
		int indexCylindre = 0;
		for (int i = 0; i < (int)Skeleton.Joint.Count; ++i) {
			Skeleton.Joint start = (Skeleton.Joint)i;
			Skeleton.Joint end = Skeleton.GetSkeletonJointParent(start);
			if (start != end) {
				GameObject start_object = joints[(int)start];
				GameObject end_object = joints[(int)end];
				HandCylinder cylindre = cylindres[indexCylindre].GetComponent<HandCylinder>();

				if (start_object.renderer.enabled && end_object.renderer.enabled) {
					cylindre.DefinirExtremites(joints[(int)start].transform.position, joints[(int)end].transform.position);
					cylindre.gameObject.SetActive(true);
				} else {
					cylindre.gameObject.SetActive(false);
				}
				++indexCylindre;
			}
		}
	}
}
