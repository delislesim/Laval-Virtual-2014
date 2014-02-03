#include "finger_finder/finger_finder.h"

#include "finger_finder/find_fingers_in_contour.h"
#include "finger_finder/match_contours_to_previous_hands.h"
#include "finger_finder/segmenter.h"
#include "kinect_wrapper/constants.h"

namespace finger_finder {

FingerFinder::FingerFinder(int hands_depth, int hands_depth_tolerance)
    : min_hands_depth_(hands_depth - hands_depth_tolerance),
      max_hands_depth_(hands_depth + hands_depth_tolerance) {
}

void FingerFinder::FindFingers(const cv::Mat& depth_mat) {
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols == kinect_wrapper::kKinectDepthWidth);
  assert(depth_mat.rows == kinect_wrapper::kKinectDepthHeight);

  cv::Mat nice_image_tmp = cv::Mat::zeros(depth_mat.size(), CV_8UC4);
  HandParametersVector hands;
  hands_.GetCurrent(&hands);

  // Find contours in the depth image.
  std::vector<std::vector<cv::Point> > contours;
  Segmenter(depth_mat, min_hands_depth_, max_hands_depth_, &contours);

  // Match each contour from this image with a hand from the previous image.
  std::vector<cv::Point> contours_centers;
  std::vector<int> contour_to_hands_match;
  MatchContoursToPreviousHands(contours, hands, &contours_centers,
                               &contour_to_hands_match);

  // Update the center of each hand, or create new hands.
  for (size_t i = 0; i < contours_centers.size(); ++i) {
    if (contour_to_hands_match[i] != -1) {
      hands[contour_to_hands_match[i]].SetContourCenter(contours_centers[i]);
    } else {
      contour_to_hands_match[i] = hands.size();
      hands.push_back(HandParameters());
    }
  }

  // Find the position of the fingers in each hand.
  for (size_t i = 0; i < contours.size(); ++i) {
    FindFingersInContour(depth_mat, contours[i],
                         &hands[contour_to_hands_match[i]],
                         &nice_image_tmp);
  }

  // Publish the next hand parameters and nice image.
  nice_image_ = nice_image_tmp;
  hands_.SetNext(hands);
}

}  // namespace hand_extractor