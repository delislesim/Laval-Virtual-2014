#include "maths/maths.h"

#include <opencv2/core/core.hpp>

#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace maths {

TEST(Maths, AngleBetween) {
  cv::Vec2i vec_left(1, 0);
  cv::Vec2i vec_top(0, 1);
  cv::Vec2i vec_right(-1, 0);
  cv::Vec2i vec_bottom(0, -1);

  cv::Vec2i diago_left(-1, 1);
  cv::Vec2i diago_right(1, 1);



  EXPECT_DOUBLE_EQ(maths::kPi / 2.0, maths::AngleBetween(vec_left, vec_top));
  EXPECT_DOUBLE_EQ(maths::kPi, maths::AngleBetween(vec_left, vec_right));
  EXPECT_DOUBLE_EQ(
      -maths::kPi / 2.0, maths::AngleBetween(vec_left, vec_bottom));
  EXPECT_DOUBLE_EQ(0.0, maths::AngleBetween(vec_left, vec_left));

  EXPECT_DOUBLE_EQ(-maths::kPi / 2.0, maths::AngleBetween(vec_top, vec_left));
  EXPECT_DOUBLE_EQ(maths::kPi, maths::AngleBetween(vec_right, vec_left));
  EXPECT_DOUBLE_EQ(maths::kPi / 2.0, maths::AngleBetween(vec_bottom, vec_left));

  EXPECT_DOUBLE_EQ(maths::kPi / 4.0, maths::AngleBetween(vec_top, diago_left));
  EXPECT_DOUBLE_EQ(
      -maths::kPi / 4.0, maths::AngleBetween(vec_top, diago_right));
  EXPECT_DOUBLE_EQ(-maths::kPi / 4.0, maths::AngleBetween(diago_left, vec_top));
  EXPECT_DOUBLE_EQ(maths::kPi / 4.0, maths::AngleBetween(diago_right, vec_top));
}

}  // namespace maths
