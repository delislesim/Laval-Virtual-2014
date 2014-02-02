#include "maths/smooth.h"

#include "base/logging.h"

namespace maths {

double Smooth(double old_val, double new_val, double min_speed_distance,
              double min_speed, double max_speed_distance, double max_speed) {
  assert(min_speed_distance > 0);
  assert(min_speed > 0);
  assert(max_speed_distance >= min_speed_distance);
  assert(max_speed >= min_speed);
  assert(max_speed <= max_speed_distance);

  double distance = new_val - old_val;
  double direction = 1;
  if (distance < 0) {
    distance = -distance;
    direction = -1;
  }

  if (distance <= min_speed_distance)
    return old_val;
  if (distance >= max_speed_distance)
    return old_val + direction * max_speed;

  double delta_distance = distance - min_speed_distance;
  double extra_speed = delta_distance * (max_speed - min_speed) /
      (max_speed_distance - min_speed_distance);
  double speed = min_speed + extra_speed;

  return old_val + direction * speed;
}

}  // namespace maths

