#include "piano/piano.h"

#include <cmath>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>


#include "base/logging.h"
#include "finger_finder/segmenter.h"
#include "finger_finder_thinning/canny_contour.h"
#include "hand_extractor/hand_extractor.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"
#include "maths/maths.h"

using namespace cv;

namespace piano {

namespace {

const cv::Scalar kFloodFillTolerance(5, 5, 5);

// Piano image
const int kPianoZ = 930;
const int kPianoZTolerance = 140;
const int kMinZ = kPianoZ - kPianoZTolerance;
const int kMaxZ = kPianoZ + kPianoZTolerance;

}  // namespace

Piano::Piano()
    : started_(false),
      finger_finder_(kPianoZ, kPianoZTolerance),
      min_hands_depth_(kPianoZ - kPianoZTolerance),
      max_hands_depth_(kPianoZ + kPianoZTolerance) {
}

Piano::~Piano() {
}

void Piano::ObserveDepth(
      const cv::Mat& /*depth_mat*/,
      const kinect_wrapper::KinectSensorData& data) {
  
  
  // Find the fingers in the depth matrix.
  std::vector<finger_finder_thinning::FingerDescription> fingers;

  cv::Mat image;
  finger_finder_.FindFingers(data, &image, &fingers);
  nice_image_.SetNext(image);

  fingers_.SetNext(fingers);

        /*
  // Obtenir les données requises.
  cv::Mat depth_mat;
  cv::Mat color_mat;
  if (!data.QueryDepth(&depth_mat) || !data.QueryColor(&color_mat))
    return;
  
  assert(depth_mat.type() == CV_16U);
  assert(color_mat.type() == CV_8UC4);

  // Créer une image pour contenir le résultat.
  cv::Mat image(depth_mat.size(), CV_8UC4, cv::Scalar(0, 0, 0, 255));

  // Créer une image avec seulement les profondeurs à la hauteur du piano.
  cv::Mat hands_depth(depth_mat.size(), CV_8U);

  unsigned short* depth_run = reinterpret_cast<unsigned short*>(depth_mat.ptr());
  unsigned char* hands_run = hands_depth.ptr();

  for (size_t i = 0; i < depth_mat.total(); ++i) {
      if (*depth_run > min_hands_depth_ && *depth_run < max_hands_depth_) {
        *hands_run = static_cast<unsigned char>(*depth_run - min_hands_depth_) / 4;
      } else {
        if (*depth_run > 100 && *depth_run < 2000) {
          int boubou = 2;
        }
        *hands_run = 0;
      }
      ++depth_run;
      ++hands_run;
  }

  hands_depth_.SetNext(hands_depth);
  */


  /*
  // Extraire des zones potentielles de mains de l'image de profondeur.
  std::vector<std::vector<cv::Point> > depth_contours;
  finger_finder::Segmenter(depth_mat, min_hands_depth_, max_hands_depth_,
                           &depth_contours);

  // Convertir les contours trouvés dans l'image de profondeur en
  // coordonnées de l'image couleur.
  kinect_wrapper::KinectSensor* sensor =
      kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0);
  std::vector<std::vector<cv::Point> > depth_contours_color_coordinates;
  for (size_t i = 0; i < depth_contours.size(); ++i) {
    depth_contours_color_coordinates.push_back(std::vector<cv::Point>());
    for (size_t j = 0; j < depth_contours[i].size(); ++j) {
      NUI_DEPTH_IMAGE_POINT depth_point;
      depth_point.x = depth_contours[i][j].x;
      depth_point.y = depth_contours[i][j].y;
      depth_point.depth = depth_mat.at<unsigned short>(depth_contours[i][j]);
      
      NUI_COLOR_IMAGE_POINT color_point;
      sensor->MapDepthPointToColorPoint(depth_point, &color_point);
      depth_contours_color_coordinates[i].push_back(
          cv::Point(color_point.x, color_point.y));      
    }
  }

  // Dessiner les contours trouvés dans l'image de profondeur sur une
  // image binaire (en coordonnées couleur) et dilater le résultat.
  cv::Mat dilated_depth_contours_img = cv::Mat::zeros(color_mat.size(), CV_8U);
  for (size_t i = 0; i < depth_contours_color_coordinates.size(); ++i) {
    const cv::Point* points[1];
    points[0] = &depth_contours_color_coordinates[i][0];
    int num_points = depth_contours_color_coordinates[i].size();
    cv::fillPoly(dilated_depth_contours_img, points, &num_points, 1, cv::Scalar(1));
  }
  cv::dilate(dilated_depth_contours_img, dilated_depth_contours_img, cv::Mat(), cv::Point(-1, -1), 5); 

  // Extraire des contours de l'image de couleur.
  cv::Mat all_contours;
  finger_finder_thinning::CannyContour(color_mat, &all_contours);

  // Enlever tous les contours de couleur qui ne sont pas à la bonne profondeur.
  unsigned char* dilated_run = dilated_depth_contours_img.ptr();
  unsigned char* contours_run = all_contours.ptr();
  for (size_t i = 0; i < dilated_depth_contours_img.total(); ++i) {
    if (*dilated_run != 1)
      *contours_run = 0;
    ++dilated_run;
    ++contours_run;
  }

  // Mettre les contours de profondeur dilatés sur l'image résultat.
  cv::cvtColor(all_contours, image, CV_GRAY2RGBA);
  */

  // Enregistrer le résultat.
  nice_image_.SetNext(image);
  started_ = true;
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  assert(nice_image);

  if (!started_)
    return;

  cv::Mat nice_mat;
  nice_image_.GetCurrent(&nice_mat);

  memcpy_s(nice_image, nice_image_size, nice_mat.ptr(), nice_mat.total() * 4);
}

void Piano::QueryHandsDepth(unsigned char* hands_depth, size_t hands_depth_size) {
  assert(hands_depth);

  if (!started_)
    return;

  cv::Mat hands_mat;
  hands_depth_.GetCurrent(&hands_mat);

  memcpy_s(hands_depth, hands_depth_size, hands_mat.ptr(), hands_mat.total());
}

void Piano::QueryHandParameters(std::vector<finger_finder::HandParameters>* hand_parameters) {
  assert(hand_parameters);

  if (!started_)
    return;

  hand_parameters_.GetCurrent(hand_parameters);
}

void Piano::QueryFingers(std::vector<finger_finder_thinning::FingerDescription>* fingers) {
  assert(fingers);

  if (!started_)
    return;

  fingers_.GetCurrent(fingers); 
}

}  // namespace piano