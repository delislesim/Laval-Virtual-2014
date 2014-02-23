using UnityEngine;
using System.Collections;

public class Kalman  {

	public Kalman() {
		// q
		// Matrice de covariance du bruit sur l'état
		// Plus c'est gros, plus ça bouge (moins on fait confiance à l'hypothèse que ça ne bouge pas).
		q.SetRow (0, new Vector4 (1.0f, 0,    0,    0));
		q.SetRow (1, new Vector4 (0,    1.0f, 0,    0));
		q.SetRow (2, new Vector4 (0,    0,    1.0f, 0));
		q.SetRow (3, new Vector4 (0,    0,    0,    1.0f));

		// r
		// Matrice de covariance de l'erreur sur l'observation.
		// Si l'observation shake beaucoup, le mettre gros.
		r.SetRow (0, new Vector4 (1.0f, 0,    0,    0));
		r.SetRow (1, new Vector4 (0,    1.0f, 0,    0));
		r.SetRow (2, new Vector4 (0,    0,    2.0f, 0));
		r.SetRow (3, new Vector4 (0,    0,    0,    1.0f));

		// p est initialement la matrice identité
		p = Matrix4x4.identity;
	}

	public void SetInitialObservation(Vector4 initialObservation) {
		x.x = initialObservation.x;
		x.y = initialObservation.y;
		x.z = initialObservation.z;
		x.w = initialObservation.w;
	}

	public Vector4 Update(Vector4 observation) {
		Vector4 z = observation - x;
		p = AddMatrixes (p, q);
		Matrix4x4 k = p * AddMatrixes (p, r).inverse;
		x = x + k * z;
		p = SubMatrix (Matrix4x4.identity, k) * p;
		return x;
	}

	public Vector4 GetFilteredVector() {
		return new Vector4 (x.x, x.y, x.z);
	}

	private Matrix4x4 AddMatrixes(Matrix4x4 a, Matrix4x4 b) {
		Matrix4x4 res = new Matrix4x4 ();
		for (int i = 0; i < 4; ++i) {
			res.SetRow (i, a.GetRow (i) + b.GetRow (i));
		}
		return res;
	}

	private Matrix4x4 SubMatrix(Matrix4x4 a, Matrix4x4 b) {
		Matrix4x4 res = new Matrix4x4 ();
		for (int i = 0; i < 4; ++i) {
			res.SetRow (i, a.GetRow (i) - b.GetRow (i));
		}
		return res;
	}

	// Position filtree.
	private Vector4 x = new Vector4 ();

	// Matrice de covariance du bruit sur l'estimation.
	// À quel point notre estimation est bonne (auto-estimation de l'erreur).
	private Matrix4x4 p;

	// Constantes.
	private Matrix4x4 q = new Matrix4x4();
	private Matrix4x4 r = new Matrix4x4();

}
