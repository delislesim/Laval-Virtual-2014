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

// Distance between considered points of the contour.
const double kWalkStep = 4;

// Multipliers for the filter.
const double kFilterMultipliers[] = {
    1.0, 0.9, 0.9, 0.6, 0.6, 0.6, 0.1
};
const double kSumMultipliers = 1.0 + 0.9 + 0.9 + 0.6 + 0.6 + 0.6 + 0.1;
const size_t kNumFilterMultipliers = 7;
const double kFilterThreshold = maths::kPi / 4.0;

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
  if (walk.size() < kNumFilterMultipliers)
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
    double val = point_curve_filtered[i];

    if (val == 0) {
      point_curve_filtered_two.push_back(0.0);
      continue;
    }

    int num_under = 0;
    int num_null = 0;
    int num_above = 0;
    CountDirection(i, point_curve_filtered, &num_under, &num_above, &num_null);

    if ((val < 0 && num_under > 3 && num_above < 3) || (val > 0 && num_above > 3 && num_under < 3))
      point_curve_filtered_two.push_back(val);
    else
      point_curve_filtered_two.push_back(0.0);
  }

  // Draw circles with colors indicating the curve.
  for (size_t i = 0; i < point_curve_filtered.size(); ++i) {
    if (point_curve_filtered_two[i] != 0) {
      unsigned char color = (point_curve_filtered_two[i] + maths::kPi) * 255.0 / (2*maths::kPi);
      cv::circle(*segmentation_mat, walk[i], 1, color, image::kThickness2);
    }
  }

}

}  // namespace hand_extractor