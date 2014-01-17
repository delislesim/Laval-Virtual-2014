#pragma once

#include <vector>

#include "base/base.h"

namespace kinect_wrapper {

class KinectBuffer {
 public:
  KinectBuffer();
  ~KinectBuffer();

  void CopyData(const char* data,
                size_t size,
                size_t width,
                size_t height,
                size_t bytes_per_pixel);

  size_t GetNbPixels() const {
    return width_ * height_;
  }

  unsigned short GetDepthPixel(size_t index) const {
    return (reinterpret_cast<const unsigned short*>(&buffer_[0]))[index];
  }

 private:
  size_t width_;
  size_t height_;

  std::vector<char> buffer_;

  DISALLOW_COPY_AND_ASSIGN(KinectBuffer);
};

}  // namespace kinect_wrapper