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

const unsigned int kNumSkeletons = 6;

const float kDistanceMaxToTrackSkeleton = 2.25f;

}  // namespace kinect_wrapper
