#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "finger_finder/hand_parameters.h"

namespace finger_finder {

void FindFingersInContour(const cv::Mat& depth_mat,
                         const std::vector<cv::Point>& contour,
                         HandParameters* hand_parameters,
                         cv::Mat* nice_image);

}  // finger_finder