#include "kinect_wrapper/kinect_sensor.h"

#include <iostream>

#include "base/logging.h"
#include "base/timer.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

KinectSensor* KinectSensor::instance_ = NULL;

// Constructeur.
KinectSensor::KinectSensor() :
    m_pKinectSensor(NULL),
    m_pCoordinateMapper(NULL),
    m_pBodyFrameReader(NULL),
    tracked_body_(-1),
    last_body_index_(1) {
}

// Destructeur.
KinectSensor::~KinectSensor() {
  Shutdown();
}

// Demarrer le thread qui capture les donnees.
void KinectSensor::StartSensorThread() {
  Shutdown();

  // Creer un evement permettant de fermer le thread.
  close_event_.Set(CreateEventW(nullptr, TRUE, FALSE, nullptr));

  // Initialiser la Kinect.
  InitializeDefaultSensor();

  // Creer le thread.
  thread_handle_.Set(CreateThread(nullptr, 0,
                                  (LPTHREAD_START_ROUTINE)KinectSensor::SensorThread,
                                  this, 0, nullptr));
}

// Fermer le thread de capture de donnees.
void KinectSensor::Shutdown() {
  if (thread_handle_.get() == INVALID_HANDLE_VALUE)
    return;

  // Envoyer un evenement pour demander au thread de se fermer.
  ::SetEvent(close_event_.get());

  // Attendre que le thread se ferme.
  ::WaitForSingleObject(thread_handle_.get(), INFINITE);

  // Fermer le thread et son evenement de fermeture.
  thread_handle_.Close();
  close_event_.Close();

  // Se desinscrire de l'evenement de nouveaux squelettes.
  IBodyFrameSource* pBodyFrameSource = NULL;
  HRESULT hr = m_pKinectSensor->get_BodyFrameSource(&pBodyFrameSource);
  if (SUCCEEDED(hr)) {
    hr = pBodyFrameSource->UnsubscribeFrameCaptured(new_frame_event_);
  }
  SafeRelease(pBodyFrameSource);

  // Fermer ce qui a trait a la Kinect.
  SafeRelease(m_pBodyFrameReader);
  SafeRelease(m_pCoordinateMapper);
  if (m_pKinectSensor) {
    m_pKinectSensor->Close();
  }
  SafeRelease(m_pKinectSensor);

  // On ne track plus de squelette.
  tracked_body_ = -1;
  last_body_index_ = 1;
  for (int i = 0; i < kNumSavedBodies; ++i) {
    bodies_[i].tracked = false;
  }
}

/// <summary>
/// Initializes the default Kinect sensor
/// </summary>
/// <returns>indicates success or failure</returns>
HRESULT KinectSensor::InitializeDefaultSensor() {
  HRESULT hr;

  hr = GetDefaultKinectSensor(&m_pKinectSensor);
  if (FAILED(hr)) {
    return hr;
  }

  if (m_pKinectSensor) {
    // Initialize the Kinect and get coordinate mapper and the body reader
    IBodyFrameSource* pBodyFrameSource = NULL;

    hr = m_pKinectSensor->Open();

    if (SUCCEEDED(hr)) {
      hr = m_pKinectSensor->get_CoordinateMapper(&m_pCoordinateMapper);
    }

    if (SUCCEEDED(hr)) {
      hr = m_pKinectSensor->get_BodyFrameSource(&pBodyFrameSource);
    }

    if (SUCCEEDED(hr)) {
      hr = pBodyFrameSource->OpenReader(&m_pBodyFrameReader);
    }

    if (SUCCEEDED(hr)) {
      hr = pBodyFrameSource->SubscribeFrameCaptured(&new_frame_event_);
    }

    SafeRelease(pBodyFrameSource);
  }

  if (!m_pKinectSensor || FAILED(hr)) {
    return E_FAIL;
  }

  return hr;
}

void KinectSensor::ReceiveBodies() {
  IBodyFrame* pBodyFrame = NULL;

  HRESULT hr = m_pBodyFrameReader->AcquireLatestFrame(&pBodyFrame);

  if (SUCCEEDED(hr)) {
    INT64 nTime = 0;

    hr = pBodyFrame->get_RelativeTime(&nTime);

    IBody* ppBodies[BODY_COUNT] = { 0 };

    if (SUCCEEDED(hr)) {
      hr = pBodyFrame->GetAndRefreshBodyData(_countof(ppBodies), ppBodies);
    }

    if (SUCCEEDED(hr)) {
      ProcessBodies(nTime, BODY_COUNT, ppBodies);
    }

    for (int i = 0; i < _countof(ppBodies); ++i) {
      SafeRelease(ppBodies[i]);
    }
  }

  SafeRelease(pBodyFrame);
}

void KinectSensor::ProcessBodies(INT64 nTime, int nBodyCount, IBody** ppBodies) {
  int next_body_index = (last_body_index_ + 1) % kNumSavedBodies;

  // Choisir le squelette le plus prometteur.
  if (!ChooseBody(nTime, nBodyCount, ppBodies)) {
    bodies_[next_body_index].tracked = false;
    bodies_[next_body_index].polled = false;
    last_body_index_ = next_body_index;
    return;
  }

  // Enregistrer les parametres du squelette le plus prometteur.
  IBody* pBody = ppBodies[tracked_body_];
  Joint joints[JointType_Count];
  JointOrientation joint_orientations[JointType_Count];
  HRESULT hr = pBody->GetJoints(_countof(joints), joints);
  if (SUCCEEDED(hr)) {
    pBody->GetJointOrientations(_countof(joint_orientations), joint_orientations);
  }
  if (!SUCCEEDED(hr)) {
    return;
  }

  for (int i = 0; i < JointType_Count; ++i) {
    bodies_[next_body_index].positions[i] = cv::Vec3f(joints[i].Position.X,
                                                      joints[i].Position.Y,
                                                      joints[i].Position.Z);
    bodies_[next_body_index].orientations[i] = cv::Vec4f(joint_orientations[i].Orientation.x,
                                                         joint_orientations[i].Orientation.y,
                                                         joint_orientations[i].Orientation.z,
                                                         joint_orientations[i].Orientation.w);
    bodies_[next_body_index].tracking_state[i] = joints[i].TrackingState;
  }
  
  bodies_[next_body_index].tracked = true;
  bodies_[next_body_index].polled = false;

  last_body_index_ = next_body_index;
}

bool KinectSensor::ChooseBody(INT64 nTime, int nBodyCount, IBody** ppBodies) {
  if (tracked_body_ != -1 && IsTracked(tracked_body_, nBodyCount, ppBodies)) {
    return true;
  }

  // On choisit juste le premier squelette pour l'instant.
  // TODO(fdoray)
  for (int i = 0; i < nBodyCount; ++i) {
    if (IsTracked(i, nBodyCount, ppBodies)) {
      tracked_body_ = i;
      return true;
    }
  }

  return false;
}

bool KinectSensor::IsTracked(int index, int nBodyCount, IBody** ppBodies) {
  if (ppBodies[index] == NULL)
    return false;

  BOOLEAN bTracked = false;
  HRESULT hr = ppBodies[index]->get_IsTracked(&bTracked);
  return SUCCEEDED(hr) && bTracked;
}

DWORD KinectSensor::SensorThread(KinectSensor* sensor) {
  assert(sensor != NULL);
  
  // Obtenir le handle qui dit quand une frame est prete.
  HANDLE events[] = {
    sensor->close_event_.get(),
    reinterpret_cast<HANDLE>(sensor->new_frame_event_)
  };
  DWORD nb_events = ARRAYSIZE(events);

  for (;;) {
    DWORD ret = ::WaitForMultipleObjects(nb_events, events,
                                         FALSE, INFINITE);

    if (ret == WAIT_OBJECT_0)  // Thread close event.
      break;

    // On a recu des squelettes!
    sensor->ReceiveBodies();
  }

  return 1;
}

}  // namespace kinect_wrapper