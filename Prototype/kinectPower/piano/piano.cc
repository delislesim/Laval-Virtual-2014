#include "piano/piano.h"

#include <cmath>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "algos/maths.h"
#include "base/logging.h"
#include "finger_finder_thinning/segmenter.h"
#include "finger_finder_thinning/canny_contour.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"

using namespace cv;

namespace piano {

namespace {

const cv::Scalar kFloodFillTolerance(5, 5, 5);

// Piano image
const unsigned short kPianoZ = 930;
const unsigned short kPianoZTolerance = 140;
const unsigned short kMinZ = kPianoZ - kPianoZTolerance;
const unsigned short kMaxZ = kPianoZ + kPianoZTolerance;

}  // namespace

Piano::Piano()
    : started_(false),
      finger_finder_(kPianoZ, kPianoZTolerance) {
}

Piano::~Piano() {
}

void Piano::ObserveDepth(
      const cv::Mat& /*depth_mat*/,
      const kinect_wrapper::KinectSensorData& data) {
  
  // Extraire les données de profondeur et de couleur.
  cv::Mat depth_mat;
  cv::Mat color_mat;
  if (!data.QueryDepth(&depth_mat) || !data.QueryColor(&color_mat))
    return;

  // Find the fingers.
  cv::Mat image;
  finger_finder_.ObserveData(depth_mat, color_mat, &image);
  nice_image_.SetNext(image);

  //fingers_.SetNext(fingers);

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

/*
void Piano::QueryFingers(std::vector<finger_finder_thinning::FingerDescription>* fingers) {
  assert(fingers);

  if (!started_)
    return;

  fingers_.GetCurrent(fingers); 
}
*/

}  // namespace piano