#include "hand_extractor/finger_finder_mountain.h"

#include <algorithm>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "base/logging.h"
#include "hand_extractor/hand_2d_parameters.h"
#include "image/image_constants.h"
#include "maths/maths.h"

using namespace cv;

namespace hand_extractor {

namespace {

}  // namespace

FingerFinderMountain::FingerFinderMountain() {
}

void FingerFinderMountain::FindFingers(
    const std::vector<cv::Point>& contour,
    const unsigned char contour_pixel_value,
    const cv::Mat& depth_mat,
    const cv::Mat& segmentation_mat,
    const Hand2dParameters* previous_hand_parameters,
    Hand2dParameters* hand_parameters) const {
  // |previous_hand_parameters| can be NULL.
  assert(hand_parameters);

  
}

}  // namespace hand_extractor