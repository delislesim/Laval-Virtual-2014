#include "finger_finder/finger_finder.h"

#include "finger_finder/find_fingers_in_contour.h"
#include "finger_finder/match_contours_to_previous_hands.h"
#include "finger_finder/segmenter.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"

namespace finger_finder {

FingerFinder::FingerFinder(int hands_depth, int hands_depth_tolerance)
    : min_hands_depth_(hands_depth - hands_depth_tolerance),
      max_hands_depth_(hands_depth + hands_depth_tolerance) {
}

void FingerFinder::FindFingers(const cv::Mat& depth_mat,
                               cv::Mat* nice_image) {
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols == static_cast<int>(kinect_wrapper::kKinectDepthWidth));
  assert(depth_mat.rows == static_cast<int>(kinect_wrapper::kKinectDepthHeight));
  assert(nice_image);

  *nice_image = cv::Mat::zeros(depth_mat.size(), CV_8UC4);
  image::InitializeBlackImage(nice_image);

  // Find contours in the depth image.
  std::vector<std::vector<cv::Point> > contours;
  Segmenter(depth_mat, min_hands_depth_, max_hands_depth_, &contours);
  
  // Match each contour from this image with a hand from the previous image.
  std::vector<cv::Point> contours_centers;
  std::vector<int> contour_to_hands_match;
  MatchContoursToPreviousHands(contours, hands_, &contours_centers,
                               &contour_to_hands_match);

  // Update the center of each hand, or create new hands.
  std::vector<HandParameters> new_hands;
  for (size_t i = 0; i < contours_centers.size(); ++i) {
    if (contour_to_hands_match[i] != -1)
      new_hands.push_back(hands_[contour_to_hands_match[i]]);
    else
      new_hands.push_back(HandParameters());
    new_hands[new_hands.size() - 1].SetContourCenter(contours_centers[i]);
  }
  hands_ = new_hands;

  // Find the position of the fingers in each hand.
  for (size_t i = 0; i < contours.size(); ++i) {
    FindFingersInContour(depth_mat, contours[i],
                         &hands_[i], nice_image);
  }
}

}  // namespace hand_extractor