#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_interaction {
class InteractionClientBase;
}

namespace kinect_wrapper {  

namespace {

  const NUI_IMAGE_TYPE kColorImageType = NUI_IMAGE_TYPE_COLOR;
  const NUI_IMAGE_TYPE kDepthImageType = NUI_IMAGE_TYPE_DEPTH_AND_PLAYER_INDEX;
  const NUI_IMAGE_RESOLUTION kDepthImageResolution = NUI_IMAGE_RESOLUTION_640x480;
  const NUI_IMAGE_RESOLUTION kColorImageResolution = NUI_IMAGE_RESOLUTION_640x480;

}  // namespace

class KinectSensorData;

class KinectSensor {
 public:
  KinectSensor(INuiSensor* native_sensor, NUI_IMAGE_TYPE color_stream_type = kColorImageType, NUI_IMAGE_TYPE depth_stream_type = kDepthImageType);
  ~KinectSensor();

  void SetNearModeEnabled(bool near_mode_enabled);

  // Depth stream.
  bool OpenDepthStream();
  bool PollNextDepthFrame(KinectSensorData* data);
  HANDLE GetDepthFrameReadyEvent() const {
    return depth_frame_ready_event_;
  }
  size_t depth_stream_width() const {
    return depth_stream_width_;
  }
  size_t depth_stream_height() const {
    return depth_stream_height_;
  }

  // Color stream.
  bool OpenColorStream();
  bool PollNextColorFrame(KinectSensorData* data);
  HANDLE GetColorFrameReadyEvent() const {
    return color_frame_ready_event_;
  }
  size_t color_stream_width() const {
    return color_stream_width_;
  }
  size_t color_stream_height() const {
    return color_stream_height_;
  }

  // Skeleton stream.
  bool OpenSkeletonStream();
  bool PollNextSkeletonFrame(KinectSensorData* data);
  HANDLE GetSkeletonFrameReadyEvent() const {
    return skeleton_frame_ready_event_;
  }

  // Interaction stream.
  bool OpenInteractionStream(
      kinect_interaction::InteractionClientBase* interaction_client);
  bool PollNextInteractionFrame(KinectSensorData* data);
  HANDLE GetInteractionFrameReadyEvent() const {
    return interaction_frame_ready_event_;
  }
  void CloseInteractionStream();

  // Coordinate mapper.
  bool MapSkeletonPointToDepthPoint(Vector4 skeleton_point,
                                    cv::Vec2i* depth_point,
                                    int* depth);
  bool MapDepthPointToColorPoint(NUI_DEPTH_IMAGE_POINT& depth_point,
                                 NUI_COLOR_IMAGE_POINT* color_point);

  // Color and depth stream types
  NUI_IMAGE_TYPE color_stream_type() const { return color_stream_type_; }
  void color_stream_type(NUI_IMAGE_TYPE val) { color_stream_type_ = val; }

  NUI_IMAGE_TYPE depth_stream_type() const { return depth_stream_type_; }
  void depth_stream_type(NUI_IMAGE_TYPE val) { depth_stream_type_ = val; }

 private:
  INuiSensor* native_sensor_;

  bool near_mode_enabled_;

  // Depth stream.
  bool depth_stream_opened_;
  HANDLE depth_frame_ready_event_;
  HANDLE depth_stream_handle_;
  size_t depth_stream_width_;
  size_t depth_stream_height_;
  NUI_IMAGE_TYPE depth_stream_type_;

  // Color stream.
  bool color_stream_opened_;
  HANDLE color_frame_ready_event_;
  HANDLE color_stream_handle_;
  size_t color_stream_width_;
  size_t color_stream_height_;
  NUI_IMAGE_TYPE color_stream_type_;

  // Skeleton stream.
  bool skeleton_seated_enabled_;
  bool skeleton_near_enabled_;  
  bool skeleton_stream_opened_;
  HANDLE skeleton_frame_ready_event_;
  DWORD skeleton_sticky_ids_[kNumTrackedSkeletons];

  // Interaction stream.
  bool interaction_stream_opened_;
  INuiInteractionStream* interaction_stream_;
  HANDLE interaction_frame_ready_event_;

  // Coordinate mapper.
  INuiCoordinateMapper* coordinate_mapper_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensor);
};

}  // namespace kinect_wrapper