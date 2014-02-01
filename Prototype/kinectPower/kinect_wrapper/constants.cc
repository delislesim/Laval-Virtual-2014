#pragma once

#include "kinect_wrapper/constants.h"

namespace kinect_wrapper {

const short kPlayerIndexBitmask = 7;
const short kPlayerIndexBitmaskWidth = 3;

const size_t kKinectDepthBytesPerPixel = 2;
const size_t kKinectDepthWidth = 640;
const size_t kKinectDepthHeight = 480;

const size_t kKinectColorWidth = 640;
const size_t kKinectColorHeight = 480;

extern const unsigned int kNumSkeletons = 6;

}  // namespace kinect_wrapper
