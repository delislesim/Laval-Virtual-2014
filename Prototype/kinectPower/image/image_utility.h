#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace image {

void InitializeBlackImage(cv::Mat* image);

// Indique si les coordonnées du point spécifié sont à l'extérieur des limites
// de l'image fournie.
inline bool OutOfBoundaries(const cv::Mat& image, const cv::Point& point) {
  if (point.x < 0 || point.x >= image.cols || 
      point.y < 0 || point.y >= image.rows) {
    return true;
  }
  return false;
}

void RgbImageToRgbaImage(const cv::Mat& rgb, cv::Mat* rgba);

}  // namespace image