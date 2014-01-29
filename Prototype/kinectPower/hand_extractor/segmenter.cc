#include "hand_extractor/segmenter.h"

#include <iostream>
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
const int kSimpleContourTolerance = 5;

}  // namespace

Segmenter::Segmenter(int hands_depth, int hands_depth_tolerance)
    : hands_depth_(hands_depth),
      hands_depth_tolerance_(hands_depth_tolerance),
      min_depth_(static_cast<unsigned short>(
          hands_depth - hands_depth_tolerance)),
      max_depth_(static_cast<unsigned short>(
          hands_depth + hands_depth_tolerance)) {
}

void Segmenter::SegmentHands(const cv::Mat& depth_mat,
                             std::vector<std::vector<cv::Point>>* contours,
                             cv::Mat* segmentation_mat) const {
  assert(depth_mat.type() == CV_16U);
  assert(contours);
  assert(contours->empty());
  assert(segmentation_mat);

  *segmentation_mat = cv::Mat::zeros(depth_mat.rows, depth_mat.cols, CV_8U);

  // Generate a matrix in which each pixel that is potentially a hand is "1" and
  // all other pixels are "0".
  cv::Mat hands_mask;
  ComputeHandsMask(depth_mat, &hands_mask);

  // Find the contour of each region in |hands_maks|.
  std::vector<std::vector<cv::Point> > complex_contours;
  cv::findContours(hands_mask, complex_contours,
                   CV_RETR_LIST,
                   CV_CHAIN_APPROX_SIMPLE);
  // TODO(fdoray): Find only the "parent" contours.

  // Value assigned to the pixels of the next contour found.
  unsigned char contour_pixel_value = 1;

  // For each distinct contour...
  for (size_t complex_contour_index = 0; 
       complex_contour_index < complex_contours.size();
       ++complex_contour_index) {
    const std::vector<cv::Point>& contour =
        complex_contours[complex_contour_index];

    // Handle the contour only if it's big enough.
    float area = maths::Area(contour);
    if (area < kMinContourArea)
      continue;

    // Simplify the contour.
    size_t simple_contour_index = contours->size();
    contours->push_back(std::vector<cv::Point>());
    cv::approxPolyDP(contour, contours->at(simple_contour_index),
                     kSimpleContourTolerance, true);

    // Remove the arm from the contour.
    // TODO(fdoray)

    // Draw the simplified contour.
    cv::drawContours(*segmentation_mat, *contours, simple_contour_index,
                     contour_pixel_value, image::kThickness1);

    // Pixel value for the next contour.
    ++contour_pixel_value;
  }
}

void Segmenter::ComputeHandsMask(const cv::Mat& depth_mat,
                                 cv::Mat* hands_mask) const {
  assert(hands_mask);
  assert(depth_mat.type() == CV_16U);

  *hands_mask = Mat(depth_mat.rows, depth_mat.cols, CV_8U);

  unsigned short const* depth_ptr =
    reinterpret_cast<unsigned short const*>(depth_mat.ptr());
  unsigned char* hands_ptr = hands_mask->ptr();

  for (size_t pixel_index = 0;
    pixel_index < depth_mat.total(); ++pixel_index) {

    if (PixelInDepthRange(*depth_ptr))
      *hands_ptr = 1;
    else
      *hands_ptr = 0;

    ++depth_ptr;
    ++hands_ptr;
  }
}

}  // namespace hand_extractor