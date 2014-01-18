#include "piano/piano.h"

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
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
const int kPianoZ = 690;
const int kPianoXMin = 15;
const int kPianoYMin = 200;
const int kPianoHeight = 80;
const int kPianoNoteWidth = 25;
const int kPianoNbNotes = 20;

int GetIndexOfPixel(int x, int y) {
  return kinect_wrapper::kKinectDepthWidth * y + x;
}

}  // namespace

Piano::Piano() {
}

Piano::~Piano() {
}

void Piano::LoadDepthImage(Mat depth_mat) {
  depth_mat_ = depth_mat;
  FindNotes();
  DrawPiano();
}

void Piano::QueryNotes(bool* notes, size_t notes_size) {
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  memcpy_s(nice_image, nice_image_size, nice_ptr(), nice_image_.total() * 4);
}

void Piano::DrawPiano() {
  nice_image_ = Mat(depth_mat_.rows, depth_mat_.cols, CV_8UC4);

  // Generate a nice image from the depth information.
  kinect_wrapper::NiceImageFromDepthMat(depth_mat_, 2500, kPianoZ, nice_ptr(),
                                        nice_image_.total() * 4);

  // Draw the actual piano.
  const int kPianoXMax = kPianoXMin + kPianoNbNotes * kPianoNoteWidth;
  const int kPianoYMax = kPianoYMin + kPianoHeight;

  DrawHorizontalLine(kPianoYMin, kPianoXMin, kPianoXMax);
  DrawVerticalLine(kPianoXMin, kPianoYMin, kPianoYMax);
  DrawVerticalLine(kPianoXMax, kPianoYMin, kPianoYMax);
  DrawHorizontalLine(kPianoYMax, kPianoXMin, kPianoXMax);

  for (int i = 1; i < kPianoNbNotes; ++i) {
    DrawVerticalLine(kPianoXMin + i*kPianoNoteWidth, kPianoYMin, kPianoYMax);
  }
}

void Piano::FindNotes() {
  
}

void Piano::DrawVerticalLine(int x, int ymin, int ymax) {
  int start_index = GetIndexOfPixel(x, ymin);
  int stop_index = GetIndexOfPixel(x, ymax);

  unsigned char* img = nice_ptr() + 4*start_index;
  unsigned char* img_stop = nice_ptr() + 4*stop_index;

  while (img < img_stop) {
    img[kRedIndex] = 0;
    img[kGreenIndex] = 255;
    img[kBlueIndex] = 0;

    img += 4*kinect_wrapper::kKinectDepthWidth;
  }
}


void Piano::DrawHorizontalLine(int y, int xmin, int xmax) {
  int start_index = GetIndexOfPixel(xmin, y);
  int stop_index = GetIndexOfPixel(xmax, y);

  unsigned char* img = nice_ptr() + 4*start_index;
  unsigned char* img_stop = nice_ptr() + 4*stop_index;

  while (img < img_stop) {
    img[kRedIndex] = 0;
    img[kGreenIndex] = 255;
    img[kBlueIndex] = 0;

    img += 4;
  }
}

}  // namespace piano