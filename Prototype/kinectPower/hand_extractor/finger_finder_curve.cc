#include "hand_extractor/finger_finder_curve.h"

#include <algorithm>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "algos/stable_matching.h"
#include "algos/contour_walk.h"
#include "base/logging.h"
#include "hand_extractor/hand_2d_parameters.h"
#include "image/image_constants.h"
#include "maths/curve.h"
#include "maths/maths.h"

using namespace cv;

namespace hand_extractor {

namespace {

struct PotentialFingerTip {
  PotentialFingerTip() : num_points(0), selected(false) {
  }

  cv::Point position;
  int num_points;
  bool selected;

  bool operator<(const PotentialFingerTip& other) {
    return position.y < other.position.y;
  }
};

// Distance between considered points of the contour.
const double kWalkStep = 4;

// Multipliers for the filter.
const double kFilterMultipliers[] = {
    1.0, 0.9, 0.9, 0.6, 0.6, 0.6, 0.1
};
const double kSumMultipliers = 1.0 + 0.9 + 0.9 + 0.6 + 0.6 + 0.6 + 0.1;
const size_t kNumFilterMultipliers = 7;
const double kFilterThreshold = maths::kPi / 4.0;

// Count runs of red points.
const size_t kNumPreCount = 20;

double AverageAngle(size_t index, const std::vector<double>& point_curve) {
  double sum = 0.0;
  
  for (size_t i = 0; i < kNumFilterMultipliers; ++i) {
    sum += point_curve[maths::Previous(i, index, point_curve.size())]
                           * kFilterMultipliers[i];
    sum += point_curve[maths::Next(i, index, point_curve.size())]
                           * kFilterMultipliers[i];
  }
  
  double average = sum / (2 * kSumMultipliers);
  return average;
}

void CountDirection(size_t index, const std::vector<double>& point_curve, int* num_under, int* num_above, int* num_null) {
  for (size_t i = 0; i < kNumFilterMultipliers; ++i) {
    double val = point_curve[maths::Previous(i, index, point_curve.size())];
    if (val < 0.0)
      ++(*num_under);
    else if (val > 0.0)
      ++(*num_above);
    else
      ++(*num_null);
  }
  for (size_t i = 0; i < kNumFilterMultipliers; ++i) {
    double val = point_curve[maths::Next(i, index, point_curve.size())];
    if (val < 0.0)
      ++(*num_under);
    else if (val > 0.0)
      ++(*num_above);
    else
      ++(*num_null);
  }
}

}  // namespace

FingerFinderCurve::FingerFinderCurve() {
}

void FingerFinderCurve::FindFingers(const std::vector<cv::Point>& contour,
                                    const unsigned char contour_pixel_value,
                                    const cv::Mat& depth_mat,
                                    cv::Mat* segmentation_mat,
                                    const Hand2dParameters* previous_hand_parameters,
                                    Hand2dParameters* hand_parameters) const {
  // Walk around the contour to find points at regular intervals.
  std::vector<cv::Point> walk;
  algos::ContourWalk(kWalkStep, contour, &walk);

  // Not enough data...
  if (walk.size() < kNumFilterMultipliers || walk.size() < kNumPreCount)
    return;

  // Compute the curve of each point of the contour.
  std::vector<double> point_curve;
  for (size_t i = 0; i < walk.size(); ++i) {
    double curve = maths::Curve(i, walk);
    point_curve.push_back(curve);
  }

  // Filter on the curve values.
  std::vector<double> point_curve_filtered;
  for (size_t i = 0; i < point_curve.size(); ++i) {
    double average = AverageAngle(i, point_curve);
    if (abs(average) < kFilterThreshold) {
      point_curve_filtered.push_back(0.0);
    } else {
      point_curve_filtered.push_back(point_curve[i]);
    }
  }

  // Second-pass filter on curve values.
  std::vector<double> point_curve_filtered_two;
  for (size_t i = 0; i < point_curve_filtered.size(); ++i) {
    double curve = point_curve_filtered[i];

    if (curve == 0) {
      point_curve_filtered_two.push_back(0.0);
      continue;
    }

    int num_under = 0;
    int num_null = 0;
    int num_above = 0;
    CountDirection(i, point_curve_filtered, &num_under, &num_above, &num_null);

    if ((curve < 0 && num_under > 3 && num_above < 3) || (curve > 0 && num_above > 3 && num_under < 3))
      point_curve_filtered_two.push_back(curve);
    else
      point_curve_filtered_two.push_back(0.0);
  }

  // Find potential fingertips.
  int count_red = 0;
  int count_black = 0;
  int count_green = 0;
  std::vector<PotentialFingerTip> potential_fingertips;
  cv::Point highest_point = cv::Point(9999, 9999);

  // Prev-count for the first points of the walk.
  for (size_t i = point_curve_filtered_two.size() - kNumPreCount; i < point_curve_filtered.size(); ++i) {
    double curve = point_curve_filtered_two[i];
    if (curve <= 0) {
      if (curve > 0) {
        ++count_green;
      } else {
        ++count_black;
      }

      if (count_red > 0 && (count_green > 0 || count_black > 2)) {
        count_red = 0;
        count_black = 0;
        count_green = 0;
      }
    } else {
      if (count_red == 0) {
        count_green = 0;
        count_black = 0;
      }

      ++count_red;
      if (walk[i].y < highest_point.y)
        highest_point = walk[i];
    }
  }

  // Actual calculation.
  for (size_t i = 0; i < point_curve_filtered_two.size(); ++i) {
    double curve = point_curve_filtered_two[i];
    if (curve <= 0) {
      if (curve > 0) {
        ++count_green;
      } else {
        ++count_black;
      }

      if (count_red > 0 && (count_green > 0 || count_black > 2)) {
        PotentialFingerTip potential_fingertip;
        potential_fingertip.num_points = count_red;
        potential_fingertip.position = highest_point;
        potential_fingertips.push_back(potential_fingertip);

        count_red = 0;
        count_black = 0;
        count_green = 0;

        highest_point = cv::Point(9999, 9999);
      }
    } else {
      if (count_red == 0) {
        count_green = 0;
        count_black = 0;
      }

      ++count_red;
      if (walk[i].y < highest_point.y)
        highest_point = walk[i];
    }
  }

  /*
  // Draw the potential fingertips.
  for (size_t i = 0; i < potential_fingertips.size(); ++i) {
    cv::circle(*segmentation_mat, potential_fingertips[i].position, 1, 255, image::kThickness2);
  }
  */

  // Find corresponding pairs of fingertips (previous / current) in order to 
  // select all fingertips that are very close from previous ones.
  if (previous_hand_parameters != NULL) {

    algos::StableMatchingQueue stable_matching_queue;
    for (int i = 0; i < potential_fingertips.size(); ++i) {
      for (int j = 0; j < previous_hand_parameters->TipSize(); ++j) {
        algos::StableMatchingPair pair;
        pair.left = i;
        pair.right = j;
        pair.distance = maths::DistanceSquare(potential_fingertips[i].position, previous_hand_parameters->TipAtIndex(j).position);
        stable_matching_queue.push(pair);
      }
    }
    std::vector<int> best_pairs;
    algos::StableMatching(potential_fingertips.size(), previous_hand_parameters->TipSize(), 100,
                          &stable_matching_queue, &best_pairs);

    for (int i = 0; i < best_pairs.size(); ++i) {
      if (best_pairs[i] != -1) {
        Hand2dParameters::Tip tip;
        tip.position = potential_fingertips[i].position;
        tip.depth = depth_mat.at<unsigned short>(tip.position);
        potential_fingertips[i].selected = true;
        hand_parameters->PushTip(tip);

        // TODO(fdoray): Smooth here.
      }
    }
  }

  // Choose the remaining fingertips from the list of potential fingertips.
  std::sort(potential_fingertips.begin(), potential_fingertips.end());
  for (size_t i = 0; hand_parameters->TipSize() < 5 && i < potential_fingertips.size(); ++i) {
    if (potential_fingertips[i].selected)
      continue;

    // Compute distance from all existing fingertips.
    bool ok = true;
    for (size_t j = 0; j < hand_parameters->TipSize(); ++j) {
      int distance_square = maths::DistanceSquare(potential_fingertips[i].position, hand_parameters->TipAtIndex(j).position);
      if (distance_square < 121) {
        ok = false;
        break;
      }
    }

    // Add the fingertip if promising.
    if (ok) {
      Hand2dParameters::Tip tip;
      tip.position = potential_fingertips[i].position;
      tip.depth = depth_mat.at<unsigned short>(tip.position);
      hand_parameters->PushTip(tip);
    }
  }

  // Draw circles with colors indicating the curve.
  /*
  for (size_t i = 0; i < point_curve_filtered.size(); ++i) {
    if (point_curve_filtered_two[i] != 0) {
      unsigned char color = (point_curve_filtered_two[i] + maths::kPi) * 255.0 / (2*maths::kPi);
      cv::circle(*segmentation_mat, walk[i], 1, color, image::kThickness2);
    }
  }
  */
  std::cout << hand_parameters->TipSize() << std::endl;
}

}  // namespace hand_extractor