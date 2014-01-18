#include "kinect_power/lib.h"

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

static KinectWrapper wrapper;

}  // namespace

void CALLBACK StatusChangeCallback(
    HRESULT /*hrStatus */,
    const OLECHAR* /* instanceName */,
    const OLECHAR* /* uniqueDeviceName */,
    void* /* pUserData */) {
  // TODO(fdoray): Implement this.
}

bool Initialize() {
  wrapper.Initialize();

  std::string error_message;
  KinectSensor* sensor = wrapper.CreateSensorByIndex(0, &error_message);
  if (sensor == NULL)
    return false;

  if (!sensor->OpenDepthStream())
    return false;

  return true;
}

bool GetNiceDepthMap(unsigned char* pixels, unsigned int buffer_size) {
  static kinect_wrapper::KinectBuffer buffer;

  // Get the raw data from the Kinect.
  std::string error_message;
  KinectSensor* sensor = wrapper.CreateSensorByIndex(0, &error_message);
  if (sensor == NULL)
    return false;

  sensor->PollNextDepthFrame(&buffer);

  // Generate a nice image.
  size_t color_index = 0;
  for (size_t pixel_index = 0;
       pixel_index < buffer.GetNbPixels(); ++pixel_index) {
    DCHECK(color_index < buffer_size);

    unsigned short pixel_data = buffer.GetDepthPixel(pixel_index);

    unsigned short depth = pixel_data >> kPlayerIndexBitmaskWidth;
    unsigned int normalized_depth =
      static_cast<unsigned int>((depth) * 255 / 2500);
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
  }

  return true;
}
