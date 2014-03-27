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

}  // namespace kinect_wrapper
