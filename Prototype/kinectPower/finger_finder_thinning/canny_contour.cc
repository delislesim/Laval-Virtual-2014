#include "finger_finder_thinning/canny_contour.h"

#include <opencv2/imgproc/imgproc.hpp>

namespace finger_finder_thinning {

namespace {

const double kMinThreshold = 30.0;
const double kMaxThreshold = 50.0;

}  // namespace

void CannyContour(const cv::Mat& color_mat,
                  cv::Mat* canny_contour) {
  assert(canny_contour);
  assert(color_mat.type() == CV_8UC4);

  // Convert image to grayscale for edge detection
  cv::Mat grayscale_color_mat;
  cv::cvtColor(color_mat, grayscale_color_mat, CV_RGBA2GRAY);
  // Remove noise
  cv::blur(grayscale_color_mat, grayscale_color_mat, cv::Size(3,3));

  // Find edges in image
  cv::Canny(grayscale_color_mat, *canny_contour, kMinThreshold, kMaxThreshold);
}

}  // namespace finger_finder_thinning