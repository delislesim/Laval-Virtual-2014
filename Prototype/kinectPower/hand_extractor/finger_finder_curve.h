#pragma once

#include <opencv2/core/core.hpp>

namespace hand_extractor {

class Hand2dParameters;

class FingerFinderCurve {
 public:
  FingerFinderCurve(int hands_depth, int hands_depth_tolerance);

  // Computes the position of the fingers in a 2D image.
  void FindFingers(const std::vector<cv::Point>& contour,
                   const unsigned char contour_pixel_value,
                   const cv::Mat& depth_mat,
                   cv::Mat* segmentation_mat,
                   const Hand2dParameters* previous_hand_parameters,
                   Hand2dParameters* hand_parameters) const;

 private:
  int AverageDepth(const cv::Point& position, const cv::Mat& depth_mat) const;

  int min_z;
  int max_z;
};

}  // namespace hand_extractor