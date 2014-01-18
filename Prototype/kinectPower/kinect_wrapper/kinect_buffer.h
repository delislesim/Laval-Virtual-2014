#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/base.h"

namespace kinect_wrapper {

class KinectBuffer {
 public:
  KinectBuffer(size_t width, size_t height, size_t bytes_per_pixel);
  ~KinectBuffer();

  // This method is not thread safe!
  void CopyData(const char* data,
                size_t size,
                size_t width,
                size_t height,
                size_t bytes_per_pixel);

  // This method is thread-safe.
  size_t GetNbPixels() const {
    return width_ * height_;
  }

  void GetDepthMat(cv::Mat* depth_mat);

 private:
  size_t current_buffer_index_;

  size_t width_;
  size_t height_;
  size_t bytes_per_pixel_;

  std::vector<char> buffers_[2];

  DISALLOW_COPY_AND_ASSIGN(KinectBuffer);
};

}  // namespace kinect_wrapper