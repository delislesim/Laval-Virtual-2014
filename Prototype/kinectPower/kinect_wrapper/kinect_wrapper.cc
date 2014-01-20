#include "kinect_wrapper/kinect_wrapper.h"

#include <iostream>

#include "base/logging.h"
#include "base/timer.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/utility.h"

#define ENABLE_TIMER

namespace kinect_wrapper {

namespace {

// Called when the Kinect device status changes.
void CALLBACK StatusChangeCallback(
    HRESULT /*hrStatus */,
    const OLECHAR* /* instanceName */,
    const OLECHAR* /* uniqueDeviceName */,
    void* /* pUserData */) {
  // TODO(fdoray): Implement this.
}

}  // namespace

KinectWrapper* KinectWrapper::instance_ = NULL;

KinectWrapper* KinectWrapper::instance() {
  if (instance_ == NULL)
    instance_ = new KinectWrapper();
  return instance_;
}

void KinectWrapper::Release() {
  delete instance_;
  instance_ = NULL;
}

KinectWrapper::KinectWrapper() {
}

KinectWrapper::~KinectWrapper() {
}

void KinectWrapper::Initialize() {
  // Register a callback to be notified when the status of a sensor changes.
  NuiSetDeviceStatusCallback(StatusChangeCallback,
                             reinterpret_cast<void*>(this));

  // Initialize all connected sensors.
  int num_sensors = GetSensorCount();
  DCHECK(num_sensors <= kMaxNumSensors);

  for (int i = 0; i < num_sensors; ++i) {
    std::string error;
    CreateSensorByIndex(i, &error);
  }
}

void KinectWrapper::StartSensorThread(int sensor_index) {
  // TODO(fdoray): Create an event to stop the thread...
  SensorThreadParams* params = new SensorThreadParams;
  params->sensor_index = sensor_index;
  params->wrapper = this;

  sensor_state_[sensor_index].SetCloseEvent(
      CreateEventW(nullptr, TRUE, FALSE, nullptr));

  sensor_state_[sensor_index].SetThread(CreateThread(
      nullptr, 0, (LPTHREAD_START_ROUTINE)KinectWrapper::SensorThread, params,
      0, nullptr));
}

void KinectWrapper::Shutdown() {
  for (int i = 0; i < kMaxNumSensors; ++i)
    sensor_state_[i].SendCloseEvent();

  for (int i = 0; i < kMaxNumSensors; ++i) {
    sensor_state_[i].WaitThreadCloseAndDelete();
    sensor_state_[i].StopRecording();
  }
}

bool KinectWrapper::RecordSensor(int sensor_index,
                                 const std::string& filename) {
  DCHECK(sensor_index < kMaxNumSensors);
  return sensor_state_[sensor_index].StartRecording(filename);
}

bool KinectWrapper::QueryDepth(int sensor_index, cv::Mat* mat) const {
  DCHECK(mat != NULL);
  return sensor_state_[sensor_index].QueryDepth(0, mat);
}

bool KinectWrapper::StartPlaySensor(int sensor_index,
                                    const std::string& filename) {
  sensor_state_[sensor_index].CreateBuffers();
  return sensor_state_[sensor_index].LoadReplayFile(filename);
}
 
bool KinectWrapper::PlayNextFrame(int sensor_index) {
  return sensor_state_[sensor_index].ReplayFrame();
}


bool KinectWrapper::QuerySkeletonFrame(
    int sensor_index, KinectSkeletonFrame* skeleton_frame) const {
  DCHECK(skeleton_frame != NULL);
  return sensor_state_[sensor_index].QuerySkeletonFrame(skeleton_frame);
}

void KinectWrapper::AddObserver(int sensor_index, KinectObserver* observer) {
  sensor_state_[sensor_index].AddObserver(observer);
}

KinectSensor* KinectWrapper::CreateSensorByIndex(int index,
                                                 std::string* error) {
  DCHECK(error != NULL);

  if (sensor_state_[index].GetSensor() != NULL)
    return sensor_state_[index].GetSensor();

  INuiSensor* native_sensor = NULL;
  if (FAILED(NuiCreateSensorByIndex(index, &native_sensor))) {
    *error = "Could not create an instance of INuiSensor.";
    return NULL;
  }

  HRESULT hr = native_sensor->NuiStatus();
  if (hr != S_OK) {
    *error = "Sensor not ready.";
    SafeRelease(native_sensor);
    return NULL;
  }

  hr = native_sensor->NuiInitialize(
      NUI_INITIALIZE_FLAG_USES_DEPTH_AND_PLAYER_INDEX
      | NUI_INITIALIZE_FLAG_USES_SKELETON
      | NUI_INITIALIZE_FLAG_USES_COLOR
      | NUI_INITIALIZE_FLAG_USES_AUDIO);

  if (SUCCEEDED(hr))
    native_sensor->NuiSetForceInfraredEmitterOff(FALSE);

  if (!SUCCEEDED(hr) && hr != E_NUI_DEVICE_IN_USE) {
    *error = "Sensor could not be initialized.";
    SafeRelease(native_sensor);
    return NULL;
  }

  sensor_state_[index].SetSensor(new KinectSensor(native_sensor));
  return sensor_state_[index].GetSensor();
}

int KinectWrapper::GetSensorCount() {
  int nb_sensors = 0;
  NuiGetSensorCount(&nb_sensors);
  return nb_sensors;
}

DWORD KinectWrapper::SensorThread(SensorThreadParams* params) {
  DCHECK(params != NULL);
  DCHECK(params->wrapper != NULL);

  // Retrieve thread parameters.
  KinectWrapper* wrapper = params->wrapper;
  int sensor_index = params->sensor_index;
  delete params;

  std::string error;
  KinectSensor* sensor = wrapper->CreateSensorByIndex(sensor_index, &error);
  if (sensor == NULL) {
    LOG(INFO) << "Error while starting sensor thread: " << error;
    return 0;
  }

  // Start the streams and create the buffers.
  sensor->OpenDepthStream();
  sensor->OpenSkeletonStream();
  wrapper->sensor_state_[sensor_index].CreateBuffers();

  // Wait for ready frames.
  HANDLE events[] = {
    wrapper->sensor_state_[sensor_index].GetCloseEvent(),
    sensor->GetDepthFrameReadyEvent(),
    sensor->GetSkeletonFrameReadyEvent()
  };
  DWORD nb_events = ARRAYSIZE(events);

  for (;;) {
    DWORD ret = ::WaitForMultipleObjects(nb_events, events,
                                         FALSE, INFINITE);

    if (ret == WAIT_OBJECT_0)  // Thread close event.
      break;

#ifdef ENABLE_TIMER
    base::Timer timer;
    timer.Start();
#endif

    // Poll the depth stream.
    bool depth_polled = sensor->PollNextDepthFrame(
        &wrapper->sensor_state_[sensor_index]);

    // Poll the skeleton stream.
    sensor->PollNextSkeletonFrame(&wrapper->sensor_state_[sensor_index]);

    // Record the frame.
    if (depth_polled)
      wrapper->sensor_state_[sensor_index].RecordFrame();

#ifdef ENABLE_TIMER
    std::cout << "Elapsed time: " << timer.ElapsedTime() << " ms." << std::endl;
#endif
  }

  // Release the buffers.
  wrapper->sensor_state_[sensor_index].ReleaseBuffers();

  return 1;
}

} // namespace kinect_wrapper