#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectBuffer {
 public:
  KinectBuffer(size_t width, size_t height, int type);
  ~KinectBuffer();

  // This method is not thread safe!
  void CopyData(const char* data,
                size_t size);

  // This method is not thread safe!
  void CopyDepthTexture(const NUI_DEPTH_IMAGE_PIXEL* start,
                        const NUI_DEPTH_IMAGE_PIXEL* end);

  // This method is thread-safe.
  size_t GetNbPixels() const {
    return width_ * height_;
  }

  // This method is thread-safe.
  void GetMatrix(int past_frame, cv::Mat* depth_mat);

 private:
  size_t current_buffer_index_;

  size_t width_;
  size_t height_;
  size_t bytes_per_pixel_;

  cv::Mat buffers_[kNumBuffers];

  DISALLOW_COPY_AND_ASSIGN(KinectBuffer);
};

}  // namespace kinect_wrapper