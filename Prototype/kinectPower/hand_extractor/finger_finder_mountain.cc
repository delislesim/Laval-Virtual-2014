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

const int kMinDistanceBetweenTips = 11;
const int kMinSquareDistanceBetweenTips = 11*11;

bool IsMountain(const cv::Point& point,
                const cv::Point& before,
                const cv::Point& after) {
  return point.y <= before.y && point.y <= after.y;
}

void FindMountainsInContour(const std::vector<cv::Point>& contour,
                            std::vector<int>* mountains) {
  assert(mountains);
  assert(mountains->empty());

  for (size_t i = 0; i < contour.size(); ++i) {
    if (IsMountain(contour[i],
                   contour[maths::Previous(i, contour.size())],
                   contour[maths::Next(i, contour.size())])) {
      mountains->push_back(i);
    }
  }
}

struct TipPositionSorter {
  TipPositionSorter(std::vector<cv::Point> const* contour)
  : contour(contour) {}

  bool operator() (const int& a, const int& b) {
    return (*contour)[a].y < (*contour)[b].y;
  }

  std::vector<cv::Point> const* contour;
};

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

  // Find the mountains in the contour.
  std::vector<int> mountains;
  FindMountainsInContour(contour, &mountains);

  // Add mountains from the depth segmentation lines.
  // TODO(fdoray)

  // Deduce fingertips from the mountains.
  // TODO(fdoray)

  // Sort the fingertips by their y position.
  TipPositionSorter position_sorter(&contour);
  std::sort(mountains.begin(), mountains.end(), position_sorter);

  // Return the best fingertips guesses.
  for (size_t i = 0; i < mountains.size(); ++i) {
    cv::Point tip_position = contour[mountains[i]];

    // Make sure that the tip is not too close from previous tips.
    bool tip_ok = true;
    if (hand_parameters->TipSize() != 0) {
      for (int previous_tip_index = hand_parameters->TipSize() - 1;
           previous_tip_index >= 0; --previous_tip_index) {
    
        Hand2dParameters::Tip previous_tip =
            hand_parameters->TipAtIndex(previous_tip_index);
    
        if (previous_tip.position.y - tip_position.y > kMinDistanceBetweenTips)
          break;
    
        if (maths::DistanceSquare(tip_position, previous_tip.position) < kMinSquareDistanceBetweenTips) {
          tip_ok = false;
          break;
        }
      }
    }

    // Insert the tip.
    if (tip_ok) {
      Hand2dParameters::Tip tip;
      tip.position = tip_position;
      tip.depth = depth_mat.at<unsigned short>(tip_position);
      hand_parameters->PushTip(tip);
    }
  }

  // Smooth the result.
  // TODO(fdoray): Remove this.
  // hand_parameters->SmoothUsingPreviousParameters(previous_hand_parameters);
}

}  // namespace hand_extractor