#include "kinect_wrapper/kinect_wrapper.h"

#include <iostream>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

namespace {

const int kMaxNumSensors = 6;

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

KinectWrapper::KinectWrapper()
    : sensor_info_(kMaxNumSensors) {
  for (int i = 0; i < kMaxNumSensors; ++i) {
    sensor_info_[i].sensor = NULL;
    sensor_info_[i].depth_buffer = NULL;
    sensor_info_[i].current_skeleton_buffer = 0;
    sensor_info_[i].thread = INVALID_HANDLE_VALUE;
    sensor_info_[i].close_event = INVALID_HANDLE_VALUE;
  }
}

KinectWrapper::~KinectWrapper() {
  for (SensorInfoVector::iterator it = sensor_info_.begin();
       it != sensor_info_.end(); ++it) {
    delete it->sensor;
    delete it->depth_buffer;
    DCHECK(it->thread == INVALID_HANDLE_VALUE);
    DCHECK(it->close_event == INVALID_HANDLE_VALUE);
  }
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

  sensor_info_[sensor_index].close_event =
      CreateEventW(nullptr, TRUE, FALSE, nullptr);

  sensor_info_[sensor_index].thread = CreateThread(
      nullptr, 0, (LPTHREAD_START_ROUTINE)KinectWrapper::SensorThread, params,
      0, nullptr);
}

void KinectWrapper::Shutdown() {
  for (SensorInfoVector::iterator it = sensor_info_.begin();
       it != sensor_info_.end(); ++it) {
    if (it->close_event != INVALID_HANDLE_VALUE)
      ::SetEvent(it->close_event);
  }

  for (SensorInfoVector::iterator it = sensor_info_.begin();
       it != sensor_info_.end(); ++it) {
    if (it->thread != INVALID_HANDLE_VALUE) {
      ::WaitForSingleObject(it->thread, INFINITE);
      ::CloseHandle(it->thread);
      it->thread = INVALID_HANDLE_VALUE;
      ::CloseHandle(it->close_event);
      it->close_event = INVALID_HANDLE_VALUE;

      DCHECK(it->depth_buffer == NULL);
    }
  }
}

bool KinectWrapper::QueryDepth(int sensor_index, cv::Mat* mat) {
  DCHECK(mat != NULL);

  if (sensor_info_[sensor_index].depth_buffer == NULL)
    return false;

  KinectBuffer* buffer = sensor_info_[sensor_index].depth_buffer;
  buffer->GetDepthMat(mat);

  return true;
}

bool KinectWrapper::QuerySkeleton(int sensor_index, KinectSkeleton* skeleton) {
  DCHECK(skeleton != NULL);

  if (sensor_info_[sensor_index].skeleton_buffer == NULL)
    return false;

  size_t current_buffer_index =
      sensor_info_[sensor_index].current_skeleton_buffer;
  *skeleton = sensor_info_[sensor_index].skeleton_buffer[current_buffer_index];

  return true;
}

KinectSensor* KinectWrapper::CreateSensorByIndex(int index,
                                                 std::string* error) {
  DCHECK(error != NULL);

  if (sensor_info_[index].sensor != NULL)
    return sensor_info_[index].sensor;

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

  sensor_info_[index].sensor = new KinectSensor(native_sensor);
  return sensor_info_[index].sensor;
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
  wrapper->sensor_info_[sensor_index].depth_buffer = new KinectBuffer(
      sensor->depth_stream_width(), sensor->depth_stream_height(),
      kKinectDepthBytesPerPixel);

  sensor->OpenSkeletonStream();
  wrapper->sensor_info_[sensor_index].skeleton_buffer = new KinectSkeleton[2];

  // Wait for ready frames.
  HANDLE events[] = {
    wrapper->sensor_info_[sensor_index].close_event,
    sensor->GetDepthFrameReadyEvent(),
    sensor->GetSkeletonFrameReadyEvent()
  };
  DWORD nb_events = ARRAYSIZE(events);

  while (true) {
    DWORD ret = ::WaitForMultipleObjects(nb_events, events,
                                         FALSE, INFINITE);

    if (ret == WAIT_OBJECT_0)  // Thread close event.
      break;

    // Poll the depth stream.
    sensor->PollNextDepthFrame(
        wrapper->sensor_info_[sensor_index].depth_buffer);

    // Poll the skeleton stream.
    size_t current_skeleton_buffer =
        wrapper->sensor_info_[sensor_index].current_skeleton_buffer;
    if (sensor->PollNextSkeletonFrame(
            &wrapper->sensor_info_[sensor_index].skeleton_buffer[
                current_skeleton_buffer])) {
      wrapper->sensor_info_[sensor_index].current_skeleton_buffer =
        (current_skeleton_buffer + 1) % kNumBuffers;
    }
  }

  // Free memory.
  delete wrapper->sensor_info_[sensor_index].depth_buffer;
  wrapper->sensor_info_[sensor_index].depth_buffer = NULL;

  delete[] wrapper->sensor_info_[sensor_index].skeleton_buffer;

  return 1;
}

} // namespace kinect_wrapper