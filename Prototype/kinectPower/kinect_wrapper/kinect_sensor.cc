#include "kinect_wrapper/kinect_sensor.h"

#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

KinectSensor::KinectSensor(INuiSensor* native_sensor)
    : native_sensor_(native_sensor) {
}

KinectSensor::~KinectSensor() {
  SafeRelease(native_sensor_);
}

KinectDepthStream* KinectSensor::GetDepthStream() {
  if (depth_stream_.get() == NULL) {
    depth_stream_.reset(new KinectDepthStream(this));
    depth_stream_->OpenStream();
  }
  
  return depth_stream_.get();
}

bool KinectSensor::ImageStreamOpen(
    /* [in] */ NUI_IMAGE_TYPE eImageType,
    /* [in] */ NUI_IMAGE_RESOLUTION eResolution,
    /* [in] */ DWORD dwImageFrameFlags,
    /* [in] */ DWORD dwFrameLimit,
    /* [in] */ HANDLE hNextFrameEvent,
    /* [out] */ HANDLE *phStreamHandle) {
  HRESULT hr = native_sensor_->NuiImageStreamOpen(
      eImageType, eResolution, dwImageFrameFlags, dwFrameLimit, hNextFrameEvent,
      phStreamHandle);
  return SUCCEEDED(hr);
}

bool KinectSensor::ImageStreamSetImageFrameFlags( 
      /* [in] */ HANDLE hStream,
      /* [in] */ DWORD dwImageFrameFlags) {
  HRESULT hr = native_sensor_->NuiImageStreamSetImageFrameFlags(
      hStream, dwImageFrameFlags);
  return SUCCEEDED(hr);
}

bool KinectSensor::ImageStreamGetNextFrame( 
    /* [in] */ HANDLE hStream,
    /* [in] */ DWORD dwMillisecondsToWait,
    /* [retval][out] */ NUI_IMAGE_FRAME *pImageFrame) {
  HRESULT hr = native_sensor_->NuiImageStreamGetNextFrame(
      hStream, dwMillisecondsToWait, pImageFrame);
  return SUCCEEDED(hr);
}

bool KinectSensor::ImageStreamReleaseFrame( 
    /* [in] */ HANDLE hStream,
    /* [in] */ NUI_IMAGE_FRAME *pImageFrame) {
  HRESULT hr = native_sensor_->NuiImageStreamReleaseFrame(
      hStream, pImageFrame);
  return SUCCEEDED(hr);
}

}  // namespace kinect_wrapper