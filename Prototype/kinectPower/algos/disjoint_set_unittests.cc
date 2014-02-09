#include "algos/disjoint_set.h"

#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace algos {

TEST(Algos, DisjointSet) {
  cv::Size size(10, 10);
  DisjointSet disjoint_set(size);

  disjoint_set.Union(9, 10);
  disjoint_set.Union(10, 8);
  disjoint_set.Union(8, 7);

  disjoint_set.Union(1, 2);
  disjoint_set.Union(2, 3);
  disjoint_set.Union(3, 4);
  disjoint_set.Union(4, 1);

  EXPECT_EQ(disjoint_set.Find(9), disjoint_set.Find(10));
  EXPECT_EQ(disjoint_set.Find(9), disjoint_set.Find(9));
  EXPECT_EQ(disjoint_set.Find(9), disjoint_set.Find(8));
  EXPECT_EQ(disjoint_set.Find(9), disjoint_set.Find(7));

  EXPECT_EQ(disjoint_set.Find(10), disjoint_set.Find(10));
  EXPECT_EQ(disjoint_set.Find(10), disjoint_set.Find(9));
  EXPECT_EQ(disjoint_set.Find(10), disjoint_set.Find(8));
  EXPECT_EQ(disjoint_set.Find(10), disjoint_set.Find(7));

  EXPECT_EQ(disjoint_set.Find(8), disjoint_set.Find(10));
  EXPECT_EQ(disjoint_set.Find(8), disjoint_set.Find(9));
  EXPECT_EQ(disjoint_set.Find(8), disjoint_set.Find(8));
  EXPECT_EQ(disjoint_set.Find(8), disjoint_set.Find(7));

  EXPECT_EQ(disjoint_set.Find(7), disjoint_set.Find(10));
  EXPECT_EQ(disjoint_set.Find(7), disjoint_set.Find(9));
  EXPECT_EQ(disjoint_set.Find(7), disjoint_set.Find(8));
  EXPECT_EQ(disjoint_set.Find(7), disjoint_set.Find(7));



  EXPECT_EQ(disjoint_set.Find(1), disjoint_set.Find(1));
  EXPECT_EQ(disjoint_set.Find(1), disjoint_set.Find(2));
  EXPECT_EQ(disjoint_set.Find(1), disjoint_set.Find(3));
  EXPECT_EQ(disjoint_set.Find(1), disjoint_set.Find(4));

  EXPECT_EQ(disjoint_set.Find(2), disjoint_set.Find(1));
  EXPECT_EQ(disjoint_set.Find(2), disjoint_set.Find(2));
  EXPECT_EQ(disjoint_set.Find(2), disjoint_set.Find(3));
  EXPECT_EQ(disjoint_set.Find(2), disjoint_set.Find(4));

  EXPECT_EQ(disjoint_set.Find(3), disjoint_set.Find(1));
  EXPECT_EQ(disjoint_set.Find(3), disjoint_set.Find(2));
  EXPECT_EQ(disjoint_set.Find(3), disjoint_set.Find(3));
  EXPECT_EQ(disjoint_set.Find(3), disjoint_set.Find(4));

  EXPECT_EQ(disjoint_set.Find(4), disjoint_set.Find(1));
  EXPECT_EQ(disjoint_set.Find(4), disjoint_set.Find(2));
  EXPECT_EQ(disjoint_set.Find(4), disjoint_set.Find(3));
  EXPECT_EQ(disjoint_set.Find(4), disjoint_set.Find(4));


  EXPECT_NE(disjoint_set.Find(9), disjoint_set.Find(1));
  EXPECT_NE(disjoint_set.Find(9), disjoint_set.Find(40));
}

}  // namespace algos