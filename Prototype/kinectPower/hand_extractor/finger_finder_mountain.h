#pragma once

#include <opencv2/core/core.hpp>

namespace hand_extractor {

class Hand2dParameters;

class FingerFinderMountain {
 public:
  FingerFinderMountain();

  // Computes the position of the fingers in a 2D image.
  void FindFingers(const std::vector<cv::Point>& contour,
                   const unsigned char contour_pixel_value,
                   const cv::Mat& depth_mat,
                   const cv::Mat& segmentation_mat,
                   const Hand2dParameters* previous_hand_parameters,
                   Hand2dParameters* hand_parameters) const;

 private:
  
};

}  // namespace hand_extractor