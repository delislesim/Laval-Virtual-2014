using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubesTombantsGuitare : MonoBehaviour {

	public GameObject cubeTombantPrefab;

	// Use this for initialization
	void Start () {
		positionDepart = new Vector3 (transform.localPosition.x,
		                              transform.localPosition.y,
		                              transform.localPosition.z);	
	}

	public void ChargerPartition(List<PartitionGuitar.Playable> partition) {
		ViderCubes ();

		for (int i = 0; i < partition.Count; ++i) {
			AjouterCube(partition[i].time, partition[i].positionManche);
		}
	}

	public void AssignerTempsCourant(float tempsCourant) {
		transform.localPosition = new Vector3 (positionDepart.x - tempsCourant * kEspaceEntreCubes,
		                                       positionDepart.y,
		                                       positionDepart.z);

		// Faire le ménage.
		while (cubes.Count != 0) {
			GameObject cube = cubes.Peek();
			float maxY = cube.transform.localPosition.x / kEspaceEntreCubes;
			if (maxY < tempsCourant) {
				cubes.Dequeue();
				Destroy(cube);
			} else {
				break;
			}
		}
	}

	public void ViderCubes() {
		while (cubes.Count != 0) {
			GameObject cube = cubes.Peek();
			cubes.Dequeue();
			Destroy(cube);
		}
	}

	private void AjouterCube(float temps, PartitionGuitar.PositionManche positionManche) {
		GameObject cubeTombant = (GameObject)Instantiate (cubeTombantPrefab);
		cubeTombant.transform.parent = this.transform;

		Vector3 localPosition = new Vector3 (temps * kEspaceEntreCubes, 0, 0);
		cubeTombant.transform.localRotation = Quaternion.identity;
		cubeTombant.transform.localScale = new Vector3 (1.0f, 0.5f, 1.0f);

		if (positionManche == PartitionGuitar.PositionManche.LOIN) {
			localPosition.y = -0.25f;
		} else {
			localPosition.y = 0.25f;
		}
		cubeTombant.transform.localPosition = localPosition;
		
		cubes.Enqueue (cubeTombant);
	}

	// Position de depart.
	private Vector3 positionDepart;

	// Queue contenant les cubes de la partition.
	private Queue<GameObject> cubes = new Queue<GameObject>();

	// Espace entre les cubes tombants.
	private const float kEspaceEntreCubes = 20.0f;
}
