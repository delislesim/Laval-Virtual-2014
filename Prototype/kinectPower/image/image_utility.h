#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#define MAT_PTR(variable, type) type* variable ## _ptr = reinterpret_cast<type*>(( variable ).ptr());
#define MAT_PTR_PTR(variable, type) type* variable ## _ptr = reinterpret_cast<type*>(( variable )->ptr());

#define FOR_MATRIX(iteration_variable, matrix_variable) \
  int num_elements_iteration_ ## __LINE__ = ( matrix_variable ).total();  \
  for (int iteration_variable = 0; iteration_variable < num_elements_iteration_ ## __LINE__ ; ++ iteration_variable )


namespace image {

// Indique si les coordonnées du point spécifié sont à l'extérieur des limites
// de l'image fournie.
inline bool OutOfBoundaries(const cv::Mat& image, const cv::Point& point) {
  if (point.x < 0 || point.x >= image.cols || 
      point.y < 0 || point.y >= image.rows) {
    return true;
  }
  return false;
}

}  // namespace image