#include "image/image_utility.h"

#include "base/logging.h"

namespace image {

void RgbImageToRgbaImage(const cv::Mat& rgb, cv::Mat* rgba) {
  assert(rgba);
  assert(rgb.type() == CV_8UC3);

  *rgba = cv::Mat(rgb.rows, rgb.cols, CV_8UC4);
  unsigned char* rgba_ptr = rgba->ptr();
  unsigned char const* rgb_ptr = rgb.ptr();
  for (size_t i = 0; i < rgb.total(); ++i) {
    rgba_ptr[0] = rgb_ptr[0];
    rgba_ptr[1] = rgb_ptr[1];
    rgba_ptr[2] = rgb_ptr[2];
    rgba_ptr[3] = 255;

    rgba_ptr += 4;
    rgb_ptr += 3;
  }
}

}  // namespace image