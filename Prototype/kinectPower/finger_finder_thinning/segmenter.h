#pragma once

#include <opencv2/core/core.hpp>

namespace finger_finder_thinning {

// Détermine le contour de chaque main. Une main est une zone de pixels
// dont la profondeur est entre |min_depth| et |max_depth|.
void Segmenter(const cv::Mat& depth_mat, int min_depth, int max_depth,
               std::vector<std::vector<cv::Point>>* contours);

}  // namespace finger_finder_thinning
