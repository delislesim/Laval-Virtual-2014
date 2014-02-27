/* Sam Cox - 2009 - FrictionPointStudios.com 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyBezier {

    List<Vector3> vectorArray = new List<Vector3>();

	private const float kResolution = 5.0f;

    public MyBezier(FlythoughWaypoint[] waypointsArray)
    {
		vectorArray.Add (waypointsArray [0].CurrentPosition);

		for (int i = 1; i < waypointsArray.Length; ++i) {
			FlythoughWaypoint from = waypointsArray[i - 1];
			FlythoughWaypoint to = waypointsArray[i];
			Vector3 direction = to.CurrentPosition - from.CurrentPosition;
			float norme = direction.magnitude;

			int nbPas = (int) (norme / kResolution);
			if (nbPas < 1) 
				nbPas = 1;

			float fractionParPas = 1.0f / (float) nbPas;

			for (int j = 0; j < nbPas; ++j) {
				vectorArray.Add(Vector3.Lerp(from.CurrentPosition,
				                             to.CurrentPosition,
				                             fractionParPas * j));
			}

		}

		// Ajouter le dernier point 3 fois pour les besoins des calculs de courbe bezier.
		for (int i = 0; i < 3; ++i) {
			vectorArray.Add(waypointsArray[waypointsArray.Length - 1].CurrentPosition);
		}
    }

    public Vector3 GetPointAtTime(float t)
    {
        return CreateBenzierForPoint(t);
    }

    private Vector3 CreateBenzierForPoint(float t)
    {
        int x = (int) (t * (float)vectorArray.Count);
		if (x > vectorArray.Count - 3) {
			x = vectorArray.Count - 3;
		}

        // This is based off http://homepage.mac.com/nephilim/sw3ddev/bezier.html
        float newT = (t * (float)vectorArray.Count) - (float)x;

        Vector3 prevl = vectorArray[CheckWithinArray(x, vectorArray.Count)];
        Vector3 thisl = vectorArray[CheckWithinArray(x + 1, vectorArray.Count)];
        Vector3 nextl = vectorArray[CheckWithinArray(x + 2, vectorArray.Count)];
        Vector3 farl = vectorArray[CheckWithinArray(x + 3, vectorArray.Count)];

        Vector3 delta1 = (nextl - prevl) * .166f;
        Vector3 delta2 = (farl - thisl) * .166f;

        return DoBenzierForNPoints(newT, new Vector3[] { thisl, thisl + delta1, nextl - delta2, nextl });

    }

    private int CheckWithinArray(int x, int c)
    {
        if (x >= c)
        {
            return x % c;
        }
        else
        {
            return x;
        }
    }

    private Vector3 DoBenzierForNPoints(float t, Vector3[] currentArray)
    {
        Vector3 returnVector = Vector3.zero;
        
        float oneMinusT = (1f - t);

        int n = currentArray.Length - 1;

        for (int i = 0; i <= n; i++)
        {
            returnVector += BinomialCoefficient(n, i) * Mathf.Pow(oneMinusT, n - i) * Mathf.Pow(t, i) * currentArray[i];
        }

        return returnVector;
    }

    #region Standard Maths methods

    private float BinomialCoefficient(int n, int k)
    {
        if ((k < 0) || (k > n)) return 0;
        k = (k > n / 2) ? n - k : k;
        return (float) FallingPower(n, k) / Factorial(k);
    }

    private int Factorial(int n)
    {
        if (n == 0) return 1;
        int t = n;
        while (n-- > 2) 
            t *= n;
        return t;
    }

    private int FallingPower(int n, int p)
    {
        int t = 1;
        for (int i = 0; i < p; i++) t *= n--;
        return t;
    }

    #endregion

}
