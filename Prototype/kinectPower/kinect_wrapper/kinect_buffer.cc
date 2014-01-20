#include "kinect_wrapper/kinect_buffer.h"

#include "base/base.h"
#include "base/logging.h"

namespace kinect_wrapper {

KinectBuffer::KinectBuffer(size_t width,
                           size_t height,
                           int type)
    : current_buffer_index_(0),
      width_(width),
      height_(height),
      bytes_per_pixel_(0) {
  for (int i = 0; i < kNumBuffers; ++i)
    buffers_[i] = cv::Mat(height, width, type);

  bytes_per_pixel_ = buffers_[0].elemSize();
}

KinectBuffer::~KinectBuffer() {
}

void KinectBuffer::CopyData(const char* data,
                            size_t size) {
  DCHECK(size == buffers_[0].total() * buffers_[0].elemSize());

  int new_buffer_index = (current_buffer_index_ + 1) % kNumBuffers;
  memcpy_s(buffers_[new_buffer_index].ptr(), size,
           data, size);
  current_buffer_index_ = new_buffer_index;
}

void KinectBuffer::CopyDepthTexture(const NUI_DEPTH_IMAGE_PIXEL* start,
                                    const NUI_DEPTH_IMAGE_PIXEL* end) {
  DCHECK(bytes_per_pixel_ == sizeof(short));

  int new_buffer_index = (current_buffer_index_ + 1) % kNumBuffers;

  unsigned short* buffer_run =
      reinterpret_cast<unsigned short*>(buffers_[new_buffer_index].ptr());
  NUI_DEPTH_IMAGE_PIXEL const* src_run = start;

  while (src_run < end) {
    *buffer_run = src_run->depth;

    ++buffer_run;
    ++src_run;
  }

  current_buffer_index_ = new_buffer_index;
}

void KinectBuffer::GetDepthMat(cv::Mat* depth_mat) {
  *depth_mat = buffers_[current_buffer_index_];
}

}  // namespace kinect_wrapper