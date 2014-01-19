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
const int kPianoNoteWidth = 20;
const int kPianoNumNotes = 20;
const int kPianoXMax = kPianoXMin + kPianoNumNotes * kPianoNoteWidth;
const int kPianoYMax = kPianoYMin + kPianoHeight;

int GetIndexOfPixel(int x, int y) {
  return kinect_wrapper::kKinectDepthWidth * y + x;
}

}  // namespace

Piano::Piano() : notes_(kPianoNumNotes) {
}

Piano::~Piano() {
}

void Piano::LoadDepthImage(Mat depth_mat) {
  depth_mat_ = depth_mat;
  FindNotes();
  DrawPiano();
}

void Piano::QueryNotes(unsigned char* notes, size_t notes_size) {
  DCHECK(notes_size == kPianoNumNotes);
  for (size_t i = 0; i < notes_.size(); ++i) {
    notes[i] = notes_[i] ? 1 : 0;
  }
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
  DrawHorizontalLine(kPianoYMin, kPianoXMin, kPianoXMax);
  DrawVerticalLine(kPianoXMin, kPianoYMin, kPianoYMax);
  DrawVerticalLine(kPianoXMax, kPianoYMin, kPianoYMax);
  DrawHorizontalLine(kPianoYMax, kPianoXMin, kPianoXMax);

  for (int i = 1; i < kPianoNumNotes; ++i) {
    DrawVerticalLine(kPianoXMin + i*kPianoNoteWidth, kPianoYMin, kPianoYMax);
  }
}

void Piano::FindNotes() {
  // Reset notes vector.
  for (int note = 0; note < kPianoNumNotes; ++note) {
    notes_[note] = false;
  }

  // Find played notes.
  for (int row = kPianoYMin; row < kPianoYMax; ++row) {
    unsigned short* pixel = depth_ptr() + GetIndexOfPixel(kPianoXMin, row);

    for (int note = 0; note < kPianoNumNotes; ++note) {
      for (int col = kPianoXMin + note * kPianoNoteWidth;
           col < kPianoXMin + (note + 1) * kPianoNoteWidth; ++col) {
        
        if (*pixel < kPianoZ && *pixel > 0)
          notes_[note] = true;

        ++pixel;
      }
    }
  }
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