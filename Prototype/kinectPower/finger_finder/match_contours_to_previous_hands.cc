#include "finger_finder/match_contours_to_previous_hands.h"

#include "algos/stable_matching.h"
#include "image/contour_center.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {

const int kMaxSquareDistanceToMatchHands = 20 * 20;

}  // namespace

void MatchContoursToPreviousHands(
    const std::vector<std::vector<cv::Point> >& contours,
    const std::vector<HandParameters>& previous_hand_parameters,
    std::vector<cv::Point>* contours_centers,
    std::vector<int>* matches) {
  assert(contours_centers);
  assert(contours_centers->empty());
  assert(matches);
  assert(matches->empty());

  // Find the center of each hand.
  for (size_t i = 0; i < contours.size(); ++i) {
    cv::Point center = image::ContourCenter(contours[i]);
    contours_centers->push_back(center);
  }

  // Match each hand from the previous frame with a contour from the current
  // frame.
  algos::StableMatchingQueue potential_pairs;
  for (size_t previous_index = 0;
       previous_index < previous_hand_parameters.size();
       ++previous_index) {
    for (size_t current_index = 0;
         current_index < contours.size();
         ++current_index) {
      algos::StableMatchingPair pair;
      pair.left = current_index;
      pair.right = previous_index;
      pair.distance = maths::DistanceSquare(
          previous_hand_parameters[previous_index].contour_center(),
          (*contours_centers)[current_index]
      );
      potential_pairs.push(pair);
    }
  }
  algos::StableMatching(
      contours.size(),
      previous_hand_parameters.size(),
      kMaxSquareDistanceToMatchHands,
      &potential_pairs,
      matches);
}

}  // namespace finger_finder