#pragma once

#include <opencv2/core/core.hpp>

#include "kinect_wrapper/kinect_sensor.h"

namespace kinect_wrapper {

// Safe release for interfaces
// From the Kinect Explorer D2D sample.
template<class Interface>
inline void SafeRelease(Interface*& pInterfaceToRelease) {
  if (pInterfaceToRelease) {
    pInterfaceToRelease->Release();
    pInterfaceToRelease = nullptr;
  }
}

void NiceImageFromDepthMat(cv::Mat depth_mat,
                           unsigned short max_depth, unsigned short min_depth,
                           unsigned short color_depth,
                           unsigned char* nice_image, size_t nice_image_size);

}  // namespace kinect_wrapper
