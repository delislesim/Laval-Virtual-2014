#include "finger_finder/find_fingers_in_contour.h"

#include "algos/contour_walk.h"
#include "finger_finder/contour_curve.h"
#include "finger_finder/contour_curve_filter.h"
#include "image/image_constants.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {

// Distance entre chaque point considéré sur le contour de la main.
const double kWalkStep = 4;

// Computes the curve of the points of the contour.
ContourCurve contour_curve_computer;

// Filter for the curve of contour points.
ContourCurveFilter contour_curve_filter;

}  // namespace

void FindFingersInContour(const cv::Mat& depth_mat,
                          const std::vector<cv::Point>& contour,
                          HandParameters* hand_parameters,
                          cv::Mat* nice_image) {
  assert(nice_image);
  assert(nice_image->size() == depth_mat.size());
  assert(nice_image->type() == CV_8UC4);

  // Walk around the contour to find points at regular intervals.
  std::vector<cv::Point> walk;
  algos::ContourWalk(kWalkStep, contour, &walk);

  // Not enough data...
  if (walk.size() < contour_curve_computer.GetMultipliersCount() ||
      walk.size() < contour_curve_filter.GetMultipliersCount())
    return;

  // Compute the curve of each point of the contour.
  std::vector<double> raw_curve;
  contour_curve_computer.ComputeContourCurve(walk, &raw_curve);

  // Filter the contour points.
  std::vector<double> filtered_curve;
  contour_curve_filter.FilterContourCurve(raw_curve, &filtered_curve);

  // Draw the curve of each points.
  // TODO(fdoray)
  for (size_t i = 0; i < walk.size(); ++i) {
    unsigned char normalized_curve =
        static_cast<unsigned char>(abs(filtered_curve[i]) * 255.0 / maths::kPi);
    cv::Scalar color;
    if (filtered_curve[i] > 0) {
      color = cv::Scalar(0, 0, normalized_curve, 255);
    } else {
      color = cv::Scalar(0, normalized_curve, 0, 255);
    }

    cv::circle(*nice_image, walk[i], 2, color, image::kThickness2);
  }


}

}  // namespace finger_finder