#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "kinect_wrapper/kinect_sensor_data.h"

namespace finger_finder_thinning {

class FingerFinder {
 public:
  FingerFinder(int hands_depth, int hands_depth_tolerance);

  // Computes the position of the fingers from a 2D image.
  void FindFingers(const kinect_wrapper::KinectSensorData& data,
                   cv::Mat* nice_image);

 private:
  // Profondeur minimale et maximale permise pour les mains.
  int min_hands_depth_;
  int max_hands_depth_;

};

}  // namespace finger_finder_thinning