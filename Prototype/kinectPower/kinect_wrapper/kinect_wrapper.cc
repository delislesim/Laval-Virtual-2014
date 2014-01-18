#include "kinect_wrapper/kinect_wrapper.h"

#include <iostream>

#include "base/logging.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

namespace {

const int kMaxNbSensors = 6;

// Called when the Kinect device status changes.
void CALLBACK StatusChangeCallback(
    HRESULT /*hrStatus */,
    const OLECHAR* /* instanceName */,
    const OLECHAR* /* uniqueDeviceName */,
    void* /* pUserData */) {
  // TODO(fdoray): Implement this.
}

}  // namespace

KinectWrapper::KinectWrapper() : sensors_(kMaxNbSensors) {
}

KinectWrapper::~KinectWrapper() {
  for (SensorVector::iterator it = sensors_.begin(); 
       it != sensors_.end(); ++it) {
    delete *it;
  }
}

void KinectWrapper::Initialize() {
  NuiSetDeviceStatusCallback(StatusChangeCallback,
                             reinterpret_cast<void*>(this));
}

KinectSensor* KinectWrapper::CreateSensorByIndex(int index,
                                                 std::string* error) {
  DCHECK(error != NULL);

  if (sensors_[index] != NULL)
    return sensors_[index];

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

  sensors_[index] = new KinectSensor(native_sensor);
  return sensors_[index];
}

int KinectWrapper::GetSensorCount() {
  int nb_sensors = 0;
  NuiGetSensorCount(&nb_sensors);
  return nb_sensors;
}

} // namespace kinect_wrapper