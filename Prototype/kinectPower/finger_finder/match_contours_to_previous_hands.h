#pragma once

#include <opencv2/core/core.hpp>

#include "finger_finder/hand_parameters.h"

namespace finger_finder {

void MatchContoursToPreviousHands(
    const std::vector<std::vector<cv::Point> >& contours,
    const std::vector<HandParameters>& previous_hand_parameters,
    std::vector<cv::Point>* contours_centers,
    std::vector<int>* matches);

}  // namespace finger_finder