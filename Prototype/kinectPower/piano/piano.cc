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

Piano::Piano() {
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
  std::vector<bool> notes_vector;
  notes_.GetCurrent(&notes_vector);

  for (size_t i = 0; i < notes_vector.size(); ++i) {
    notes[i] = notes_vector[i] ? 1 : 0;
  }
}

void Piano::QueryNiceImage(unsigned char* nice_image, size_t nice_image_size) {
  cv::Mat nice_mat;
  nice_image_.GetCurrent(&nice_mat);

  memcpy_s(nice_image, nice_image_size, nice_mat.ptr(), nice_mat.total() * 4);
}

void Piano::DrawPiano() {
  cv::Mat nice_image = Mat(depth_mat_.rows, depth_mat_.cols, CV_8UC4);
  unsigned char* nice_ptr = nice_image.ptr();

  // Generate a nice image from the depth information.
  kinect_wrapper::NiceImageFromDepthMat(depth_mat_, 2500, kPianoZ, nice_ptr,
                                        nice_image.total() * 4);

  // Draw the actual piano.
  DrawHorizontalLine(kPianoYMin, kPianoXMin, kPianoXMax, nice_ptr);
  DrawVerticalLine(kPianoXMin, kPianoYMin, kPianoYMax, nice_ptr);
  DrawVerticalLine(kPianoXMax, kPianoYMin, kPianoYMax, nice_ptr);
  DrawHorizontalLine(kPianoYMax, kPianoXMin, kPianoXMax, nice_ptr);

  for (int i = 1; i < kPianoNumNotes; ++i) {
    DrawVerticalLine(kPianoXMin + i*kPianoNoteWidth, kPianoYMin,
                     kPianoYMax, nice_ptr);
  }

  nice_image_.SetNext(nice_image);
}

void Piano::FindNotes() {
  std::vector<bool> notes(kPianoNumNotes);

  // Reset notes vector.
  for (int note = 0; note < kPianoNumNotes; ++note) {
    notes[note] = false;
  }

  // Find played notes.
  for (int row = kPianoYMin; row < kPianoYMax; ++row) {
    unsigned short* pixel = depth_ptr() + GetIndexOfPixel(kPianoXMin, row);

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

void Piano::DrawVerticalLine(int x, int ymin, int ymax, unsigned char* img) {
  int start_index = GetIndexOfPixel(x, ymin);
  int stop_index = GetIndexOfPixel(x, ymax);

  unsigned char* img_run = img + 4*start_index;
  unsigned char* img_stop = img + 4*stop_index;

  while (img_run < img_stop) {
    img_run[kRedIndex] = 0;
    img_run[kGreenIndex] = 255;
    img_run[kBlueIndex] = 0;

    img_run += 4*kinect_wrapper::kKinectDepthWidth;
  }
}


void Piano::DrawHorizontalLine(int y, int xmin, int xmax, unsigned char* img) {
  int start_index = GetIndexOfPixel(xmin, y);
  int stop_index = GetIndexOfPixel(xmax, y);

  unsigned char* img_run = img + 4*start_index;
  unsigned char* img_stop = img + 4*stop_index;

  while (img_run < img_stop) {
    img_run[kRedIndex] = 0;
    img_run[kGreenIndex] = 255;
    img_run[kBlueIndex] = 0;

    img_run += 4;
  }
}

}  // namespace piano