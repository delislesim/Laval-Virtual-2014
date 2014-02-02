#pragma once

namespace maths {

double Smooth(double old_val, double new_val, double min_speed_distance,
              double min_speed, double max_speed_distance, double max_speed);

}  // maths