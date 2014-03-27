#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "base/scoped_handle.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_skeleton.h"

namespace kinect_wrapper {  

namespace {
}  // namespace

// Represente le capteur Kinect. Est un singleton.
class KinectSensor {
 public:
  static KinectSensor* Instance() {
    if (instance_ == NULL) {
      instance_ = new KinectSensor();
    }
    return instance_;
  }

  // Demarrer le thread qui capture les donnees.
  void StartSensorThread();

  // Fermer le thread de capture de donnees.
  void Shutdown();

  // Obtenir le squelette le plus recent.
  KinectSkeleton* GetLastBody() {
    return &bodies_[last_body_index_];
  }

  ICoordinateMapper* GetCoordinateMapper() {
    return m_pCoordinateMapper;
  }

 private:
  // Constructeur et destructeur prives.
  KinectSensor();
  ~KinectSensor();

  /// <summary>
  /// Initializes the default Kinect sensor
  /// </summary>
  /// <returns>S_OK on success, otherwise failure code</returns>
  HRESULT InitializeDefaultSensor();

  // Fonction du thread qui capture les donnees.
  static DWORD KinectSensor::SensorThread(KinectSensor* sensor);

  // Va chercher les squelettes recus de la Kinect.
  void ReceiveBodies();

  // Enregistre les squelettes fournis en parametre.
  void ProcessBodies(INT64 nTime, int nBodyCount, IBody** ppBodies);

  // Choisit le squelette le plus prometteur, si aucun squelette n'est
  // presentement choisi.
  bool ChooseBody(INT64 nTime, int nBodyCount, IBody** ppBodies);

  // Indique si le squelette dont l'index est passe en parametre est valide.
  bool IsTracked(int index, int nBodyCount, IBody** ppBodies);

  // Unique instance de cette classe.
  static KinectSensor* instance_;

  // Thread de capture de donnees.
  base::ScopedHandle thread_handle_;

  // Evenement de fermeture du thread de capture de donnees.
  base::ScopedHandle close_event_;

  // Evenement d'arrive de nouvelle frame.
  WAITABLE_HANDLE new_frame_event_;

  // Current Kinect
  IKinectSensor*          m_pKinectSensor;
  ICoordinateMapper*      m_pCoordinateMapper;

  // Body reader
  IBodyFrameReader*       m_pBodyFrameReader;

  // Index du squelette qui nous interesse. -1 si aucun squelette n'est choisi.
  int tracked_body_;

  // Index du dernier squelette enregistre.
  int last_body_index_;

  // Squelettes enregistres.
  KinectSkeleton bodies_[kNumSavedBodies];

  DISALLOW_COPY_AND_ASSIGN(KinectSensor);
};

}  // namespace kinect_wrapper