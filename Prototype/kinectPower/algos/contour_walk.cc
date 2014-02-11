#include "algos/contour_walk.h"

#include "base/logging.h"
#include "algos/maths.h"

namespace algos {

void ContourWalk(double step, const std::vector<cv::Point>& contour,
                 std::vector<cv::Point>* walk) {
  assert(walk);
  assert(walk->empty());

  double remaining_distance_to_next_step = 0.0;

  for (size_t i = 0; i < contour.size(); ++i) {
    cv::Point start_point = contour[i];
    cv::Point end_point = contour[(i + 1) % contour.size()];

    if (start_point == end_point)
      continue;

    cv::Vec3d direction(
        static_cast<double>(end_point.x - start_point.x),
        static_cast<double>(end_point.y - start_point.y));
    double segment_length = cv::norm(direction);
    cv::Vec3d normalized_direction = cv::normalize(direction);

    double position_in_segment = remaining_distance_to_next_step;

    for (;;) {
      if (position_in_segment > segment_length)
        break;

      cv::Point next_point(
          start_point.x + static_cast<int>(normalized_direction.val[0] * position_in_segment),
          start_point.y + static_cast<int>(normalized_direction.val[1] * position_in_segment));
      walk->push_back(next_point);

      position_in_segment += step;
    }

    remaining_distance_to_next_step = position_in_segment - segment_length;
    assert(remaining_distance_to_next_step >= 0);
  }
}

}  // namespace algos