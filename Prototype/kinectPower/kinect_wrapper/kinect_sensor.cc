#include "kinect_wrapper/kinect_sensor.h"

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor_state.h"
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
      depth_stream_height_(0),
      skeleton_seated_enabled_(false),
      skeleton_stream_opened_(false),
      skeleton_frame_ready_event_(INVALID_HANDLE_VALUE) {
  depth_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
  skeleton_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);

  skeleton_sticky_ids_[0] = 0;
  skeleton_sticky_ids_[1] = 0;

  native_sensor_->NuiGetCoordinateMapper(&coordinate_mapper_);
}

void KinectSensor::SetNearModeEnabled(bool near_mode_enabled) {
  near_mode_enabled_ = near_mode_enabled;
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

bool KinectSensor::PollNextDepthFrame(KinectSensorState* state) {
  assert(state);
  assert(depth_stream_opened_);
  assert(depth_stream_handle_ != INVALID_HANDLE_VALUE);

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

  BOOL near_mode;
  INuiFrameTexture* texture = NULL;

  hr = native_sensor_->NuiImageFrameGetDepthImagePixelFrameTexture(
      depth_stream_handle_, &image_frame, &near_mode, &texture);
  if (FAILED(hr))
    goto ReleaseFrame;

  NUI_LOCKED_RECT locked_rect;

  // Lock the frame data so the Kinect knows not to modify it while we're
  // reading it.
  texture->LockRect(0, &locked_rect, NULL, 0);

  if (locked_rect.Pitch == 0) {
    res = false;
    goto ReleaseFrame;
  }

  const NUI_DEPTH_IMAGE_PIXEL* start =
      reinterpret_cast<const NUI_DEPTH_IMAGE_PIXEL*>(locked_rect.pBits);
  const NUI_DEPTH_IMAGE_PIXEL* end =
      start + depth_stream_width_ * depth_stream_height_;

  state->InsertDepthFrame(start, end);

  image_frame.pFrameTexture->UnlockRect(0);

ReleaseFrame:
  native_sensor_->NuiImageStreamReleaseFrame(depth_stream_handle_, &image_frame);
  return res;
}

bool KinectSensor::OpenSkeletonStream() {
  if (skeleton_stream_opened_)
    return true;

  DWORD flags = (
      (skeleton_seated_enabled_ ?
           NUI_SKELETON_TRACKING_FLAG_ENABLE_SEATED_SUPPORT : 0) |
      (near_mode_enabled_ ?
           NUI_SKELETON_TRACKING_FLAG_ENABLE_IN_NEAR_RANGE : 0)
  );

  HRESULT res = native_sensor_->NuiSkeletonTrackingEnable(
      skeleton_frame_ready_event_, flags);

  if (FAILED(res))
    return false;

  skeleton_stream_opened_ = true;
  return true;
}

bool KinectSensor::PollNextSkeletonFrame(KinectSensorState* state) {
  assert(state);
  assert(skeleton_stream_opened_);

  if (WaitForSingleObject(skeleton_frame_ready_event_, 0) != WAIT_OBJECT_0)
    return false;

  KinectSkeletonFrame skeleton_frame;
  NUI_SKELETON_FRAME* frame = skeleton_frame.GetSkeletonFramePtr();

  HRESULT res = native_sensor_->NuiSkeletonGetNextFrame(0, frame);

  if (FAILED(res))
    return false;

  // Smooth out the skeleton data.
  native_sensor_->NuiTransformSmooth(frame, nullptr);

  // Try to always track the same skeletons.
  DWORD track_ids[kNumTrackedSkeletons];
  ZeroMemory(track_ids, sizeof(track_ids));

  for (int i = 0; i < kNumTrackedSkeletons; ++i) {
    for (int j = 0; j < NUI_SKELETON_COUNT; ++j) {
      if (frame->SkeletonData[j].eTrackingState != NUI_SKELETON_NOT_TRACKED) {
        DWORD track_id = frame->SkeletonData[j].dwTrackingID;
        if (track_id == skeleton_sticky_ids_[i]) {
          track_ids[i] = track_id;
          break;
        }
      }
    }
  }

  for (int i = 0; i < NUI_SKELETON_COUNT; i++) {
    if (track_ids[0] && track_ids[1])
      break;

    if (frame->SkeletonData[i].eTrackingState != NUI_SKELETON_NOT_TRACKED) {
      DWORD track_id = frame->SkeletonData[i].dwTrackingID;

      if (!track_ids[0] && track_id != track_ids[1])
        track_ids[0] = track_id;
      else if (!track_ids[1] && track_id != track_ids[0])
        track_ids[1] = track_id;
    }
  }

  skeleton_sticky_ids_[0] = track_ids[0];
  skeleton_sticky_ids_[1] = track_ids[1];

  skeleton_frame.SetTrackedSkeletons(track_ids[0], track_ids[1]);
  state->InsertSkeletonFrame(skeleton_frame);

  native_sensor_->NuiSkeletonSetTrackedSkeletons(track_ids);

  return true;
}

bool KinectSensor::MapSkeletonPointToDepthPoint(Vector4 skeleton_point,
                                                cv::Vec2i* depth_point,
                                                int* depth) {
  NUI_DEPTH_IMAGE_POINT depth_image_point;
  HRESULT res = coordinate_mapper_->MapSkeletonPointToDepthPoint(
      &skeleton_point, kDepthImageResolution, &depth_image_point);
  if (FAILED(res))
    return false;

  *depth_point = cv::Vec2i(depth_image_point.x, depth_image_point.y);
  *depth = depth_image_point.depth;

  return true;
}

KinectSensor::~KinectSensor() {
  // TODO(fdoray): Release the depth stream?
  native_sensor_->NuiShutdown();
  SafeRelease(native_sensor_);
}

}  // namespace kinect_wrapper