#include "hand_extractor/hand_extractor.h"

#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "algos/stable_matching.h"
#include "base/logging.h"
#include "hand_extractor/hand_2d_parameters.h"
#include "image/contour_center.h"
#include "image/image_constants.h"
#include "maths/maths.h"

using namespace cv;

namespace hand_extractor {

namespace {

const int kMaxSquareDistanceToMatchHands = 50;

}  // namespace

HandExtractor::HandExtractor(int hands_depth, int hands_depth_tolerance)
    : segmenter_(hands_depth, hands_depth_tolerance) {
}

void HandExtractor::ExtractHands(const cv::Mat& depth_mat,
                                 cv::Mat* segmentation_mat) {
  assert(segmentation_mat);

  // Find the contour of each hand.
  std::vector<std::vector<cv::Point> > contours;
  segmenter_.SegmentHands(depth_mat, &contours, segmentation_mat);

  // Find the center of each hand.
  std::vector<cv::Point> contours_centers;
  for (size_t i = 0; i < contours.size(); ++i) {
    cv::Point center = image::ContourCenter(contours[i]);
    contours_centers.push_back(center);
  }

  // Match each hand from the previous frame with a contour from the current
  // frame.
  algos::StableMatchingQueue potential_pairs;
  for (size_t previous_index = 0; 
       previous_index < last_hands_parameters_.size();
       ++previous_index) {
    for (size_t current_index = 0;
         current_index < contours.size();
         ++current_index) {
      algos::StableMatchingPair pair;
      pair.left = current_index;
      pair.right = previous_index;
      pair.distance = maths::DistanceSquare(
          last_hands_parameters_[previous_index].GetContourCenter(),
          contours_centers[current_index]
      );
      potential_pairs.push(pair);
    }
  }
  std::vector<int> best_pairs;
  algos::StableMatching(
      contours.size(),
      last_hands_parameters_.size(),
      kMaxSquareDistanceToMatchHands,
      &potential_pairs,
      &best_pairs);

  // Find the position of the fingers.
  std::vector<Hand2dParameters> hands_parameters;

  for (size_t i = 0; i < contours.size(); ++i) {
    // Retrieve the parameters of the hand at last frame.
    const Hand2dParameters* last_hand_parameters = NULL;
    if (best_pairs[i] != -1)
      last_hand_parameters = &last_hands_parameters_[best_pairs[i]];

    // Compute the new hand parameters.
    hands_parameters.push_back(Hand2dParameters());
    finger_finder_.FindFingers(contours[i], static_cast<unsigned char>(i + 1),
                               depth_mat, *segmentation_mat,
                               last_hand_parameters, &hands_parameters[i]);

    // Remember the center of the hand.
    hands_parameters[i].SetContourCenter(contours_centers[i]);

    // Draw a small circle on each fingertip.
    const cv::Scalar kFingertipColor = 39;
    for (size_t j = 0; j < Hand2dParameters::JOINTS_COUNT; ++j) {
      Hand2dParameters::HandJoint joint =
          static_cast<Hand2dParameters::HandJoint>(j);
      cv::Point joint_position;
      if (hands_parameters[i].GetJointPosition(joint, &joint_position)) {
        cv::circle(*segmentation_mat, joint_position, 2,
                   kFingertipColor, image::kThickness2);
      }
    }

  }

  // Remember the hand parameters.
  last_hands_parameters_ = hands_parameters;

}

}  // namespace hand_extractor