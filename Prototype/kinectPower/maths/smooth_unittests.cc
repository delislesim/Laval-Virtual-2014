#include "maths/smooth.h"

#include <opencv2/core/core.hpp>

#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace maths {

TEST(Maths, Smooth) {
  const double min_speed_distance = 10;
  const double min_speed = 1;
  const double max_speed_distance = 20;
  const double max_speed = 11;

  EXPECT_DOUBLE_EQ(0, maths::Smooth(0, 5, min_speed_distance, min_speed, max_speed_distance, max_speed));
  EXPECT_DOUBLE_EQ(11, maths::Smooth(0, 20, min_speed_distance, min_speed, max_speed_distance, max_speed));
  EXPECT_DOUBLE_EQ(6, maths::Smooth(0, 15, min_speed_distance, min_speed, max_speed_distance, max_speed));

  EXPECT_DOUBLE_EQ(10+0, maths::Smooth(10+0, 10+5, min_speed_distance, min_speed, max_speed_distance, max_speed));
  EXPECT_DOUBLE_EQ(10+11, maths::Smooth(10+0, 10+20, min_speed_distance, min_speed, max_speed_distance, max_speed));
  EXPECT_DOUBLE_EQ(10+6, maths::Smooth(10+0, 10+15, min_speed_distance, min_speed, max_speed_distance, max_speed));
}

}  // namespace maths
