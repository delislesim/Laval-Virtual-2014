#include "hand_extractor/hand_2d_parameters.h"

#include "algos/stable_matching.h"
#include "maths/maths.h"
#include "maths/smooth.h"

namespace hand_extractor {

namespace {

const int kStableMatchingMaxDistance = 225;

const double min_speed_distance = 2;
const double min_speed = 1;
const double max_speed_distance = 8;
const double max_speed = 6;

}  // namespace

Hand2dParameters::Hand2dParameters() {
}

void Hand2dParameters::SmoothUsingPreviousParameters(const Hand2dParameters* previous) {
  if (previous == NULL) {
    for (int i = 0; i < tips_.size(); ++i) {
      tips_[i].smoothed_position = tips_[i].position;
    }
    return;
  }

  // Find corresponding pairs of points.
  algos::StableMatchingQueue stable_matching_queue;
  for (int i = 0; i < tips_.size(); ++i) {
    for (int j = 0; j < previous->tips_.size(); ++j) {
      algos::StableMatchingPair pair;
      pair.left = i;
      pair.right = j;
      pair.distance = maths::DistanceSquare(tips_[i].position, previous->tips_[j].position);
      stable_matching_queue.push(pair);
    }
  }
  std::vector<int> best_pairs;
  algos::StableMatching(tips_.size(), previous->tips_.size(), kStableMatchingMaxDistance,
                        &stable_matching_queue, &best_pairs);

  // Smooth each fingertip.
  for (int i = 0; i < tips_.size(); ++i) {
    if (best_pairs[i] == -1) {
      tips_[i].smoothed_position = tips_[i].position;
      continue;
    }

    cv::Point position = tips_[i].position;
    cv::Point previous_position = previous->tips_[best_pairs[i]].smoothed_position;

    tips_[i].smoothed_position.x = maths::Smooth(
      previous_position.x, position.x,
      min_speed_distance, min_speed,
      max_speed_distance, max_speed);
    tips_[i].smoothed_position.y = maths::Smooth(
      previous_position.y, position.y,
      min_speed_distance, min_speed,
      max_speed_distance, max_speed);

    int depth = tips_[i].depth;
    int previous_depth = previous->tips_[best_pairs[i]].smoothed_depth;

    tips_[i].smoothed_depth = maths::Smooth(
      previous_depth, depth,
      min_speed_distance, min_speed,
      max_speed_distance, max_speed);

  }

}

}  // namespace hand_extractor