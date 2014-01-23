INSTRUCTIONS POUR UTILISER KINECT POWER
========================================================

A. PREREQUIS
--------------------------------------------------------
1. Installer le SDK officiel de la Kinect, Unity et Visual Studio (2010-2013).
2. Installer OpenCV. KinectPower a ete teste avec OpenCV 2.4.8.
    2.1 Telecharger "OpenCV for Windows" a l'adresse http://opencv.org/downloads.html.
    2.2 Ajouter une variable d'environnement nommee "OPENCV_DIR" et contenant le chemin
        complet vers le dossier "build" de l'installation d'OpenCV.
3. Installer CMake.
    3.1 Telecharger CMake a l'adresse http://www.cmake.org/cmake/resources/software.html
        La version "Windows (Win32 Installer)" est la plus simple a installer.
    3.2 Suivre les installations d'instruction.
    3.3 S'assurer que le dossier "bin" de CMake est ajoute au PATH.

B. COMPILATION DE KINECT POWER
--------------------------------------------------------
1. Aller dans le dossier de kinectPower et lancer la commande:
    cmake -G "Visual Studio X"
	---> ou X est remplace par:
		10: pour Visual Studio 2010
		11: pour Visual Studio 2012
		12: pour Visual Studio 2013
2. CMake suggere d'ajouter un dossier au PATH. L'ajouter commme demande.
2. Ouvrir le projet Visual Studio qui est genere et compiler!
3. Une DLL nommee "kinect_power.dll" est alors generee dans le dossier
   "Debug" ou "Release" de kinectPower. Pour que cette DLL puisse etre
   utilisee par Unity, il faut ajouter son path complet a la variable
   d'environnement PATH de Windows.

C. EXECUTION DE KINECTPOWERUNITY
--------------------------------------------------------
1. Ouvrir le projet kinectPowerUnity.
2. Jouer avec les options du GameObject "KinectPower".
3. Cliquer sur Play!

D. ENREGISTRER LES DONNEES DE LA KINECT
--------------------------------------------------------
1. Dans les options du GameObject "KinectPower", selectionner
   "RECORD" pour le champ "Replay".
2. Indiquer le nom du fichier dans lequel sauvegarder les donnees.
   Il sera enregistre dans le meme dossier que le projet Unity.
3. Demarrer la scene, toutes les donnees de la Kinect seront enregistrees!

E. REJOUER DES DONNEES ENREGISTREES PAR LA KINECT
--------------------------------------------------------
1. Dans les options du GameObject "KinectPower", selectionner
   "REPLAY" pour le champ "Replay".
2. Indiquer le nom du fichier duquel charger les donnees.
   Le chemin est relatif au dossier de projet Unity.
3. Demarrer la scene, les donnees seront retransmises en boucle!