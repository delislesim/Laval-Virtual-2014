#include "piano/piano.h"

#include <cmath>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

#include "base/logging.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"
#include "maths/maths.h"

#include "hand_extractor/hand_extractor.h"

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

const int kPixelsToPlay = 150 /*250*/;
const int kPixelsToRemove = 75;

const int kMinFingerDepth = 20;
const int kMaxFingerAngle = 60;

const int ACTION_PLAY = 1;
const int ACTION_STAY = 2;
const int ACTION_REMOVE = 3;

int GetIndexOfPixel(int x, int y) {
  return kinect_wrapper::kKinectDepthWidth * y + x;
}

bool IsLeft(cv::Point line1, cv::Point line2, cv::Point point) {
  return ((line2.x - line1.x)*(point.y - line1.y) - (line2.y - line1.y)*(point.x - line1.x)) > 0;
}

bool IsBlack(const cv::Vec3i& color) {
  return color[0] == 0 && color[1] == 0 && color[2] == 0;
}

double CalculateTilt(double m11, double m20, double m02) {
  double diff = m20 - m02;
  if (diff == 0) {
    if (m11 == 0)
      return 0;
    else if (m11 > 0)
      return 45;
    else  // m11 < 0
      return -45;
  }
  
  double theta = 0.5 * std::atan2(2*m11, diff);
  int tilt = (int)round(maths::RadToDegrees(theta));
  
  if ((diff > 0) && (m11 == 0))
    return 0;
  else if ((diff < 0) && (m11 == 0))
    return -90;
  else if ((diff > 0) && (m11 > 0))  // 0 to 45 degrees
    return tilt;
  else if ((diff > 0) && (m11 < 0))  // -45 to 0
    return (180 + tilt);   // change to counter-clockwise angle
  else if ((diff < 0) && (m11 > 0))   // 45 to 90
    return tilt;
  else if ((diff < 0) && (m11 < 0))   // -90 to -45
    return (180 + tilt);  // change to counter-clockwise angle

  NOTREACHED();
  return 0;
}

void ReduceTips(const std::vector<cv::Point>& contour, 
                const std::vector<cv::Vec4i>& defects,
                std::vector<cv::Point>* tips,
                std::vector<cv::Point>* folds) {
  assert(tips);
  assert(folds);
  assert(tips->empty());
  assert(folds->empty());

  bool found_first_fold = false;

  for (size_t i = 0; i < defects.size(); ++i) {
    std::vector<cv::Point> defect_points;
    defect_points.push_back(contour[defects[i].val[0]]);
    defect_points.push_back(contour[defects[i].val[1]]);
    defect_points.push_back(contour[defects[i].val[2]]);

    const float kThresholdArea = 100.0;

    RotatedRect box = minAreaRect(defect_points);
    float area = box.size.height * box.size.width;

 
    if (area < kThresholdArea)
      continue;
  
    if (!found_first_fold) {
      tips->push_back(defect_points[0]);
      found_first_fold = true;
    }

    tips->push_back(defect_points[1]);
    folds->push_back(defect_points[2]);
  }
}   

void DrawConvexityDefects(const std::vector<cv::Point> contour,
                          const std::vector<Vec4i>& convexity_defects,
                          cv::Mat* image) {
  for (size_t i = 0; i < convexity_defects.size(); ++i) {
    cv::circle(*image, contour[convexity_defects[i].val[0]], 2, image::kGreen);
    cv::circle(*image, contour[convexity_defects[i].val[1]], 2, image::kGreen);
    cv::circle(*image, contour[convexity_defects[i].val[2]], 2, image::kRed);
  }
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
  cv::Mat fingers_image;
  DrawFingers(depth_mat, &fingers_image);
  FindNotes(depth_mat, fingers_image, sensor_state);


  hand_extractor::HandExtractor extractor(kPianoZ, kPianoZTolerance);
  std::vector<cv::Point> hand_positions;
  cv::Mat simplified_depth_mat;
  extractor.ExtractHands(depth_mat, &hand_positions, &simplified_depth_mat);

  cv::Mat image = cv::Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);

  unsigned char* image_ptr = image.ptr();
  unsigned char* simplified_depth_ptr = simplified_depth_mat.ptr();

  for (size_t i = 0; i < image.total(); ++i) {
    if (*simplified_depth_ptr == 0) {
      image_ptr[image::kRedIndex] = 0;
      image_ptr[image::kGreenIndex] = 0;
      image_ptr[image::kBlueIndex] = 0;
      image_ptr[image::kAlphaIndex] = 255;
    } else {
      int color_index = *simplified_depth_ptr % kNumColors;

      image_ptr[image::kRedIndex] = kColors[color_index].val[0];
      image_ptr[image::kGreenIndex] = kColors[color_index].val[1];
      image_ptr[image::kBlueIndex] = kColors[color_index].val[2];
      image_ptr[image::kAlphaIndex] = 255;
    }

    image_ptr += 4;
    simplified_depth_ptr += 1;
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

void Piano::DrawFingers(const cv::Mat& depth_mat, cv::Mat* rgba_image) {
  assert(rgba_image);

  cv::Mat image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC3);
  unsigned char* image_ptr = image.ptr();
  unsigned short const* depth_ptr = reinterpret_cast<unsigned short const*>(depth_mat.ptr());

  cv::Mat simple_hand = cv::Mat(depth_mat.rows, depth_mat.cols, CV_8U);
  unsigned char* simple_hand_ptr = simple_hand.ptr();

  // Draw the initial depth image.
  for (int y = 0; y < image.rows; ++y) {
    for (int x = 0; x < image.cols; ++x) {
      
      if (*depth_ptr > kMinZ && *depth_ptr < kMaxZ && y > 100 && x > 100 && x < 500) {

        unsigned int normalized_depth = (((unsigned int)*depth_ptr) - kMinZ) * 255;
        normalized_depth /= (kMaxZ - kMinZ);

        image_ptr[image::kRedIndex] = (unsigned char)normalized_depth;
        image_ptr[image::kGreenIndex] = (unsigned char)normalized_depth;
        image_ptr[image::kBlueIndex] = (unsigned char)normalized_depth;

        *simple_hand_ptr = 1;
      }
      else {
        image_ptr[image::kRedIndex] = 0;
        image_ptr[image::kGreenIndex] = 0;
        image_ptr[image::kBlueIndex] = 0;

        *simple_hand_ptr = 0;
      }

      image_ptr += image.channels();
      ++depth_ptr;
      ++simple_hand_ptr;
    }
  }

  // Find contours.
  std::vector<std::vector<cv::Point> > contours;
  cv::findContours(simple_hand, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE);

  for (size_t contour_index = 0; contour_index < contours.size(); ++contour_index) {
    const std::vector<cv::Point>& contour = contours[contour_index];

    // Handle the contour only if it's big enough.
    float area = maths::Area(contours[contour_index]);

    // Minimum area to consider that a contour is a hand contour.
    const float kMinContourArea = 1000.0;

    if (area < kMinContourArea)
      continue;

    // Simplify the contour.
    std::vector<cv::Point> simple_contour;
    cv::approxPolyDP(contour, simple_contour, 3 /* epsilon */, true);

    // Draw the contour.
    std::vector<std::vector<cv::Point> > simple_contours;
    simple_contours.push_back(simple_contour);
    cv::drawContours(image, simple_contours, 0, image::kBlue, image::kThickness1);
    //cv::drawContours(image, contours, contour_index, image::kBlue, image::kThickness1);

    // Compute a convex hull for the hand.
    std::vector<int> convex_hull;
    cv::convexHull(contour, convex_hull);

    // Find convexity defects.
    std::vector<Vec4i> convexity_defects;
    cv::convexityDefects(contour, convex_hull, convexity_defects);

    //DrawConvexityDefects(contour, convexity_defects, &image);

    // Find the moments of the hand.
    cv::Moments hand_moments = cv::moments(contour);
    cv::Point center_of_gravity = cv::Point(static_cast<int>(round(hand_moments.m10/hand_moments.m00)),
                                            static_cast<int>(round(hand_moments.m01/hand_moments.m00)));
    // double tilt = CalculateTilt(hand_moments.m11, hand_moments.m20, hand_moments.m02);

    // Draw the center of gravity.
    cv::circle(image, center_of_gravity, 5, Scalar(255, 0, 0));

    // Remove unwanted defects.
    std::vector<cv::Point> tips;
    std::vector<cv::Point> folds;
    ReduceTips(contour, convexity_defects, &tips, &folds);

    // Draw the tips and folds.
    /*
    for (size_t i = 0; i < tips.size(); ++i)
      cv::circle(image, tips[i], 5, Scalar(0, 255, 0));
    for (size_t i = 0; i < folds.size(); ++i)
      cv::circle(image, folds[i], 5, Scalar(0, 0, 255));
    */
    
    if (folds.size() > 1) {
      for (size_t i = 0; i < folds.size(); ++i) {

        cv::Point line1(folds[i]);
        cv::Point line2(folds[(i + 1) % folds.size()]);
        cv::Point parallel(line2.x - line1.x,
          line2.y - line1.y);

        // Draw a line to separate the finger from the rest of the hand.
        cv::line(image, line1, line2, image::kBlue, image::kThickness1);

        // Compute a vector perpendicular to the separation line and that
        // points toward the finger tip.
        cv::Point tip = tips[(i + 1) % folds.size()];
        cv::Point line_toward_tip(tip.x - line2.x,
          tip.y - line2.y);

        cv::Point perp(-parallel.y, parallel.x);

        if (perp.x == 0 && perp.y == 0)
          continue;  // Avoid division by 0.
        double norm_perp = norm(perp);
        perp = cv::Point(static_cast<int>(perp.x * 2 / norm_perp),
          static_cast<int>(perp.y * 2 / norm_perp));

        // Fill the finger.
        cv::Point fill_point(line1 + cv::Point(parallel.x / 2, parallel.y / 2)
          + perp);
        Vec3b fill_point_color = image.at<Vec3b>(fill_point);
        if (IsBlack(fill_point_color)) {
          fill_point = (line1 + cv::Point(parallel.x / 2, parallel.y / 2)
            - perp);
          fill_point_color = image.at<Vec3b>(fill_point);
        }

        if (!IsBlack(fill_point_color)) {
          cv::floodFill(image, fill_point, kColors[i % kNumColors], 0,
                        kFloodFillTolerance, kFloodFillTolerance);
        }

        // Draw the flood fill seed point.
        //cv::circle(image, fill_point, 4, cv::Scalar(255, 0, 255), 3);
      }
    }
  }

  // Add an alpha channel to the image...
  image::RgbImageToRgbaImage(image, rgba_image);

  // Draw the piano.
  DrawPiano(rgba_image);
}

void Piano::DrawDepth(const cv::Mat& depth_mat, cv::Mat* image) {
  *image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);
  unsigned char* image_ptr = image->ptr();

  // Generate a nice image from the depth information.
  kinect_wrapper::NiceImageFromDepthMat(depth_mat, kMaxZ, kMinZ, kPianoZ,
                                        image_ptr,
                                        image->total() * image->channels());

  // Draw the piano.
  DrawPiano(image);
}

void Piano::DrawMotion(const cv::Mat& depth_mat,
                       const kinect_wrapper::KinectSensorState& sensor_state,
                       cv::Mat* image) {
  cv::Mat past_mat;
  sensor_state.QueryDepth(8, &past_mat);

  *image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);

  // Draw the motion.
  cv::Mat motion;
  cv::subtract(depth_mat, past_mat, motion, noArray(), CV_16S);

  const unsigned short* depth_ptr =
      reinterpret_cast<const unsigned short*>(depth_mat.ptr());
  const unsigned short* past_ptr =
      reinterpret_cast<const unsigned short*>(past_mat.ptr());
  short* motion_ptr = reinterpret_cast<short*>(motion.ptr());
  unsigned char* img_ptr = image->ptr();

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
      img_ptr[image::kBlueIndex] = 0;
      img_ptr[image::kGreenIndex] = 0;
      img_ptr[image::kRedIndex] = 0;
    } else if (*motion_ptr > 0) {
      img_ptr[image::kBlueIndex] = normalized_motion;
      img_ptr[image::kGreenIndex] = 0;
      img_ptr[image::kRedIndex] = 0;
    } else {
      img_ptr[image::kBlueIndex] = normalized_motion;
      img_ptr[image::kGreenIndex] = normalized_motion;
      img_ptr[image::kRedIndex] = normalized_motion;
    }

    ++depth_ptr;
    ++past_ptr;
    ++motion_ptr;
    img_ptr += 4;
  }

  // Draw the piano.
  //DrawPiano(image);
}

void Piano::DrawPiano(cv::Mat* image) {
  cv::line(*image, cv::Point(kPianoXMin, kPianoYMin),
           cv::Point(kPianoXMax, kPianoYMin), image::kGrey);
  cv::line(*image, cv::Point(kPianoXMin, kPianoYMin),
           cv::Point(kPianoXMin, kPianoYMax), image::kGrey);
  cv::line(*image, cv::Point(kPianoXMax, kPianoYMin),
           cv::Point(kPianoXMax, kPianoYMax), image::kGrey);
  cv::line(*image, cv::Point(kPianoXMin, kPianoYMax),
           cv::Point(kPianoXMax, kPianoYMax), image::kGrey);

  for (int i = 0; i < kPianoNumNotes; ++i) {
    cv::line(*image, cv::Point(kPianoXMin + i*kPianoNoteWidth, kPianoYMin),
             cv::Point(kPianoXMin + i*kPianoNoteWidth, kPianoYMax), image::kGrey);
  }
}

void Piano::FindNotes(const cv::Mat& depth_mat,
                      const cv::Mat& fingers_mat,
                      const kinect_wrapper::KinectSensorState& sensor_state) {
  std::vector<int> notes(kPianoNumNotes);
  std::vector<int> pixels_per_note(kPianoNumNotes);
  std::vector<int> motion_per_note(kPianoNumNotes);

  cv::Mat past_mat;
  sensor_state.QueryDepth(8, &past_mat);

  cv::Mat motion;
  cv::subtract(depth_mat, past_mat, motion, noArray(), CV_16S);

  short const* motion_ptr = reinterpret_cast<short const*>(motion.ptr());

  // Count active pixels per note and calculate motion.
  for (int row = kPianoYMin; row < kPianoYMax; ++row) {
    unsigned short const* pixel = reinterpret_cast<unsigned short const*>(
        depth_mat.ptr()) + GetIndexOfPixel(kPianoXMin, row);

    for (int note = 0; note < kPianoNumNotes; ++note) {
      for (int col = kPianoXMin + note * kPianoNoteWidth;
           col < kPianoXMin + (note + 1) * kPianoNoteWidth; ++col) {
        
        if (*pixel < kPianoZ && *pixel > 0) {
          ++pixels_per_note[note];
          motion_per_note[note] += abs(*motion_ptr);
        }

        ++pixel;
        ++motion_ptr;
      }
    }
  }

  // Find played notes.
  for (size_t i = 0; i < kPianoNumNotes; ++i) {
    int average_motion = 0;
    if (pixels_per_note[i] > 0)    
      average_motion = motion_per_note[i] / pixels_per_note[i];

    if (pixels_per_note[i] > kPixelsToPlay) {
      notes[i] = ACTION_PLAY;
    } else if (pixels_per_note[i] < kPixelsToRemove) {
      notes[i] = ACTION_REMOVE;
    } else {
      notes[i] = ACTION_STAY;
    }
  }

  notes_.SetNext(notes);
}

}  // namespace piano