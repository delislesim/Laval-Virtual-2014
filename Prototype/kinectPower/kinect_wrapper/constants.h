#pragma once

namespace kinect_wrapper {

extern const short kPlayerIndexBitmask;
extern const short kPlayerIndexBitmaskWidth;

extern const size_t kKinectDepthBytesPerPixel;
extern const size_t kKinectDepthWidth;
extern const size_t kKinectDepthHeight;

#define kNumBuffers (10)

#define kNumTrackedSkeletons (2)

}  // namespace kinect_wrapper
