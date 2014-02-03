#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "finger_finder/hand_parameters.h"
#include "kinect_wrapper/kinect_switch.h"

namespace finger_finder {

class FingerFinder {
 public:
  typedef std::vector<HandParameters> HandParametersVector;

  FingerFinder(int hands_depth, int hands_depth_tolerance);

  // Computes the position of the fingers from a 2D image.
  void FindFingers(const cv::Mat& depth_mat,
                   cv::Mat* nice_image);

 private:
  HandParametersVector hands_;

  // Profondeur minimale et maximale permise pour les mains.
  int min_hands_depth_;
  int max_hands_depth_;

};

}  // namespace finger_finder