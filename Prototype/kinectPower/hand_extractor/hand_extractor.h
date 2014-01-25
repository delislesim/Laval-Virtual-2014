#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace hand_extractor {

class HandExtractor {
 public:
  HandExtractor(int hands_depth, int hands_depth_tolerance);

  // Generates a matrix in which each pixel belonging to an hand is identified
  // with a different value.
  void ExtractHands(const cv::Mat& depth_mat,
                    std::vector<cv::Point>* hand_positions,
                    cv::Mat* simplified_depth_mat) const;

 private:
  // Generates a matrix in which pixels that are part of an hand are equal to
  // 1 and other pixels are equal to 0. A pixel is considered to be part of an
  // hand if it's in the range |hands_depth_| +/- |hands_depth_tolerance_|.
  // TODO(fdoray): Not used?
  void ComputeHandsMask(const cv::Mat& depth_mat, cv::Mat* hands_mat) const;

  int hands_depth_;
  int hands_depth_tolerance_;
};

}  // namespace hand_extractor