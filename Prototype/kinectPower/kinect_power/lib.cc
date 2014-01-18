#include "kinect_power/lib.h"

#include <opencv2/core/core.hpp>

#include "base/logging.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"

using namespace kinect_wrapper;

namespace {

const int kBlueIndex = 0;
const int kGreenIndex = 1;
const int kRedIndex = 2;
const int kAlphaIndex = 3;

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

bool GetNiceDepthMap(unsigned char* pixels, unsigned int buffer_size) {
  const int kMaxDepth = 2500;
   
  KinectWrapper* wrapper = KinectWrapper::instance();

  // Get the raw data from the Kinect.
  cv::Mat mat;
  if (!wrapper->QueryDepthBuffer(0, &mat))
    return false;

  unsigned short* ptr = reinterpret_cast<unsigned short*>(mat.ptr());

  // Generate a nice image.
  size_t color_index = 0;
  for (size_t pixel_index = 0;
       pixel_index < mat.total(); ++pixel_index) {
    DCHECK(color_index < buffer_size);

    unsigned short pixel_data = *ptr;

    unsigned short depth = pixel_data; /* >> kPlayerIndexBitmaskWidth; */
    unsigned int normalized_depth =
      static_cast<unsigned int>((depth) * 255 / kMaxDepth);
    if (normalized_depth > 255)
      normalized_depth = 255;
    unsigned char byte = static_cast<unsigned char>(normalized_depth);

    pixels[color_index + kBlueIndex] = 255 - byte;
    pixels[color_index + kGreenIndex] = 255 - byte;
    pixels[color_index + kRedIndex] = 255 - byte;
    pixels[color_index + kAlphaIndex] = 255;

    if (depth == 0) {
      pixels[color_index + kBlueIndex] = 0;
      pixels[color_index + kGreenIndex] = 0;
    }

    color_index += 4;
    ++ptr;
  }

  return true;
}
