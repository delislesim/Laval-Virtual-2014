#include "kinect_wrapper/kinect_buffer.h"

#include "base/base.h"
#include "base/logging.h"

namespace kinect_wrapper {

KinectBuffer::KinectBuffer(size_t width,
                           size_t height,
                           size_t bytes_per_pixel)
    : current_buffer_index_(0),
      width_(width),
      height_(height),
      bytes_per_pixel_(bytes_per_pixel) {
  size_t size = width * height * bytes_per_pixel;
  for (int i = 0; i < kNumBuffers; ++i)
    buffers_[i].resize(size);
}

KinectBuffer::~KinectBuffer() {
}

void KinectBuffer::CopyData(const char* data,
                            size_t size,
                            size_t width,
                            size_t height,
                            size_t bytes_per_pixel) {
  DCHECK(size == width * height * bytes_per_pixel);

  int new_buffer_index = current_buffer_index_ == 0 ? 1 : 0;
  memcpy_s(&buffers_[new_buffer_index][0], buffers_[new_buffer_index].size(),
           data, size);
  current_buffer_index_ = new_buffer_index;

  // TODO(fdoray): Copy in an OpenCV matrix immediatly.
}

void KinectBuffer::CopyDepthTexture(const NUI_DEPTH_IMAGE_PIXEL* start,
                                    const NUI_DEPTH_IMAGE_PIXEL* end) {
  int new_buffer_index = current_buffer_index_ == 0 ? 1 : 0;

  unsigned short* buffer_run =
      reinterpret_cast<unsigned short*>(&buffers_[new_buffer_index][0]);
  NUI_DEPTH_IMAGE_PIXEL const* src_run = start;

  while (src_run < end) {
    *buffer_run = src_run->depth;

    ++buffer_run;
    ++src_run;
  }

  current_buffer_index_ = new_buffer_index;
}

void KinectBuffer::GetDepthMat(cv::Mat* depth_mat) {
  *depth_mat = cv::Mat(height_, width_, CV_16U, 
                       &buffers_[current_buffer_index_][0]).clone();
}

}  // namespace kinect_wrapper