using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubesTombantsGuitare : MonoBehaviour {

	public GameObject cubeTombantPrefab;
	public GameObject soloTombantPrefab;

	// Use this for initialization
	void Start () {
		positionDepart = new Vector3 (transform.localPosition.x,
		                              transform.localPosition.y,
		                              transform.localPosition.z);	
	}

	public void ChargerPartition(List<PartitionGuitar.Playable> partition) {
		ViderCubes ();

		float tempsDebutSolo = 0.0f;

		for (int i = 0; i < partition.Count; ++i) {
			if (partition[i].solo == PartitionGuitar.Solo.DEBUT) {
				tempsDebutSolo = partition[i].time;
			} else if (partition[i].solo == PartitionGuitar.Solo.FIN) {
				float tempsFinSolo = partition[i].time;
				AjouterSolo(tempsDebutSolo, tempsFinSolo);
			} else {
				AjouterCube(partition[i].time, partition[i].positionManche);
			}
		}
	}

	public void AssignerTempsCourant(float tempsCourant) {
		transform.localPosition = new Vector3 (positionDepart.x - tempsCourant * kEspaceEntreCubes,
		                                       positionDepart.y,
		                                       positionDepart.z);

		// Faire le ménage des cubes.
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

		// Faire le ménage des solos.
		for (int i = 0; i < solos.Count; ++i) {
			LineWaveGuitar solo = solos[i];

			float minY = solo.transform.localPosition.x / kEspaceEntreCubes;
			float maxY = minY + solo.GetLength() / kEspaceEntreCubes;

			if (minY > tempsCourant - 10.0f && maxY < tempsCourant + 10.0f) {
				solo.gameObject.SetActive(true);
			} else {
				solo.gameObject.SetActive(false);
			}
		}
	}
	
	public void ViderCubes() {
		while (cubes.Count != 0) {
			GameObject cube = cubes.Peek();
			cubes.Dequeue();
			Destroy(cube);
		}
		for (int i = 0; i < solos.Count; ++i) {
			Destroy(solos[i]);
		}
		solos.Clear ();
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

	private void AjouterSolo(float tempsDebut, float tempsFin) {
		const float kDureeEnsembleMax = 5.0f;
		float dureeTotale = tempsFin - tempsDebut;
		int numEnsembles = 1 + (int)(dureeTotale / kDureeEnsembleMax);
		float dureeEnsemble = dureeTotale / numEnsembles;

		for (int i = 0; i < numEnsembles; ++i) {
			GameObject solo = (GameObject)Instantiate (soloTombantPrefab);
			solo.transform.parent = this.transform;

			solo.transform.localPosition = new Vector3 ((tempsDebut - 1.0f + dureeEnsemble * i) * kEspaceEntreCubes, 0, 0);
			solo.transform.localRotation = Quaternion.identity;
			solo.transform.localScale = Vector3.one;

			LineWaveGuitar lineWave = solo.GetComponent<LineWaveGuitar> ();
			lineWave.SetLength ((dureeEnsemble + 2.0f) * kEspaceEntreCubes);

			solo.SetActive(false);
			solos.Add (lineWave);
		}
	}

	// Position de depart.
	private Vector3 positionDepart;

	// Queue contenant les cubes de la partition.
	private Queue<GameObject> cubes = new Queue<GameObject>();

	// Queue contenant les solos de la partition.
	private List<LineWaveGuitar> solos = new List<LineWaveGuitar> ();

	// Espace entre les cubes tombants.
	private const float kEspaceEntreCubes = 20.0f;
}
