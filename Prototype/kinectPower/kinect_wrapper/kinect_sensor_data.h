#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/observer_list.h"
#include "kinect_interaction/interaction_frame.h"
#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"

namespace kinect_wrapper {  

class KinectObserver;

class KinectSensorData {
 public:
  KinectSensorData();
  ~KinectSensorData();

  void CreateBuffers();

  // Retrieves the last depth matrix.
  // @param mat the last depth matrix.
  // @returns true in case of success, false otherwise.
  bool QueryDepth(cv::Mat* mat) const;

  // Retrieves the last color matrix.
  // @param mat the last color matrix.
  // @returns true in case of success, false otherwise.
  bool QueryColor(cv::Mat* mat) const;
  
  const KinectSkeletonFrame* GetSkeletonFrame() const {
    return &skeleton_buffer_;
  }

  kinect_interaction::InteractionFrame* GetInteractionFrame() {
    return &interaction_buffer_;
  }
  const kinect_interaction::InteractionFrame* GetInteractionFrame() const {
    return &interaction_buffer_;
  }

  void InsertDepthFrame(const char* depth_frame, size_t depth_frame_size);
  void InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                        const size_t& num_pixels);
  void InsertColorFrame(const char* color_frame,
                        const size_t& color_frame_size);
  void InsertSkeletonFrame(const KinectSkeletonFrame& skeleton_frame);
  void InsertInteractionFrame(const NUI_INTERACTION_FRAME& interaction_frame);
  
  void AddObserver(KinectObserver* obs);

 private:
  cv::Mat depth_buffer_;
  cv::Mat color_buffer_;
  KinectSkeletonFrame skeleton_buffer_;
  kinect_interaction::InteractionFrame interaction_buffer_;

  ObserverList<KinectObserver> observers_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensorData);
};

}  // namespace kinect_wrapper