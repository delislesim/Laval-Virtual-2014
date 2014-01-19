#pragma once

#include "base/logging.h"
#include "kinect_wrapper/utility.h"

namespace kinect_wrapper {

namespace {

const int kBlueIndex = 0;
const int kGreenIndex = 1;
const int kRedIndex = 2;
const int kAlphaIndex = 3;

}  // namespace

void NiceImageFromDepthMat(cv::Mat depth_mat,
                           unsigned short max_depth, unsigned short min_depth,
                           unsigned char* nice_image, size_t nice_image_size) {
  unsigned short* ptr = reinterpret_cast<unsigned short*>(depth_mat.ptr());

  // Generate a nice image.
  size_t color_index = 0;
  for (size_t pixel_index = 0;
       pixel_index < depth_mat.total(); ++pixel_index) {
    DCHECK(color_index < nice_image_size);

    unsigned short pixel_data = *ptr;

    unsigned short depth = pixel_data; /* >> kPlayerIndexBitmaskWidth; */
    unsigned int normalized_depth =
      static_cast<unsigned int>((depth) * 255 / max_depth);
    if (normalized_depth > 255)
      normalized_depth = 255;
    unsigned char byte = static_cast<unsigned char>(normalized_depth);

    nice_image[color_index + kBlueIndex] = 255 - byte;
    nice_image[color_index + kGreenIndex] = 255 - byte;
    nice_image[color_index + kRedIndex] = 255 - byte;
    nice_image[color_index + kAlphaIndex] = 255;

    if (depth == 0) {
      // Red.
      nice_image[color_index + kBlueIndex] = 0;
      nice_image[color_index + kGreenIndex] = 0;
    } else if (depth < min_depth) {
      // Blue.
      nice_image[color_index + kRedIndex] = 0;
      nice_image[color_index + kGreenIndex] = 0;
    }

    color_index += 4;
    ++ptr;
  }
}

}  // namespace kinect_wrapper
