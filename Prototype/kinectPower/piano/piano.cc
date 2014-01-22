#include "piano/piano.h"

#include <cmath>
#include <iostream>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

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
const int kPianoZ = 690 /*690 */ /*750 */;
const int kPianoXMin = 100;
const int kPianoYMin = 200;
const int kPianoHeight = 130;
const int kPianoNoteWidth = 24;
const int kPianoNumNotes = 20;
const int kPianoXMax = kPianoXMin + kPianoNumNotes * kPianoNoteWidth;
const int kPianoYMax = kPianoYMin + kPianoHeight;

const int kPixelsToPlay = 150 /*250*/;
const int kPixelsToRemove = 75;

const double kPi = 3.14159265359;

const int kMinFingerDepth = 20;
const int kMaxFingerAngle = 60;

const int ACTION_PLAY = 1;
const int ACTION_STAY = 2;
const int ACTION_REMOVE = 3;

int GetIndexOfPixel(int x, int y) {
  return kinect_wrapper::kKinectDepthWidth * y + x;
}

double round(double number) {
    return number < 0.0 ? ceil(number - 0.5) : floor(number + 0.5);
}

bool IsLeft(cv::Point line1, cv::Point line2, cv::Point point) {
  return ((line2.x - line1.x)*(point.y - line1.y) - (line2.y - line1.y)*(point.x - line1.x)) > 0;
}

double RadToDegrees(double rad) {
  return rad * 360 / (2*kPi);
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
  int tilt = (int)round(RadToDegrees(theta));
  
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

  NOTREACHED() << "Not reached in CalculateTilt";
  return 0;
}

int AngleBetween(const cv::Point& tip, const cv::Point& before,
                 const cv::Point& after) {
  int angle =  abs(round(RadToDegrees(
      atan2(static_cast<double>(before.x - tip.x),
            static_cast<double>(before.y - tip.y)) -
      atan2(static_cast<double>(after.x - tip.x),
            static_cast<double>(after.y - tip.y)))));
  return angle;
}

void ReduceTips(const std::vector<cv::Point>& contour, 
                const std::vector<cv::Vec4i>& defects,
                std::vector<cv::Point>* tips,
                std::vector<cv::Point>* folds) {
  DCHECK(tips);
  DCHECK(folds);
  DCHECK(tips->empty());
  DCHECK(folds->empty());

  bool found_first_fold = false;

  for (int i = 0; i < defects.size(); ++i) {
    std::vector<cv::Point> defect_points;
    defect_points.push_back(contour[defects[i].val[0]]);
    defect_points.push_back(contour[defects[i].val[1]]);
    defect_points.push_back(contour[defects[i].val[2]]);

    const float kThresholdArea = 900.0;

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
  FindNotes(depth_mat, sensor_state);
  DrawFingers(depth_mat);
  //DrawDepth(depth_mat);
  //DrawMotion(depth_mat, sensor_state);
  started_ = true;
}

void Piano::QueryNotes(unsigned char* notes, size_t notes_size) {
  DCHECK(notes_size == kPianoNumNotes);

  if (!started_)
    return;

  std::vector<int> notes_vector;
  notes_.GetCurrent(&notes_vector);

  for (size_t i = 0; i < notes_vector.size(); ++i) {
    notes[i] = notes_vector[i];
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

void Piano::DrawFingers(const cv::Mat& depth_mat____param) {
  cv::Mat depth_mat = depth_mat____param;

  cv::Mat image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC3);
  unsigned char* image_ptr = image.ptr();
  unsigned short const* depth_ptr = reinterpret_cast<unsigned short const*>(depth_mat.ptr());

  cv::Mat jpg = cv::imread("hands.jpg", CV_LOAD_IMAGE_COLOR);
  unsigned char* jpg_ptr = jpg.ptr();

  // TEMP ----------
  for (int i = 0; i < jpg.total(); ++i) {
    int orig_x = i % jpg.cols;
    int orig_y = i / jpg.cols;

    int target_x = 100 + orig_x;
    int target_y = 100 + orig_y;

    int sum = jpg_ptr[0] + jpg_ptr[1] + jpg_ptr[2];

    if (sum > 255*3 - 20) {
      depth_mat.at<unsigned short>(target_y, target_x) = 0;
    } else {
      depth_mat.at<unsigned short>(target_y, target_x) = kPianoZ;
    }
    jpg_ptr += 3;
  }
  // TEMP ----------

  const int kMinZ = kPianoZ - 140;
  const int kMaxZ = kPianoZ + 140;

  cv::Mat simple_hand = cv::Mat(depth_mat.rows, depth_mat.cols, CV_8U);
  unsigned char* simple_hand_ptr = simple_hand.ptr();

  // Draw the initial depth image.
  for (int i = 0; i < image.total(); ++i) {

    int y = i / depth_mat.cols;
    int x = i % depth_mat.cols;

    if (*depth_ptr > kMinZ && *depth_ptr < kMaxZ && y > 100 && y < 100 + jpg.rows && x > 100 && x < 100 + jpg.cols /*&& y > 100 && y < 350 && x > 100 && x < 500*/) {
      
      unsigned int normalized_depth = (((unsigned int)*depth_ptr) - kMinZ) * 255;
      normalized_depth /= (kMaxZ - kMinZ);
      
      image_ptr[kRedIndex] = (unsigned char)normalized_depth;
      image_ptr[kGreenIndex] = (unsigned char)normalized_depth;
      image_ptr[kBlueIndex] = (unsigned char)normalized_depth;

      *simple_hand_ptr = 1;

    } else {
      image_ptr[kRedIndex] = 0;
      image_ptr[kGreenIndex] = 0;
      image_ptr[kBlueIndex] = 0;

      *simple_hand_ptr = 0;
    }

    image_ptr += 3; //// number of channels !!!!!!!
    ++depth_ptr;
    ++simple_hand_ptr;
  }
  
  // Find contours.
  std::vector<std::vector<cv::Point> > contours;
  cv::findContours(simple_hand, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE);

  size_t biggest_contour = 9999;
  size_t biggest_contour_size = 0;
  for (int i = 0; i < contours.size(); ++i) {
    if (contours[i].size() > biggest_contour_size) {
      biggest_contour = i;
      biggest_contour_size = contours[i].size();
    }
  }

  // TODO(fdoray): Find the biggest AREA for a contour rather than highest number of points.
  // TODO(fdoray): Make a separate method for each step.

  if (biggest_contour != 9999) {
    // Draw the biggest contour.
    cv::drawContours(image, contours, biggest_contour, Scalar(255, 0, 0), 1, 8);

    // Find a convex hull for the hand.
    std::vector<int> convex_hull;
    cv::convexHull(contours[biggest_contour], convex_hull);

    // --- Draw the convex hull ---
    /*
    std::vector<std::vector<cv::Point> > hull_pts;
    hull_pts.push_back(std::vector<cv::Point>());
    for (int i = 0; i < convex_hull.size(); ++i) {
      hull_pts[0].push_back(contours[biggest_contour][convex_hull[i]]);
    }
    cv::drawContours(image, hull_pts, 0, Scalar(255, 0, 0), 2, 8);
    */

    // Find convexity defects.
    std::vector<Vec4i> convexity_defects;
    cv::convexityDefects(contours[biggest_contour], convex_hull, convexity_defects);

    /*
    for (int i = 0; i < convexity_defects.size(); ++i) {
      cv::circle(image, contours[biggest_contour][convexity_defects[i].val[0]], 2, Scalar(0, 255, 0));
      cv::circle(image, contours[biggest_contour][convexity_defects[i].val[1]], 2, Scalar(0, 255, 0));
      cv::circle(image, contours[biggest_contour][convexity_defects[i].val[2]], 2, Scalar(0, 0, 255));
    }
    */

    // Find the moments of the image.
    cv::Moments hand_moments = cv::moments(contours[biggest_contour]);
    cv::Point center_of_gravity = cv::Point(round(hand_moments.m10/hand_moments.m00),
                                            round(hand_moments.m01/hand_moments.m00));
    double tilt = CalculateTilt(hand_moments.m11, hand_moments.m20, hand_moments.m02);

    // Draw the center of gravity.
    cv::circle(image, center_of_gravity, 5, Scalar(255, 0, 0));

    // Remove unwanted defects.
    std::vector<cv::Point> tips;
    std::vector<cv::Point> folds;
    ReduceTips(contours[biggest_contour], convexity_defects,
               &tips, &folds);

    // Draw the finger points.
    for (int i = 0; i < tips.size(); ++i)
      cv::circle(image, tips[i], 5, Scalar(0, 255, 0));
    for (int i = 0; i < folds.size(); ++i)
      cv::circle(image, folds[i], 2, Scalar(0, 0, 255));

    std::vector<cv::Scalar> colors;
    colors.push_back(cv::Scalar(176, 23, 31)); // indian red
    colors.push_back(cv::Scalar(70, 130, 180)); // steel blue
    colors.push_back(cv::Scalar(0, 201, 87)); // emerald green
    colors.push_back(cv::Scalar(238, 201, 0)); // gold 
    colors.push_back(cv::Scalar(255, 127, 80)); // coral
    colors.push_back(cv::Scalar(255, 250, 250)); // snow
    colors.push_back(cv::Scalar(124, 252, 0)); // lawn green 

    // Draw a line between each fold.
    for (int i = 0; i < folds.size(); ++i) {
      cv::Point line1(folds[i]);
      cv::Point line2(folds[(i + 1)%folds.size()]);

      cv::line(image, line1, line2, cv::Scalar(255, 0, 0), 1);

      // Vector parallel to the separation line.
      cv::Point parallel(line2.x - line1.x,
                         line2.y - line1.y);
      
      // Vector from the separation line to the tip.
      cv::Point tip = tips[(i + 1)%folds.size()];
      cv::Point line_toward_tip(tip.x - line2.x,
                                tip.y - line2.y);

      // Vectors perpendicular to the separation line.
      cv::Point perp1(-parallel.y, parallel.x);
      cv::Point perp2 = -perp1;
      cv::Point perp;

      bool perp1_left = IsLeft(line1, line2, perp1);
      bool tip_left = IsLeft(line1, line2, line_toward_tip);
      if (perp1_left == tip_left)
        perp = perp1;
      else
        perp = perp2;

      int norm_perp = norm(perp);
      perp = cv::Point(perp.x * 6 / norm_perp, perp.y * 6 / norm_perp);

      // Fill the finger.
      cv::Point fill_point(line1 + cv::Point(parallel.x / 2, parallel.y / 2) + perp);
      cv::floodFill(image, fill_point, colors[i % colors.size()], 0, cv::Scalar(5, 5, 5), cv::Scalar(5, 5, 5));

      cv::circle(image, fill_point, 4, cv::Scalar(255, 0, 255), 3);

    }
  }

  // Add an alpha channel to the image...
  cv::Mat rgba_image = cv::Mat(image.rows, image.cols, CV_8UC4);
  unsigned char* rgba_ptr = rgba_image.ptr();
  unsigned char* rgb_ptr = image.ptr();
  for (int i = 0; i < image.total(); ++i) {
    rgba_ptr[0] = rgb_ptr[0];
    rgba_ptr[1] = rgb_ptr[1];
    rgba_ptr[2] = rgb_ptr[2];
    rgba_ptr[3] = 255;

    rgba_ptr += 4;
    rgb_ptr += 3;

  }

  nice_image_.SetNext(rgba_image);
}

void Piano::DrawDepth(const cv::Mat& depth_mat) {
  cv::Mat image = Mat(depth_mat.rows, depth_mat.cols, CV_8UC4);
  unsigned char* image_ptr = image.ptr();

  // Generate a nice image from the depth information.
  kinect_wrapper::NiceImageFromDepthMat(depth_mat, kPianoZ + 140, kPianoZ - 140, kPianoZ, image_ptr,
                                        image.total() * 4);

  // Draw the piano.
  DrawPiano(&image);

  nice_image_.SetNext(image);
}

void Piano::DrawMotion(const cv::Mat& depth_mat,
                       const kinect_wrapper::KinectSensorState& sensor_state) {
  cv::Mat past_mat;
  sensor_state.QueryDepth(8, &past_mat);

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

void Piano::FindNotes(const cv::Mat& depth_mat,
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
  for (int i = 0; i < kPianoNumNotes; ++i) {
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

void Piano::DrawVerticalLine(int x, int ymin, int ymax, cv::Mat* image) {
  unsigned char* img_ptr = image->ptr();

  int start_index = GetIndexOfPixel(x, ymin);
  int stop_index = GetIndexOfPixel(x, ymax);

  unsigned char* img_run = img_ptr + 4*start_index;
  unsigned char* img_stop = img_ptr + 4*stop_index;

  while (img_run < img_stop) {
    img_run[kRedIndex] = (unsigned char)(static_cast<unsigned int>(255 - img_run[kRedIndex]) * 180 / 255);
    img_run[kGreenIndex] = (unsigned char)(static_cast<unsigned int>(255 -img_run[kGreenIndex]) * 180 / 255);
    img_run[kBlueIndex] = (unsigned char)(static_cast<unsigned int>(255 -img_run[kBlueIndex]) * 180 / 255);

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
    img_run[kRedIndex] = (unsigned char)(static_cast<unsigned int>(255 - img_run[kRedIndex]) * 180 / 255);
    img_run[kGreenIndex] = (unsigned char)(static_cast<unsigned int>(255 -img_run[kGreenIndex]) * 180 / 255);
    img_run[kBlueIndex] = (unsigned char)(static_cast<unsigned int>(255 -img_run[kBlueIndex]) * 180 / 255);

    img_run += 4;
  }
}

}  // namespace piano