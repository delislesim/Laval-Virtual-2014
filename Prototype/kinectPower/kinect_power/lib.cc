#include "kinect_power/lib.h"

#include <iostream>
#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/utility.h"
#include "piano/piano.h"

using namespace kinect_wrapper;

namespace {

piano::Piano the_piano;

}  // namespace


bool Initialize() {
  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Initialize();
  wrapper->StartSensorThread(0);
  return true;
}

bool Shutdown() {
  KinectWrapper* wrapper = KinectWrapper::instance();
  wrapper->Shutdown();
  return true;
}

bool GetNiceDepthMap(unsigned char* pixels, unsigned int pixels_size) {
  const int kMinDepth = 690;
  const int kMaxDepth = 2500;
   
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->QueryDepthBuffer(0, &mat))
    return false;

  NiceImageFromDepthMat(mat, kMaxDepth, kMinDepth, pixels, pixels_size);

  return true;
}

bool GetPianoInfo(unsigned char* notes, unsigned int notes_size,
                  unsigned char* pixels, unsigned int pixels_size) {
  KinectWrapper* wrapper = KinectWrapper::instance();

  cv::Mat depth_mat;
  if (!wrapper->QueryDepthBuffer(0, &depth_mat))
    return false;

  the_piano.LoadDepthImage(depth_mat);
  the_piano.QueryNiceImage(pixels, pixels_size);
  the_piano.QueryNotes(notes, notes_size);

  return true;
}
