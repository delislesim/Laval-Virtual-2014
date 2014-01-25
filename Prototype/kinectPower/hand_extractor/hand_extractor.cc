#include "hand_extractor/hand_extractor.h"

#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "base/logging.h"
#include "image/image_constants.h"
#include "maths/maths.h"

using namespace cv;

namespace hand_extractor {

namespace {

// Minimum area to consider that a contour is a hand contour, in pixels.
const float kMinContourArea = 1000.0;

// Distance tolerated between the real contour and the simplified contour.
const int kSimpleContourTolerance = 3;

}  // namespace

HandExtractor::HandExtractor(int hands_depth, int hands_depth_tolerance)
    : hands_depth_(hands_depth),
      hands_depth_tolerance_(hands_depth_tolerance) {
}

void HandExtractor::ExtractHands(const cv::Mat& depth_mat,
                                 std::vector<cv::Point>* hand_positions,
                                 cv::Mat* simplified_depth_mat) const {
  assert(hand_positions);
  assert(simplified_depth_mat);
  assert(depth_mat.type() == CV_16U);

  *simplified_depth_mat = cv::Mat::zeros(depth_mat.rows, depth_mat.cols, CV_8U);

  // Generate a matrix in which each pixel that is potentially a hand is "1" and
  // all other pixels are "0".
  cv::Mat hands_mask;
  ComputeHandsMask(depth_mat, &hands_mask);

  // Find the contour of each region in |hands_maks|.
  std::vector<std::vector<cv::Point> > contours;
  cv::findContours(hands_mask, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE);
    // TODO(fdoray): Find only the "parent" contours.

  // Value assigned to the pixels of the next contour found.
  unsigned char contour_pixel_value = 1;

  // For each distinct contour...
  for (size_t contour_index = 0; contour_index < contours.size();
       ++contour_index) {
    const std::vector<cv::Point>& contour = contours[contour_index];

    // Handle the contour only if it's big enough.
    float area = maths::Area(contours[contour_index]);
    if (area < kMinContourArea)
      continue;

    // Simplify the contour.
    std::vector<std::vector<cv::Point> > simple_contour;
    simple_contour.push_back(std::vector<cv::Point>());
    cv::approxPolyDP(contour, simple_contour[0], kSimpleContourTolerance, true);

    // Remove the "arm" part from the contour.
    // TODO(fdoray)

    // Find the center of the hand.
    cv::Point center(0, 0);
    hand_positions->push_back(center);

    // Draw the simplified contour.
    cv::drawContours(*simplified_depth_mat, simple_contour, 0, contour_pixel_value,
                     image::kThickness1);

    // Fill the hand.
    

    // Pixel value for the next contour.
    ++contour_pixel_value;
  }
}

void HandExtractor::ComputeHandsMask(const cv::Mat& depth_mat,
                                     cv::Mat* hands_mask) const {
  assert(hands_mask);
  assert(depth_mat.type() == CV_16U);

  const int min_depth = hands_depth_ - hands_depth_tolerance_;
  const int max_depth = hands_depth_ + hands_depth_tolerance_;

  *hands_mask = Mat(depth_mat.rows, depth_mat.cols, CV_8U);

  unsigned short const* depth_ptr =
      reinterpret_cast<unsigned short const*>(depth_mat.ptr());
  unsigned char* hands_ptr = hands_mask->ptr();

  for (size_t pixel_index = 0;
       pixel_index < depth_mat.total(); ++pixel_index) {

    if (*depth_ptr > min_depth && *depth_ptr < max_depth)
      *hands_ptr = 1;
    else
      *hands_ptr = 0;

    ++depth_ptr;
    ++hands_ptr;
  }
}

}  // namespace hand_extractor