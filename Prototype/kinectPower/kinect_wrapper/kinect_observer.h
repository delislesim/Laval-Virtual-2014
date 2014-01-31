#pragma once

#include <opencv2/core/core.hpp>

namespace kinect_wrapper {

class KinectSensorData;
class KinectSkeletonFrame;

class KinectObserver {
 public:
  virtual void ObserveDepth(const cv::Mat& /* depth_mat */,
                            const KinectSensorData& /* sensor_data */) {}
  virtual void ObserveColor(const cv::Mat& /* color_mat */,
                            const KinectSensorData& /* sensor_data */) {}
  virtual void ObserveSkeleton(const KinectSkeletonFrame& /* skeleton_frame */,
                               const KinectSensorData& /* sensor_data */) {}
};

}  // namespace kinect_wrapper