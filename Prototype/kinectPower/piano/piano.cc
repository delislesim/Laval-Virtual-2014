#include "piano/piano.h"

#include <cmath>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "base/logging.h"
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

const cv::Scalar kColors[] = {
  cv::Scalar(176, 23, 31), // indian red
  cv::Scalar(70, 130, 180), // steel blue
  cv::Scalar(0, 201, 87), // emerald green
  cv::Scalar(238, 201, 0), // gold 
  cv::Scalar(255, 127, 80), // coral
  cv::Scalar(255, 250, 250), // snow
  cv::Scalar(124, 252, 0), // lawn green
  cv::Scalar(255, 0, 255) // magenta
};
const int kNumColors = 8;

const cv::Scalar kFloodFillTolerance(5, 5, 5);

// Piano image
const int kPianoZ = 930;
const int kPianoZTolerance = 140;
const int kMinZ = kPianoZ - kPianoZTolerance;
const int kMaxZ = kPianoZ + kPianoZTolerance;

}  // namespace

Piano::Piano()
    : started_(false),
      hand_extractor_(kPianoZ, kPianoZTolerance) {
}

Piano::~Piano() {
}

void Piano::ObserveDepth(
      const cv::Mat& depth_mat,
      const kinect_wrapper::KinectSensorData& data) {
  // Call the hand extractor.
  cv::Mat segmentation_mat;
  std::vector<hand_extractor::Hand2dParameters> hand_parameters_tmp;
  hand_extractor_.ExtractHands(depth_mat, &segmentation_mat, &hand_parameters_tmp);
  hand_parameters_.SetNext(hand_parameters_tmp);

  // Generate an RGBA image from the computed information.
  cv::Mat image = cv::Mat::zeros(depth_mat.rows, depth_mat.cols, CV_8UC4);

  unsigned char* image_ptr = image.ptr();
  unsigned char* segmentation_ptr = segmentation_mat.ptr();
  unsigned short const* depth_ptr = reinterpret_cast<unsigned short const*>(depth_mat.ptr());
  
  for (size_t i = 0; i < image.total(); ++i) {
    if (*segmentation_ptr == 0) {
      // Depth.
      if (*depth_ptr < kMaxZ && *depth_ptr > kMinZ) {
        int normalized_depth = static_cast<int>(*depth_ptr - kMinZ) * 255;
        normalized_depth /= (kMaxZ - kMinZ);
        unsigned char char_normalized_depth = static_cast<unsigned char>(normalized_depth);

        image_ptr[image::kRedIndex] = char_normalized_depth;
        image_ptr[image::kGreenIndex] = char_normalized_depth;
        image_ptr[image::kBlueIndex] = char_normalized_depth;
        image_ptr[image::kAlphaIndex] = 255;

      } else {
        image_ptr[image::kRedIndex] = 0;
        image_ptr[image::kGreenIndex] = 0;
        image_ptr[image::kBlueIndex] = 0;
        image_ptr[image::kAlphaIndex] = 255;
      }

    } else {
      // Special colors.
      int color_index = *segmentation_ptr;
  
      if (color_index > 128) {
        image_ptr[image::kRedIndex] = 255 - (color_index - 127) * 2;
        image_ptr[image::kGreenIndex] = 0;
        image_ptr[image::kBlueIndex] = 0;
      } else if (color_index < 127) {
        image_ptr[image::kRedIndex] = 0;
        image_ptr[image::kGreenIndex] = (127 - color_index) * 2;
        image_ptr[image::kBlueIndex] = 0;
      }
      image_ptr[image::kAlphaIndex] = 255;
    }
    image_ptr += 4;
    segmentation_ptr += 1;
    depth_ptr += 1;
  }
  
  started_ = true;

  nice_image_.SetNext(image);
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  assert(nice_image);

  if (!started_)
    return;

  cv::Mat nice_mat;
  nice_image_.GetCurrent(&nice_mat);

  memcpy_s(nice_image, nice_image_size, nice_mat.ptr(), nice_mat.total() * 4);
}

void Piano::QueryHandParameters(std::vector<hand_extractor::Hand2dParameters>* hand_parameters) {
  assert(hand_parameters);

  if (!started_)
    return;

  hand_parameters_.GetCurrent(hand_parameters);
}

}  // namespace piano