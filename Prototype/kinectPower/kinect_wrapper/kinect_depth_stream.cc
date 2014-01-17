#include "kinect_wrapper/kinect_depth_stream.h"

#include "base/logging.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_sensor.h"

namespace kinect_wrapper {

namespace {

const NUI_IMAGE_TYPE kDepthImageType = NUI_IMAGE_TYPE_DEPTH_AND_PLAYER_INDEX;
const NUI_IMAGE_RESOLUTION kDepthImageResolution = NUI_IMAGE_RESOLUTION_640x480;
const size_t kDepthBytesPerPixel = 2;

}  // namespace

KinectDepthStream::KinectDepthStream(KinectSensor* sensor)
    : KinectStream(sensor),
      opened_(false),
      stream_handle_(INVALID_HANDLE_VALUE),
      near_mode_enabled_(false) {
}

KinectDepthStream::~KinectDepthStream() {
}

bool KinectDepthStream::OpenStream() {
  DCHECK(!opened_);

  // Open depth stream.
  bool res = GetSensor()->ImageStreamOpen(
      kDepthImageType, kDepthImageResolution, 0, 2, GetFrameReadyEvent(),
      &stream_handle_);
  if (!res)
    return false;

  GetSensor()->ImageStreamSetImageFrameFlags(
      &stream_handle_,
      near_mode_enabled_ ? NUI_IMAGE_STREAM_FLAG_ENABLE_NEAR_MODE : 0);
  
  DWORD width = 0;
  DWORD height = 0;
  ::NuiImageResolutionToSize(kDepthImageResolution, width, height);
  stream_width_ = width;
  stream_height_ = height;
  
  opened_ = true;
  return true;
}

bool KinectDepthStream::PollNextFrame(KinectBuffer* buffer) {
  DCHECK(buffer);

  if (WaitForSingleObject(GetFrameReadyEvent(), 0) != WAIT_OBJECT_0)
    return false;

  bool res = true;

  NUI_IMAGE_FRAME image_frame;
  HRESULT hr = GetSensor()->ImageStreamGetNextFrame(
      stream_handle_, 0, &image_frame);
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
                   locked_rect.size, stream_width_, stream_height_,
                   kDepthBytesPerPixel);

  image_frame.pFrameTexture->UnlockRect(0);

ReleaseFrame:
  GetSensor()->ImageStreamReleaseFrame(stream_handle_, &image_frame);
  return res;
}

}  // namespace kinect_wrapper