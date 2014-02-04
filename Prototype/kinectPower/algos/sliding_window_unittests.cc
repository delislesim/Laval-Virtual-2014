#include "algos/sliding_window.h"

#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace algos {

TEST(Algos, SlidingWindow) {
  std::vector<double> values;
  values.push_back(1.0);
  values.push_back(2.0);
  values.push_back(3.0);
  values.push_back(4.0);
  values.push_back(5.0);
  values.push_back(6.0);

  std::vector<double> sums;

  algos::SlidingWindow(values, 2, &sums);

  EXPECT_DOUBLE_EQ(16.0 + 1.0, sums[0]);
  EXPECT_DOUBLE_EQ(14.0 + 2.0, sums[1]);
  EXPECT_DOUBLE_EQ(12.0 + 3.0, sums[2]);
  EXPECT_DOUBLE_EQ(16.0 + 4.0, sums[3]);
  EXPECT_DOUBLE_EQ(14.0 + 5.0, sums[4]);
  EXPECT_DOUBLE_EQ(12.0 + 6.0, sums[5]);
}

}  // namespace algos