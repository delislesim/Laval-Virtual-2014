#include "image/image_utility.h"

#include "base/logging.h"
#include "image/image_constants.h"

namespace image {

void InitializeBlackImage(cv::Mat* image) {
  assert(image);
  assert(image->type() == CV_8UC4);

  unsigned char* image_run = reinterpret_cast<unsigned char*>(image->ptr());

  for (size_t i = 0; i < image->total(); ++i) {
    image_run[kRedIndex] = 0;
    image_run[kGreenIndex] = 0;
    image_run[kBlueIndex] = 0;
    image_run[kAlphaIndex] = 255;
    image_run += 4;
  }
}

// TODO(fdoray): This function doesn't seem to be used.
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