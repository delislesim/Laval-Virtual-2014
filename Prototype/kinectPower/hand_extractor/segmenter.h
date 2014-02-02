#pragma once

#include <opencv2/core/core.hpp>

namespace hand_extractor {

class Segmenter {
 public:
  Segmenter(int hands_depth, int hands_depth_tolerance);

  void SegmentHands(const cv::Mat& depth_mat,
                    std::vector<std::vector<cv::Point>>* contours,
                    cv::Mat* segmentation_mat) const;

 private:
  bool PixelInDepthRange(unsigned short pixel_value) const {
    return pixel_value > min_depth_ && pixel_value < max_depth_;
  }

  // Generates a matrix in which pixels that are part of an hand are equal to
  // 1 and other pixels are equal to 0. A pixel is considered to be part of an
  // hand if it's in the range |hands_depth_| +/- |hands_depth_tolerance_|.
  void ComputeHandsMask(const cv::Mat& depth_mat, cv::Mat* hands_mask) const;

  int hands_depth_;
  int hands_depth_tolerance_;

  unsigned short min_depth_;
  unsigned short max_depth_;
};

}  // namespace hand_extractor