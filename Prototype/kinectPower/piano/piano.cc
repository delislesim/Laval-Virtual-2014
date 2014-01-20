#include "piano/piano.h"

#include <iostream>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"

using namespace cv;

namespace piano {

namespace {

const unsigned short kPlayDepth = 690;

const int kBlueIndex = 0;
const int kGreenIndex = 1;
const int kRedIndex = 2;
const int kAlphaIndex = 3;

// Piano image
const int kPianoZ = /*690 */ 750;
const int kPianoXMin = 15;
const int kPianoYMin = 200;
const int kPianoHeight = 80;
const int kPianoNoteWidth = 20;
const int kPianoNumNotes = 20;
const int kPianoXMax = kPianoXMin + kPianoNumNotes * kPianoNoteWidth;
const int kPianoYMax = kPianoYMin + kPianoHeight;

int GetIndexOfPixel(int x, int y) {
  return kinect_wrapper::kKinectDepthWidth * y + x;
}

}  // namespace

Piano::Piano() : started_(false) {
  // Register the piano as an observer of the main Kinect sensor.
  kinect_wrapper::KinectWrapper::instance()->AddObserver(0, this);
}

Piano::~Piano() {
}

void Piano::ObserveDepth(
      const cv::Mat& depth_mat,
      const kinect_wrapper::KinectSensorState& sensor_state) {
  FindNotes(depth_mat);
  //DrawDepth(depth_mat);
  DrawMotion(depth_mat, sensor_state);
  started_ = true;
}

void Piano::QueryNotes(unsigned char* notes, size_t notes_size) {
  DCHECK(notes_size == kPianoNumNotes);

  if (!started_)
    return;

  std::vector<bool> notes_vector;
  notes_.GetCurrent(&notes_vector);

  for (size_t i = 0; i < notes_vector.size(); ++i) {
    notes[i] = notes_vector[i] ? 1 : 0;
  }
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  DCHECK(nice_image);

  if (!started_)
    return;

  cv::Mat nice_mat;
  nice_image_.GetCurrent(&nice_mat);

  memcpy_s(nice_image, nice_image_size, nice_mat.ptr(), nice_mat.total() * 4);
}

void Piano::DrawDepth(const cv::Mat& depth_mat) {
  cv::Mat image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);
  unsigned char* image_ptr = image.ptr();

  // Generate a nice image from the depth information.
  kinect_wrapper::NiceImageFromDepthMat(depth_mat, 2500, kPianoZ, image_ptr,
                                        image.total() * 4);

  // Draw the piano.
  DrawPiano(&image);

  nice_image_.SetNext(image);
}

void Piano::DrawMotion(const cv::Mat& depth_mat,
                       const kinect_wrapper::KinectSensorState& sensor_state) {
  cv::Mat past_mat;
  sensor_state.QueryDepth(4, &past_mat);

  cv::Mat image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);

  // Draw the motion.
  cv::Mat motion;
  cv::subtract(depth_mat, past_mat, motion, noArray(), CV_16S);

  const unsigned short* depth_ptr =
      reinterpret_cast<const unsigned short*>(depth_mat.ptr());
  const unsigned short* past_ptr =
      reinterpret_cast<const unsigned short*>(past_mat.ptr());
  short* motion_ptr = reinterpret_cast<short*>(motion.ptr());
  unsigned char* img_ptr = image.ptr();

  unsigned long long total = 0;

  for (size_t i = 0; i < motion.total(); ++i) {
    unsigned char normalized_motion = 0;

    if (abs(*motion_ptr) < 8) {
      normalized_motion = 0;
    } else if (abs(*motion_ptr) < 14) {
      normalized_motion =
          static_cast<unsigned char>(abs(*motion_ptr)) * 8;
    } else {
      normalized_motion =
          static_cast<unsigned char>(abs(*motion_ptr)) * 10;
    }

    if (*depth_ptr == 0 || *past_ptr == 0 || *depth_ptr > 1400) {
      img_ptr[kBlueIndex] = 0;
      img_ptr[kGreenIndex] = 0;
      img_ptr[kRedIndex] = 0;
    } else if (*motion_ptr > 0) {
      img_ptr[kBlueIndex] = normalized_motion;
      img_ptr[kGreenIndex] = 0;
      img_ptr[kRedIndex] = 0;
    } else {
      img_ptr[kBlueIndex] = normalized_motion;
      img_ptr[kGreenIndex] = normalized_motion;
      img_ptr[kRedIndex] = normalized_motion;
    }

    ++depth_ptr;
    ++past_ptr;
    ++motion_ptr;
    img_ptr += 4;
  }

  // Draw the piano.
  DrawPiano(&image);

  nice_image_.SetNext(image);
}

void Piano::DrawPiano(cv::Mat* image) {
  DrawHorizontalLine(kPianoYMin, kPianoXMin, kPianoXMax, image);
  DrawVerticalLine(kPianoXMin, kPianoYMin, kPianoYMax, image);
  DrawVerticalLine(kPianoXMax, kPianoYMin, kPianoYMax, image);
  DrawHorizontalLine(kPianoYMax, kPianoXMin, kPianoXMax, image);

  for (int i = 1; i < kPianoNumNotes; ++i) {
    DrawVerticalLine(kPianoXMin + i*kPianoNoteWidth, kPianoYMin,
                     kPianoYMax, image);
  }
}

void Piano::FindNotes(const cv::Mat& depth_mat) {
  std::vector<bool> notes(kPianoNumNotes);

  // Reset notes vector.
  for (int note = 0; note < kPianoNumNotes; ++note) {
    notes[note] = false;
  }

  // Find played notes.
  for (int row = kPianoYMin; row < kPianoYMax; ++row) {
    unsigned short const* pixel = reinterpret_cast<unsigned short const*>(
        depth_mat.ptr()) + GetIndexOfPixel(kPianoXMin, row);

    for (int note = 0; note < kPianoNumNotes; ++note) {
      for (int col = kPianoXMin + note * kPianoNoteWidth;
           col < kPianoXMin + (note + 1) * kPianoNoteWidth; ++col) {
        
        if (*pixel < kPianoZ && *pixel > 0)
          notes[note] = true;

        ++pixel;
      }
    }
  }

  notes_.SetNext(notes);
}

void Piano::DrawVerticalLine(int x, int ymin, int ymax, cv::Mat* image) {
  unsigned char* img_ptr = image->ptr();

  int start_index = GetIndexOfPixel(x, ymin);
  int stop_index = GetIndexOfPixel(x, ymax);

  unsigned char* img_run = img_ptr + 4*start_index;
  unsigned char* img_stop = img_ptr + 4*stop_index;

  while (img_run < img_stop) {
    img_run[kRedIndex] = 0;
    img_run[kGreenIndex] = 255;
    img_run[kBlueIndex] = 0;

    img_run += 4*kinect_wrapper::kKinectDepthWidth;
  }
}


void Piano::DrawHorizontalLine(int y, int xmin, int xmax, cv::Mat* image) {
  unsigned char* img_ptr = image->ptr();

  int start_index = GetIndexOfPixel(xmin, y);
  int stop_index = GetIndexOfPixel(xmax, y);

  unsigned char* img_run = img_ptr + 4*start_index;
  unsigned char* img_stop = img_ptr + 4*stop_index;

  while (img_run < img_stop) {
    img_run[kRedIndex] = 0;
    img_run[kGreenIndex] = 255;
    img_run[kBlueIndex] = 0;

    img_run += 4;
  }
}

}  // namespace piano