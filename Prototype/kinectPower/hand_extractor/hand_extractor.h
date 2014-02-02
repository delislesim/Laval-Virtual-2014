#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/base.h"
#include "hand_extractor/finger_finder_convex.h"
#include "hand_extractor/finger_finder_curve.h"
#include "hand_extractor/finger_finder_mountain.h"
#include "hand_extractor/hand_2d_parameters.h"
#include "hand_extractor/segmenter.h"

namespace hand_extractor {

class HandExtractor {
 public:
  HandExtractor(int hands_depth, int hands_depth_tolerance);

  // Generates a matrix in which each pixel that belong to an hand is identified
  // with a different value.
  void ExtractHands(const cv::Mat& depth_mat,
                    cv::Mat* segmentation_mat,
                    std::vector<Hand2dParameters>* hand_parameters);

 private:
  std::vector<Hand2dParameters> last_hands_parameters_;

  Segmenter segmenter_;
  FingerFinderConvex finger_finder_convex_;
  FingerFinderCurve finger_finder_curve_;
  FingerFinderMountain finger_finder_mountain_;

  DISALLOW_COPY_AND_ASSIGN(HandExtractor);
};

}  // namespace hand_extractor