using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubesTombants : MonoBehaviour {

	public GameObject cubeTombantPrefab;

	// Use this for initialization
	void Start () {
		positionDepart = new Vector3 (transform.localPosition.x,
		                              transform.localPosition.y,
		                              transform.localPosition.z);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ViderCubes() {
		while (cubes.Count != 0) {
			GameObject cube = cubes.Peek();
			cubes.Dequeue();
			Destroy(cube);
		}
	}

	public void AssignerTempsCourant(float tempsCourant) {
		transform.localPosition = new Vector3 (positionDepart.x,
		                                       positionDepart.y - tempsCourant,
		                                       positionDepart.z);

		// Faire le ménage.
		while (cubes.Count != 0) {
			GameObject cube = cubes.Peek();
			float maxY = cube.transform.localPosition.y + cube.transform.localScale.y / 2.0f;
			if (maxY < tempsCourant) {
				cubes.Dequeue();
				Destroy(cube);
			} else {
				break;
			}
		}
	}

	public void AssignerInstrument(Instrument instrument) {
		this.instrument = instrument;
	}

	public void AjouterCube(int indexNote, float debut, float duree) {
		// Creer le nouveau cube.
		GameObject cubeTombant = (GameObject)Instantiate (cubeTombantPrefab);
		cubeTombant.transform.parent = this.transform;

		// Obtenir la position et la taille de la note a jouer.
		float positionHorizontale;
		float largeur;
		instrument.ObtenirInfoNotePourCubesTombants (indexNote, out positionHorizontale, out largeur);

		// Placer le cube tombant au bon endroit et a la bonne taille.
		cubeTombant.transform.localPosition = new Vector3 (positionHorizontale,
		                                                   debut + (duree / 2.0f),
		                                                   0);
		cubeTombant.transform.localScale = new Vector3 (largeur, duree, 1.0f);
		cubeTombant.transform.localRotation = Quaternion.identity;

		cubes.Enqueue (cubeTombant);
	}

	// Instrument pour lequel sont affiches les cubes.
	private Instrument instrument;

	// Position de depart.
	private Vector3 positionDepart;

	// Queue contenant les cubes de la partition.
	private Queue<GameObject> cubes = new Queue<GameObject>();
}
