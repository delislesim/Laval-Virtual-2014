#include "finger_finder/find_fingers_in_contour.h"

#include <opencv2/imgproc/imgproc.hpp>

#include "algos/contour_walk.h"
#include "finger_finder/contour_curve.h"
#include "finger_finder/contour_curve_filter.h"
#include "finger_finder_thinning/canny_contour.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "maths/maths.h"

namespace finger_finder {

namespace {

// Distance entre chaque point considéré sur le contour de la main.
const double kWalkStep = 4;

// Computes the curve of the points of the contour.
ContourCurve contour_curve_computer;

// Filter for the curve of contour points.
ContourCurveFilter contour_curve_filter;

void FindContourStartingAt(const cv::Point& start, cv::Mat* canny_mat, std::vector<cv::Point>* contour) {
  cv::Point current_point = start;

  for (;;) {
    canny_mat->at<unsigned char>(current_point) = 0;
    contour->push_back(current_point);

    cv::Point top_left(     current_point.x - 1,  current_point.y - 1);
    cv::Point top(          current_point.x,      current_point.y - 1);
    cv::Point top_right(    current_point.x + 1,  current_point.y - 1);
    cv::Point right(        current_point.x + 1,  current_point.y);
    cv::Point bottom_right( current_point.x + 1,  current_point.y + 1);
    cv::Point bottom(       current_point.x,      current_point.y + 1);
    cv::Point bottom_left(  current_point.x - 1,  current_point.y + 1);
    cv::Point left(         current_point.x - 1,  current_point.y);

    if (!image::OutOfBoundaries(*canny_mat, top_left) && canny_mat->at<unsigned char>(top_left) == 255)
      current_point = top_left;
    else if (!image::OutOfBoundaries(*canny_mat, top) && canny_mat->at<unsigned char>(top) == 255)
      current_point = top;
    else if (!image::OutOfBoundaries(*canny_mat, top_right) && canny_mat->at<unsigned char>(top_right) == 255)
      current_point = top_right;
    else if (!image::OutOfBoundaries(*canny_mat, right) && canny_mat->at<unsigned char>(right) == 255)
      current_point = right;
    else if (!image::OutOfBoundaries(*canny_mat, bottom_right) && canny_mat->at<unsigned char>(bottom_right) == 255)
      current_point = bottom_right;
    else if (!image::OutOfBoundaries(*canny_mat, bottom) && canny_mat->at<unsigned char>(bottom) == 255)
      current_point = bottom;
    else if (!image::OutOfBoundaries(*canny_mat, bottom_left) && canny_mat->at<unsigned char>(bottom_left) == 255)
      current_point = bottom_left;
    else if (!image::OutOfBoundaries(*canny_mat, left) && canny_mat->at<unsigned char>(left) == 255)
      current_point = left;
    else
      break;
  }
}

}  // namespace
void FindFingersInColor(const cv::Mat& color_mat,
                        const cv::Mat& depth_mat,
                        const std::vector<cv::Point>& depth_contour,
                        HandParameters* hand_parameters,
                        cv::Mat* nice_image) {
  // Extraire des contours de l'image couleur.
  cv::Mat canny_mat;
  finger_finder_thinning::CannyContour(color_mat, &canny_mat);

  // Supprimer les points qui ne sont pas à la bonne profondeur.
  kinect_wrapper::KinectSensor* sensor =
      kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0); // TODO(fdoray): Prendre le bon index.
  std::vector<cv::Point> depth_contour_color_coordinates;
  for (size_t i = 0; i < depth_contour.size(); ++i) {
    NUI_DEPTH_IMAGE_POINT depth_point;
    depth_point.x = depth_contour[i].x;
    depth_point.y = depth_contour[i].y;
    depth_point.depth = depth_mat.at<unsigned short>(depth_contour[i]);
    
    NUI_COLOR_IMAGE_POINT color_point;
    sensor->MapDepthPointToColorPoint(depth_point, &color_point);
    depth_contour_color_coordinates.push_back(cv::Point(color_point.x, color_point.y));      
  }

  cv::Mat dilated_depth_img = cv::Mat::zeros(color_mat.size(), CV_8U);
  {
    const cv::Point* points[1];
    points[0] = &depth_contour_color_coordinates[0];
    int num_points = depth_contour_color_coordinates.size();
    cv::fillPoly(dilated_depth_img, points, &num_points, 1, cv::Scalar(1));
  }
  cv::dilate(dilated_depth_img, dilated_depth_img, cv::Mat(), cv::Point(-1, -1), 5);

  unsigned char* dilated_run = dilated_depth_img.ptr();
  unsigned char* canny_run = canny_mat.ptr();
  for (size_t i = 0; i < dilated_depth_img.total(); ++i) {
    if (*dilated_run != 1)
      *canny_run = 0;
    ++dilated_run;
    ++canny_run;
  }

  // Trouver les points formant chaque contour dans l'image de couleur.
  std::vector<std::vector<cv::Point> > contours_color;

  for (int y = 0; y < color_mat.rows; ++y) {
    for (int x = 0; x < color_mat.cols; ++x) {
      // TODO(fdoray): Faire un effort pour trouver les plus longs contours possible.
      
      if (canny_mat.at<unsigned char>(cv::Point(x, y)) == 255) {
        contours_color.push_back(std::vector<cv::Point>());
        FindContourStartingAt(cv::Point(x, y), &canny_mat, &contours_color[contours_color.size() - 1]);
      }
    }
  }
  // TODO(fdoray): Faire des merge de contours.

  // == Trouver des courbures dans les courbes de l'image couleur. ==
  for (size_t i = 0; i < contours_color.size(); ++i) {

    // Walk around the contour to find points at regular intervals.
    std::vector<cv::Point> walk;
    algos::ContourWalk(kWalkStep, contours_color[i], &walk);

    // Not enough data...
    if (walk.size() < contour_curve_computer.GetMultipliersCount() ||
        walk.size() < contour_curve_filter.GetMultipliersCount())
      continue;
  
    // Compute the curve of each point of the contour.
    std::vector<double> raw_curve;
    contour_curve_computer.ComputeContourCurve(walk, &raw_curve);

    // Filter the contour points.
    std::vector<double> filtered_curve;
    contour_curve_filter.FilterContourCurve(raw_curve, &filtered_curve);

    filtered_curve = raw_curve;

    // Draw the curve of each points.
    // TODO(fdoray)
    for (size_t j = 0; j < walk.size(); ++j) {
      unsigned char normalized_curve =
          static_cast<unsigned char>(abs(filtered_curve[j]) * 255.0 / maths::kPi);
      cv::Scalar color;
      if (filtered_curve[j] > 0) {
        color = cv::Scalar(0, 0, normalized_curve, 255);
      } else {
        color = cv::Scalar(0, normalized_curve, 0, 255);
      }

      cv::circle(*nice_image, walk[j], 2, color, image::kThickness2);
    }
  }

}

}  // namespace finger_finder