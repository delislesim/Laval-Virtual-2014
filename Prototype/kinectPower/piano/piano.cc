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

const unsigned short kPlayDepth = 690;

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
const int kPianoZ = 690;
const int kPianoZTolerance = 140;
const int kMinZ = kPianoZ - kPianoZTolerance;
const int kMaxZ = kPianoZ + kPianoZTolerance;

const int kPianoXMin = 100;
const int kPianoYMin = 200;
const int kPianoHeight = 130;
const int kPianoNoteWidth = 30;
const int kPianoNumNotes = 14;
const int kPianoXMax = kPianoXMin + kPianoNumNotes * kPianoNoteWidth;
const int kPianoYMax = kPianoYMin + kPianoHeight;

const int ACTION_PLAY = 1;
const int ACTION_STAY = 2;
const int ACTION_REMOVE = 3;

}  // namespace

Piano::Piano()
    : started_(false),
      hand_extractor_(kPianoZ, kPianoZTolerance) {
  // Register the piano as an observer of the main Kinect sensor.
  kinect_wrapper::KinectWrapper::instance()->AddObserver(0, this);
}

Piano::~Piano() {
}

void Piano::ObserveDepth(
      const cv::Mat& depth_mat,
      const kinect_wrapper::KinectSensorState& sensor_state) {
  // Call the hand extractor.
  cv::Mat segmentation_mat;
  hand_extractor_.ExtractHands(depth_mat, &segmentation_mat);

  // Generate an RGBA image from the computed information.
  cv::Mat image = cv::Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);

  unsigned char* image_ptr = image.ptr();
  unsigned char* segmentation_ptr = segmentation_mat.ptr();
  unsigned short const* depth_ptr = reinterpret_cast<unsigned short const*>(depth_mat.ptr());

  for (size_t i = 0; i < image.total(); ++i) {
    if (*segmentation_ptr == 0) {

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
      int color_index = *segmentation_ptr % kNumColors;

      image_ptr[image::kRedIndex] = static_cast<unsigned char>(kColors[color_index].val[0]);
      image_ptr[image::kGreenIndex] = static_cast<unsigned char>(kColors[color_index].val[1]);
      image_ptr[image::kBlueIndex] = static_cast<unsigned char>(kColors[color_index].val[2]);
      image_ptr[image::kAlphaIndex] = 255;
    }

    image_ptr += 4;
    segmentation_ptr += 1;
    depth_ptr += 1;
  }

  started_ = true;

  nice_image_.SetNext(image);
}

void Piano::QueryNotes(unsigned char* notes, size_t notes_size) {
  assert(notes_size == kPianoNumNotes);

  if (!started_)
    return;

  std::vector<int> notes_vector;
  notes_.GetCurrent(&notes_vector);

  for (size_t i = 0; i < notes_vector.size(); ++i) {
    notes[i] = static_cast<unsigned char>(notes_vector[i]);
  }
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  assert(nice_image);

  if (!started_)
    return;

  cv::Mat nice_mat;
  nice_image_.GetCurrent(&nice_mat);

  memcpy_s(nice_image, nice_image_size, nice_mat.ptr(), nice_mat.total() * 4);
}

}  // namespace piano