#include "finger_finder_thinning/segmenter.h"

#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "kinect_wrapper/constants.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {

// Minimum area to consider that a contour is a hand contour, in pixels.
const float kMinContourArea = 1000.0;

// Distance tolerated between the real contour and the simplified contour.
const int kSimpleContourTolerance = 1;

bool PixelInDepthRange(unsigned short depth, int min_depth, int max_depth) {
  return depth >= min_depth && depth <= max_depth;
}

// Calcule une matrice dans laquelle les pixels de |depth_max| dont la
// profondeur est entre |min_depth| et |max_depth| sont à 1 et tous les
// autres sont à 0.
void ComputeHandsMask(const cv::Mat& depth_mat, int min_depth, int max_depth,
                      cv::Mat* hands_mask) {
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols == static_cast<int>(kinect_wrapper::kKinectDepthWidth));
  assert(depth_mat.rows == static_cast<int>(kinect_wrapper::kKinectDepthHeight));
  assert(hands_mask);

  *hands_mask = cv::Mat(depth_mat.rows, depth_mat.cols, CV_8U);

  unsigned short const* depth_ptr =
      reinterpret_cast<unsigned short const*>(depth_mat.ptr());
  unsigned char* hands_ptr = hands_mask->ptr();

  for (size_t pixel_index = 0;
       pixel_index < depth_mat.total(); ++pixel_index) {

    if (PixelInDepthRange(*depth_ptr, min_depth, max_depth))
      *hands_ptr = 1;
    else
      *hands_ptr = 0;

    ++depth_ptr;
    ++hands_ptr;
  }
}

}  // namespace

void Segmenter(const cv::Mat& depth_mat, int min_depth, int max_depth,
               std::vector<std::vector<cv::Point>>* contours) {
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols == static_cast<int>(kinect_wrapper::kKinectDepthWidth));
  assert(depth_mat.rows == static_cast<int>(kinect_wrapper::kKinectDepthHeight));
  assert(contours);
  assert(contours->empty());

  // Calculer une matrice dans laquelle tous les pixels qui font partie d'une
  // main sont à 1 et les autres à 0.
  cv::Mat hands_mask;
  ComputeHandsMask(depth_mat, min_depth, max_depth, &hands_mask);

  // Trouver le contour de chaque region de pixels à 1 dans |hands_mask|.
  std::vector<std::vector<cv::Point> > complex_contours;
  cv::findContours(hands_mask, complex_contours,
                   CV_RETR_LIST,
                   CV_CHAIN_APPROX_SIMPLE);
  // TODO(fdoray): Find only the "parent" contours.

  // Pour chaque contour trouvé...
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
  }
  
}


}  // namespace finger_finder