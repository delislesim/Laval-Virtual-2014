#pragma once

#include <opencv2/core/core.hpp>

namespace kinect_wrapper {

class KinectSensorState;
class KinectSkeletonFrame;

class KinectObserver {
 public:
  virtual void ObserveDepth(const cv::Mat& /* depth_mat */,
                            const KinectSensorState& /* sensor_state */) {}
  virtual void ObserveSkeleton(const KinectSkeletonFrame& /* skeleton_frame */,
                               const KinectSensorState& /* sensor_state */) {}
};

}  // namespace kinect_wrapper