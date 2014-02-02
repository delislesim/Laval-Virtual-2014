#include "hand_extractor/finger_finder_convex.h"

#include <algorithm>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "algos/stable_matching.h"
#include "base/logging.h"
#include "hand_extractor/hand_2d_parameters.h"
#include "image/image_constants.h"
#include "maths/maths.h"

using namespace cv;

namespace hand_extractor {

namespace {

// Minimum distance between two fingertips ^2.
const int kTipReduceThresholdDistanceSquare = 196;

// Maximum distance to match two fingertips between the current and previous
// frame.
const int kTipMatchingThresholdDistanceSquare = 121;

const float kMaximumLifetime = 3;

const float kLifetimeErosion = 0.6f;

struct TipDescription {
  TipDescription()
      : tip_index(0),
        fold_before_index(0),
        fold_after_index(0),
        matched(false),
        reputation_score(0),
        lifetime(0) {}

  size_t tip_index;
  size_t fold_before_index;
  size_t fold_after_index;

  bool matched;
  float reputation_score;
  float lifetime;
};

float ReputationScore(int distance, float lifetime) {
  assert(distance >= 0);
  float inverse_distance = 50.0f / (50.0f + static_cast<float>(distance + 1));
  float score = inverse_distance * lifetime;
  return score;
}

struct TipDescriptionPositionSorter {
  TipDescriptionPositionSorter(std::vector<cv::Point> const* contour)
  : contour(contour) {}

  bool operator() (const TipDescription& a, const TipDescription& b) {
    return (*contour)[a.tip_index].y < (*contour)[b.tip_index].y;
  }

  std::vector<cv::Point> const* contour;
};

struct TipDescriptionReputationSorter {
  bool operator() (const TipDescription& a, const TipDescription& b) {
    return a.reputation_score > b.reputation_score;
  }
};

void ProcessTip(const std::vector<cv::Point>& contour,
                const TipDescription& tip_description,
                std::vector<TipDescription>* tips) {
  std::vector<cv::Point> defect_points;
  defect_points.push_back(contour[tip_description.tip_index]);
  defect_points.push_back(contour[tip_description.fold_before_index]);
  defect_points.push_back(contour[tip_description.fold_after_index]);

  const float kThresholdArea = 100.0;

  RotatedRect box = minAreaRect(defect_points);
  float area = box.size.height * box.size.width;

  if (area < kThresholdArea)
    return;

  tips->push_back(tip_description);
}


void ReduceTips(const std::vector<cv::Point>& contour,
                const std::vector<cv::Vec4i>& defects,
                std::vector<TipDescription>* tips) {
  assert(tips);
  assert(tips->empty());

  for (size_t i = 0; i < defects.size(); ++i) {
    // First tip
    TipDescription tip_description;
    tip_description.tip_index = defects[i].val[0];
    tip_description.fold_before_index =
        defects[(i + defects.size() - 1) % defects.size()].val[2];
    tip_description.fold_after_index = defects[i].val[2];

    size_t previous_tip_index =
        defects[(i + defects.size() - 1) % defects.size()].val[1];
    int square_distance = maths::DistanceSquare(
        contour[previous_tip_index],
        contour[tip_description.tip_index]
    );
    if (square_distance > kTipReduceThresholdDistanceSquare) {
      ProcessTip(contour, tip_description, tips);
    }

    // Second tip
    TipDescription second_tip_description;
    second_tip_description.tip_index = defects[i].val[1];
    second_tip_description.fold_before_index = defects[i].val[2];
    second_tip_description.fold_after_index = 
        defects[(i + 1) % defects.size()].val[2];

    ProcessTip(contour, second_tip_description, tips);
  }
}

}  // namespace

FingerFinderConvex::FingerFinderConvex() {
}

void FingerFinderConvex::FindFingers(const std::vector<cv::Point>& contour,
                               const unsigned char contour_pixel_value,
                               const cv::Mat& depth_mat,
                               const cv::Mat& segmentation_mat,
                               const Hand2dParameters* previous_hand_parameters,
                               Hand2dParameters* hand_parameters) const {
  // |previous_hand_parameters| can be NULL.
  assert(hand_parameters);

  // Compute a convex hull for the hand.
  std::vector<int> convex_hull;
  cv::convexHull(contour, convex_hull);

  // Find convexity defects.
  std::vector<Vec4i> convexity_defects;
  cv::convexityDefects(contour, convex_hull, convexity_defects);

  // Remove unwanted defects.
  std::vector<TipDescription> tips;
  ReduceTips(contour, convexity_defects, &tips);
  
  // Find the distance between the tips from last frame and from this frame.
  if (previous_hand_parameters) {
    algos::StableMatchingQueue fingers_potential_pairs;
    const Hand2dParameters::PotentialTipVector& previous_tips =
        previous_hand_parameters->GetPotentialTips();

    for (size_t current_index = 0; current_index < tips.size(); ++current_index) {
      for (size_t previous_index = 0; previous_index < previous_tips.size(); ++previous_index) {
        algos::StableMatchingPair potential_pair;
        potential_pair.left = current_index;
        potential_pair.right = previous_index;
        potential_pair.distance = maths::DistanceSquare(
            contour[tips[current_index].tip_index],
            previous_tips[previous_index].position
        );
        fingers_potential_pairs.push(potential_pair);
      }
    }

    std::vector<int> fingers_best_pairs;
    algos::StableMatching(tips.size(), previous_tips.size(),
                          kTipMatchingThresholdDistanceSquare,
                          &fingers_potential_pairs,
                          &fingers_best_pairs);

    for (size_t i = 0; i < tips.size(); ++i) {
      if (fingers_best_pairs[i] != -1) {
        float previous_lifetime = previous_tips[fingers_best_pairs[i]].lifetime;
        int distance = maths::DistanceSquare(
            contour[tips[i].tip_index],
            previous_tips[fingers_best_pairs[i]].position
        );
        tips[i].reputation_score = ReputationScore(distance, previous_lifetime);
        tips[i].lifetime = previous_lifetime * kLifetimeErosion;
      }
    }
  }

  // Sort the tips by position.
  TipDescriptionPositionSorter tip_position_sorter(&contour);
  std::sort(tips.begin(), tips.end(), tip_position_sorter);

  // Increase the lifetime and reputation of the highest tips.
  for (size_t i = 0; i < Hand2dParameters::JOINTS_COUNT && i < tips.size();
       ++i) {
    tips[i].lifetime += 1;
    tips[i].reputation_score += 0.5;
  }

  // Sort the tips by reputation.
  TipDescriptionReputationSorter tip_reputation_sorter;
  std::sort(tips.begin(), tips.end(), tip_reputation_sorter);

  // Return the best finger positions.
  size_t tip_data_index = 0;
  for (size_t joint_index = 0; joint_index < Hand2dParameters::JOINTS_COUNT;
       ++joint_index) {
    // No more fingers detected.
    if (tip_data_index >= tips.size())
      break;

    Hand2dParameters::HandJoint joint =
      static_cast<Hand2dParameters::HandJoint>(joint_index);
    cv::Point position = contour[tips[tip_data_index].tip_index];

    hand_parameters->SetJointPosition(joint,
                                      position,
                                      depth_mat.at<unsigned short>(position));

    ++tip_data_index;
  }

  // Store all the fingertips in the hand parameters structure.
  for (size_t i = 0; i < tips.size(); ++i) {
    Hand2dParameters::PotentialTip potential_tip;
    potential_tip.position = contour[tips[i].tip_index];
    potential_tip.lifetime =
        maths::Clamp(tips[i].lifetime, 0.0f, kMaximumLifetime);
    hand_parameters->PushPotentialTip(potential_tip);
  }

}

}  // namespace hand_extractor