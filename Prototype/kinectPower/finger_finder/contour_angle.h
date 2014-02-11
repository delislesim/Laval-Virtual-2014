#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace finger_finder {

struct ContourPointInfo {
  ContourPointInfo(int index, double angle)
      : index(index), angle(angle) {}
  int index;
  double angle;
};

typedef std::vector<std::vector<ContourPointInfo> > ContoursList;

void ContourAngle(const cv::Mat& contours, ContoursList* contours_list);

}  // namespace finger_finder
