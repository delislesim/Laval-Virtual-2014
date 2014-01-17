#include "kinect_power/lib.h"

#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_depth_stream.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"

using namespace kinect_wrapper;

namespace {

KinectWrapper wrapper;

const int kBlueIndex = 0;
const int kGreenIndex = 1;
const int kRedIndex = 2;
const int kAlphaIndex = 3;

}  // namespace

bool GetNiceDepthMap(unsigned char* pixels) {
  static kinect_wrapper::KinectBuffer buffer;

  // Get the raw data from the Kinect.
  std::string error_message;
  KinectSensor* sensor = wrapper.CreateSensorByIndex(0, &error_message);
  if (sensor == NULL)
    return false;

  KinectDepthStream* depth_stream = sensor->GetDepthStream();
  if (depth_stream == NULL || !depth_stream->opened())
    return false;

  depth_stream->PollNextFrame(&buffer);

  // Generate a nice image.
  size_t color_index = 0;
  for (size_t pixel_index = 0;
       pixel_index < buffer.GetNbPixels(); ++pixel_index) {
    color_index += 4;

    unsigned short pixel_data = buffer.GetDepthPixel(pixel_index);

    unsigned short depth = pixel_data >> kPlayerIndexBitmaskWidth;
    unsigned char normalized_depth = static_cast<unsigned char>(
      static_cast<unsigned int>(depth) * 255 / 1500);

    pixels[color_index + kBlueIndex] = normalized_depth;
    pixels[color_index + kGreenIndex] = normalized_depth;
    pixels[color_index + kRedIndex] = normalized_depth;
    pixels[color_index + kAlphaIndex] = 255;

    if (depth == 0) {
      pixels[color_index + kBlueIndex] = 0;
      pixels[color_index + kGreenIndex] = 0;
    }
  }

  return true;
}
