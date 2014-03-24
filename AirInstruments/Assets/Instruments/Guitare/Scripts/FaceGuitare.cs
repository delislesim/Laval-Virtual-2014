using UnityEngine;
using System.Collections;
using KinectHelpers;

public class FaceGuitare : MonoBehaviour {

	// Use this for initialization
	void Start () {
		streamTexture = new Texture2D(kTextureWidth, kTextureHeight);

		streamTexture.SetPixels32(streamColors);
		renderer.material.mainTexture = streamTexture;
	}
	
	// Update is called once per frame
	void Update () {
		timerUpdate += Time.deltaTime;
		if (timerUpdate >= kTempsRafraichissement) {
			timerUpdate %= kTempsRafraichissement;

			// Obtenir l'image de couleur.
			KinectPowerInterop.GetColorImage(streamBuffer, (uint)streamBuffer.Length);

			// Obtenir la position de la tete.
			int[] positionTete = new int[2];
			KinectPowerInterop.GetHeadPositionColor(0, positionTete);

			// Copier la partie de l'image qui correspond a la tete.
			int xMin = positionTete[0] - (kTextureWidth/2);
			int yMin = positionTete[1] - (kTextureHeight/2);;

			int xTarget = 0;
			int yTarget = 0;

			for (int j = yMin; j < yMin + kTextureHeight; ++j) {
				for (int i = xMin; i < xMin + kTextureWidth; ++i) {
					if (i >= 0 && j >= 0 && i < kStreamWidth && j < kStreamHeight) {
						int sourceIndex = j*kStreamWidth + i;

						int redIndex = sourceIndex*4 + 2;
						int greenIndex = sourceIndex*4 + 1;
						int blueIndex = sourceIndex*4 + 0;
						int alphaIndex = sourceIndex*4 + 3;

						int targetIndex = (kTextureHeight - yTarget - 1)*kTextureWidth + xTarget;

						streamColors[targetIndex].r = streamBuffer[redIndex];
						streamColors[targetIndex].g = streamBuffer[greenIndex];
						streamColors[targetIndex].b = streamBuffer[blueIndex];
						streamColors[targetIndex].a = streamBuffer[alphaIndex];
					}
					++xTarget;
				}
				++yTarget;
				xTarget = 0;
			}

			streamTexture.SetPixels32(streamColors);
			streamTexture.Apply();
		}
	}

	// Timer pour updater l'image.
	private float timerUpdate = 0;

	// Temps avant un rafraichissement.
	private const float kTempsRafraichissement = 0.1f;

	// Largeur de la texture.
	private const int kTextureWidth = 60;

	// Hauteur de la texture.
	private const int kTextureHeight = 60;

	// Buffer pour l'image de couleur.
	private byte[] streamBuffer = new byte[kStreamWidth * kStreamHeight * 4];

	// Couleurs de la texture de visage.
	private Color32[] streamColors = new Color32[kTextureWidth * kTextureHeight];

	// Texture du visage du guitariste.
	private Texture2D streamTexture;

	// Taille de l'image fournie par la Kinect.
	private const int kStreamWidth = 640;
	private const int kStreamHeight = 480;

	// Indique si le face tracker est active.
	private bool estActive = false;

}
