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

cv::Point FindFingerTip(const cv::Point& guess_fingertip,
                        const cv::Mat& depth_mat) {
  return cv::Point(0, 0);
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
  for (size_t i = 0; i < Hand2dParameters::JOINTS_COUNT; ++i) {
    if (i >= mountains.size())
      break;

    Hand2dParameters::HandJoint joint =
        static_cast<Hand2dParameters::HandJoint>(i);

    hand_parameters->SetJointPosition(joint, contour[mountains[i]]);
  }
}

}  // namespace hand_extractor