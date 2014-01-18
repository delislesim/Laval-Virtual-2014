#include "kinect_wrapper/kinect_sensor.h"

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

namespace {

const NUI_IMAGE_TYPE kDepthImageType = NUI_IMAGE_TYPE_DEPTH_AND_PLAYER_INDEX;
const NUI_IMAGE_RESOLUTION kDepthImageResolution = NUI_IMAGE_RESOLUTION_640x480;

}  // namespace

KinectSensor::KinectSensor(INuiSensor* native_sensor)
    : native_sensor_(native_sensor),
      near_mode_enabled_(false),
      depth_stream_opened_(false),
      depth_frame_ready_event_(INVALID_HANDLE_VALUE),
      depth_stream_handle_(INVALID_HANDLE_VALUE),
      depth_stream_width_(0),
      depth_stream_height_(0) {
  depth_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
}

bool KinectSensor::OpenDepthStream() {
  if (depth_stream_opened_)
    return true;

  HRESULT res = native_sensor_->NuiImageStreamOpen(
      kDepthImageType, kDepthImageResolution, 0, 2, depth_frame_ready_event_,
      &depth_stream_handle_);

  if (FAILED(res))
    return false;

  res = native_sensor_->NuiImageStreamSetImageFrameFlags(
      &depth_stream_handle_,
      near_mode_enabled_ ? NUI_IMAGE_STREAM_FLAG_ENABLE_NEAR_MODE : 0);
  
  if (FAILED(res)) {
    // TODO(fdoray): Release the depth stream?
    depth_stream_handle_ = INVALID_HANDLE_VALUE;
    return false;
  }

  DWORD width = 0;
  DWORD height = 0;
  ::NuiImageResolutionToSize(kDepthImageResolution, width, height);
  depth_stream_width_ = width;
  depth_stream_height_ = height;

  depth_stream_opened_ = true;
  return true;
}

bool KinectSensor::PollNextDepthFrame(KinectBuffer* buffer) {
  DCHECK(buffer);
  DCHECK(depth_stream_opened_);
  DCHECK(depth_stream_handle_ != INVALID_HANDLE_VALUE);

  if (WaitForSingleObject(depth_frame_ready_event_, 0) != WAIT_OBJECT_0)
    return false;

  bool res = true;

  NUI_IMAGE_FRAME image_frame;
  HRESULT hr = native_sensor_->NuiImageStreamGetNextFrame(
      depth_stream_handle_, 0, &image_frame);
  if (FAILED(hr)) {
    res = false;
    goto ReleaseFrame;
  }

  NUI_LOCKED_RECT locked_rect;

  // Lock the frame data so the Kinect knows not to modify it while we're
  // reading it.
  image_frame.pFrameTexture->LockRect(0, &locked_rect, NULL, 0);

  if (locked_rect.Pitch == 0) {
    res = false;
    goto ReleaseFrame;
  }

  buffer->CopyData(reinterpret_cast<const char*>(locked_rect.pBits),
                   locked_rect.size, depth_stream_width_, depth_stream_height_,
                   kKinectDepthBytesPerPixel);

  image_frame.pFrameTexture->UnlockRect(0);

ReleaseFrame:
  native_sensor_->NuiImageStreamReleaseFrame(depth_stream_handle_, &image_frame);
  return res;
}

KinectSensor::~KinectSensor() {
  // TODO(fdoray): Release the depth stream?
  native_sensor_->NuiShutdown();
  SafeRelease(native_sensor_);
}

}  // namespace kinect_wrapper