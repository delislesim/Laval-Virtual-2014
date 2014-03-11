#include "kinect_wrapper/kinect_sensor.h"

#include <iostream>

#include "base/logging.h"
#include "base/timer.h"
#include "kinect_interaction/interaction_client_base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor_data.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

KinectSensor::KinectSensor(INuiSensor* native_sensor, NUI_IMAGE_TYPE color_stream_type, NUI_IMAGE_TYPE depth_stream_type)
    : native_sensor_(native_sensor),
      near_mode_enabled_(false),
      depth_stream_opened_(false),
      depth_frame_ready_event_(INVALID_HANDLE_VALUE),
      depth_stream_handle_(INVALID_HANDLE_VALUE),
      depth_stream_width_(0),
      depth_stream_height_(0),
      color_stream_opened_(false),
      color_frame_ready_event_(INVALID_HANDLE_VALUE),
      color_stream_handle_(INVALID_HANDLE_VALUE),
      color_stream_width_(0),
      color_stream_height_(0),
      skeleton_seated_enabled_(false),
      skeleton_stream_opened_(false),
      skeleton_frame_ready_event_(INVALID_HANDLE_VALUE),
      interaction_stream_opened_(false),
      interaction_stream_(NULL),
      interaction_frame_ready_event_(INVALID_HANDLE_VALUE),
      depth_stream_type_(depth_stream_type),
      color_stream_type_(color_stream_type),
      angle_thread_(INVALID_HANDLE_VALUE),
      angle_event_(INVALID_HANDLE_VALUE),
      target_angle_(0),
      num_skeletons_to_avoid_(0) {
  depth_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
  color_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
  skeleton_frame_ready_event_ = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);
  interaction_frame_ready_event_ =
      ::CreateEventW(nullptr, TRUE, FALSE, nullptr);

  skeleton_sticky_ids_[0] = 0;
  skeleton_sticky_ids_[1] = 0;

  for (int i = 0; i < NUI_SKELETON_COUNT; ++i) {
    skeletons_to_avoid_[i] = 0;
  }

  native_sensor_->NuiGetCoordinateMapper(&coordinate_mapper_);

  // Creer le thread et l'event pour definir l'angle de la Kinect.
  angle_event_.Set(::CreateEventW(nullptr, TRUE, FALSE, nullptr));
  angle_thread_.Set(::CreateThread(
      nullptr, 0, (LPTHREAD_START_ROUTINE)KinectSensor::AngleThread, this,
      0, nullptr));
}

void KinectSensor::SetNearModeEnabled(bool near_mode_enabled) {
  near_mode_enabled_ = near_mode_enabled;
}

void KinectSensor::SetAngle(int angle) {
  if (angle > 27)
    angle = 27;
  else if (angle < -27)
    angle = -27;

  target_angle_ = angle;
  ::SetEvent(angle_event_.get());
}

int KinectSensor::GetAngle() {
  LONG angle = 0;
  native_sensor_->NuiCameraElevationGetAngle(&angle);
  return angle;
}

bool KinectSensor::OpenDepthStream() {
  if (depth_stream_opened_)
    return true;

  HRESULT res = native_sensor_->NuiImageStreamOpen(
      depth_stream_type_, kDepthImageResolution, 0, 2, depth_frame_ready_event_,
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

bool KinectSensor::PollNextDepthFrame(KinectSensorData* data) {
  assert(data);
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

  /*
  const NUI_DEPTH_IMAGE_PIXEL* start =
      reinterpret_cast<const NUI_DEPTH_IMAGE_PIXEL*>(locked_rect.pBits);
  data->InsertDepthFrame(start, depth_stream_width_ * depth_stream_height_);
  */

  if (interaction_stream_opened_) {
    // Provide the data to the interaction stream.
    HRESULT res = interaction_stream_->ProcessDepth(locked_rect.size,
                                      locked_rect.pBits,
                                      image_frame.liTimeStamp);
  }

  image_frame.pFrameTexture->UnlockRect(0);

ReleaseFrame:
  native_sensor_->NuiImageStreamReleaseFrame(depth_stream_handle_, &image_frame);
  return res;
}

bool KinectSensor::OpenColorStream() {
  if (color_stream_opened_)
    return true;

  HRESULT res = native_sensor_->NuiImageStreamOpen(
    color_stream_type_, kColorImageResolution, 0, 2, color_frame_ready_event_,
    &color_stream_handle_);

  if (FAILED(res))
    return false;

  DWORD width = 0;
  DWORD height = 0;
  ::NuiImageResolutionToSize(kColorImageResolution, width, height);
  color_stream_width_ = width;
  color_stream_height_ = height;

  color_stream_opened_ = true;
  return true;
}

bool KinectSensor::PollNextColorFrame(KinectSensorData* data) {
  assert(data);
  assert(color_stream_opened_);
  assert(color_stream_handle_ != INVALID_HANDLE_VALUE);

  if (WaitForSingleObject(color_frame_ready_event_, 0) != WAIT_OBJECT_0)
    return false;

  bool res = true;

  NUI_IMAGE_FRAME image_frame;

  HRESULT hr = native_sensor_->NuiImageStreamGetNextFrame(
    color_stream_handle_, 0, &image_frame);
  if (FAILED(hr)) {
    res = false;
    goto ReleaseFrame;
  }

  INuiFrameTexture* texture = image_frame.pFrameTexture;
  NUI_LOCKED_RECT locked_rect;

  // Lock the frame data so the Kinect knows not to modify it while we're
  // reading it.
  texture->LockRect(0, &locked_rect, NULL, 0);

  if (locked_rect.Pitch == 0) {
    res = false;
    goto ReleaseFrame;
  }

  data->InsertColorFrame(
      reinterpret_cast<const char*>(locked_rect.pBits),
      locked_rect.size);

  image_frame.pFrameTexture->UnlockRect(0);

ReleaseFrame:
  native_sensor_->NuiImageStreamReleaseFrame(color_stream_handle_, &image_frame);
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

bool KinectSensor::PollNextSkeletonFrame(KinectSensorData* data) {
  assert(data);
  //assert(skeleton_stream_opened_);

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

  // Find new skeletons.
  FindNewSkeletons(track_ids, frame);
  if (!track_ids[0] && !track_ids[1] && num_skeletons_to_avoid_ != 0) {
    num_skeletons_to_avoid_ = 0;
    FindNewSkeletons(track_ids, frame);
  }

  skeleton_sticky_ids_[0] = track_ids[0];
  skeleton_sticky_ids_[1] = track_ids[1];

  skeleton_frame.SetTrackedSkeletons(track_ids[0], track_ids[1]);
  data->InsertSkeletonFrame(skeleton_frame);

  native_sensor_->NuiSkeletonSetTrackedSkeletons(track_ids);

  if (interaction_stream_opened_) {
    // Provide the data to the interaction stream.
    Vector4 gravity = { 0 };
    native_sensor_->NuiAccelerometerGetCurrentReading(&gravity);
    HRESULT res = interaction_stream_->ProcessSkeleton(kNumSkeletons, frame->SkeletonData,
                                                       &gravity, frame->liTimeStamp);

    data->GetInteractionFrame()->SetTrackedSkeletons(track_ids[0],
                                                     track_ids[1]);
  }

  return true;
}

void KinectSensor::AvoidCurrentSkeleton() {
  if (skeleton_sticky_ids_[0] == 0) {
    skeleton_sticky_ids_[0] = skeleton_sticky_ids_[1];
    skeleton_sticky_ids_[1] = 0;
    return;
  }

  if (num_skeletons_to_avoid_ == NUI_SKELETON_COUNT) {
    num_skeletons_to_avoid_ = 0;
  }

  skeletons_to_avoid_[num_skeletons_to_avoid_] = skeleton_sticky_ids_[0];
  ++num_skeletons_to_avoid_;

  skeleton_sticky_ids_[0] = 0;
  skeleton_sticky_ids_[1] = 0;
}

void KinectSensor::FindNewSkeletons(DWORD* track_ids, NUI_SKELETON_FRAME* frame) {
  for (int i = 0; i < NUI_SKELETON_COUNT; i++) {
    if (track_ids[0] && track_ids[1])
      break;

    if (frame->SkeletonData[i].eTrackingState != NUI_SKELETON_NOT_TRACKED &&
        frame->SkeletonData[i].Position.z < kDistanceMaxToTrackSkeleton) {
      DWORD track_id = frame->SkeletonData[i].dwTrackingID;

      // Ne pas selectionner le squelette s'il fait partie de ceux a eviter.
      bool ok = true;
      for (int j = 0; j < num_skeletons_to_avoid_; ++j) {
        if (track_id == skeletons_to_avoid_[j]) {
          ok = false;
        }
      }
      if (!ok)
        continue;

      if (!track_ids[0] && track_id != track_ids[1])
        track_ids[0] = track_id;
      else if (!track_ids[1] && track_id != track_ids[0])
        track_ids[1] = track_id;
    }
  }
}

bool KinectSensor::OpenInteractionStream(
    kinect_interaction::InteractionClientBase* interaction_client) {
  assert(interaction_client);

  if (interaction_stream_opened_)
    return true;

  HRESULT hr = ::NuiCreateInteractionStream(native_sensor_,
                                            interaction_client,
                                            &interaction_stream_);
  if (FAILED(hr))
    return false;

  hr = interaction_stream_->Enable(interaction_frame_ready_event_);

  if (FAILED(hr)) {
    SafeRelease(interaction_stream_);
    return false;
  }

  interaction_stream_opened_ = true;

  return true;
}

bool KinectSensor::PollNextInteractionFrame(KinectSensorData* data) {
  assert(data);
  assert(interaction_stream_opened_);

  if (WaitForSingleObject(interaction_frame_ready_event_, 0) != WAIT_OBJECT_0)
    return false;

  NUI_INTERACTION_FRAME interaction_frame;
  ::ZeroMemory(&interaction_frame, sizeof(interaction_frame));
  HRESULT hr = interaction_stream_->GetNextFrame(0, &interaction_frame);
  if (FAILED(hr))
    return false;

  data->InsertInteractionFrame(interaction_frame);
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

bool KinectSensor::MapDepthPointToColorPoint(NUI_DEPTH_IMAGE_POINT& depth_point,
                                             NUI_COLOR_IMAGE_POINT* color_point) {
  HRESULT res = coordinate_mapper_->MapDepthPointToColorPoint(kDepthImageResolution,
                                                              &depth_point,
                                                              kColorImageType,
                                                              kColorImageResolution,
                                                              color_point);
  return SUCCEEDED(res);
}

bool KinectSensor::MapColorFrameToDepthFrame(NUI_DEPTH_IMAGE_PIXEL* depth_pixels,
                                             NUI_DEPTH_IMAGE_POINT* depth_points) {
  HRESULT res = coordinate_mapper_->MapColorFrameToDepthFrame(kColorImageType,
                                                              kColorImageResolution,
                                                              kDepthImageResolution,
                                                              kKinectDepthWidth * kKinectDepthHeight,
                                                              depth_pixels,
                                                              kKinectDepthWidth * kKinectDepthHeight,
                                                              depth_points);
  return SUCCEEDED(res);
}

void KinectSensor::CloseInteractionStream() {
  if (!interaction_stream_opened_)
    return;

  interaction_stream_->Disable();
  SafeRelease(interaction_stream_);
}

KinectSensor::~KinectSensor() {
  // TODO(fdoray): Release the depth stream?
  native_sensor_->NuiShutdown();
  SafeRelease(native_sensor_);

  ::CloseHandle(depth_frame_ready_event_);
  ::CloseHandle(color_frame_ready_event_);
  ::CloseHandle(skeleton_frame_ready_event_);
  ::CloseHandle(interaction_frame_ready_event_);

  assert(angle_event_.get() == INVALID_HANDLE_VALUE);
  assert(angle_thread_.get() == INVALID_HANDLE_VALUE);
}

void KinectSensor::Shutdown() {
  // Fermer le thread servant a definir l'angle.
  target_angle_ = -999;
  ::SetEvent(angle_event_.get());
  ::WaitForSingleObject(angle_thread_.get(), INFINITE);

  angle_event_.Close();
  angle_thread_.Close();
}

DWORD KinectSensor::AngleThread(KinectSensor* sensor) {
  for (;;) {
    DWORD ret = ::WaitForSingleObject(sensor->angle_event_.get(), INFINITE);

    if (ret != WAIT_OBJECT_0)  // Thread close event.
      break;
    
    if (sensor->target_angle_ == -999)
      break;

    // Definir l'angle de la Kinect.
    sensor->native_sensor_->NuiCameraElevationSetAngle(sensor->target_angle_);
  }

  return 1;
}

}  // namespace kinect_wrapper