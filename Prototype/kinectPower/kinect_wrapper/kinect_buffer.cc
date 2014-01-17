#include "kinect_wrapper/kinect_buffer.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectBuffer::KinectBuffer()
    : width_(0),
      height_(0) {
}

KinectBuffer::~KinectBuffer() {
}

void KinectBuffer::CopyData(const char* data,
                            size_t size,
                            size_t width,
                            size_t height,
                            size_t bytes_per_pixel) {
  DCHECK(size = width * height * bytes_per_pixel);
  width_ = width;
  height_ = height;
  buffer_.resize(size);

  memcpy_s(&buffer_[0], buffer_.size(), data, size);
}

}  // namespace kinect_wrapper