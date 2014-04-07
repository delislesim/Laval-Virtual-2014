using UnityEngine;
using System.Collections;

public class CureMode : MonoBehaviour {

	public ParticleEmitter[] emitters;

	void Start() {
		foreach (ParticleEmitter emitter in emitters) {
			emitter.emit = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (!isOn)
			return;

		compteurStop += Time.deltaTime;
		if (compteurStop >= kTempsStop) {
			foreach (ParticleEmitter emitter in emitters) {
				emitter.emit = false;
			}
			isOn = false;
		}
	}

	public void Burst(Vector3 localPosition) {
		isOn = true;
		compteurStop = 0;
		transform.localPosition = localPosition;

		foreach (ParticleEmitter emitter in emitters) {
			emitter.emit = true;
		}
	}

	private bool isOn = false;

	private float compteurStop = 0;

	private const float kTempsStop = 1.5f;
}
