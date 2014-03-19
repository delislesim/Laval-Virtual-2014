using System;

public class Lerp
{
	public static float LerpFloat (float initial, float final, float vitesse)
	{
		if (initial < final) {
			initial += vitesse;
			if (initial > final)
			    initial = final;
		} else if (initial > final) {
			initial -= vitesse;
			if (initial < final)
				initial = final;
		}
		return initial;
	}
}

