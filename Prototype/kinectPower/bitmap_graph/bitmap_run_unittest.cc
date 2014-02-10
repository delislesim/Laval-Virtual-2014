#include "bitmap_graph/bitmap_run.h"

#include "bitmap_graph/bitmap_graph_builder.h"
#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace bitmap_graph {

TEST(BitmapRun, Bitmap4x4) {
  unsigned char values[] = {1, 1, 0, 0,
                            0, 1, 0, 0,
                            0, 1, 0, 1,
                            0, 0, 1, 0};
  cv::Mat test_mat(4, 4, CV_8U, values);

  BitmapGraphBuilder builder;
  BitmapRun run;
  run.Run(&test_mat, &builder);
}

TEST(BitmapRun, Bitmap8x8) {
  unsigned char values[] = {1, 0, 0, 0, 0, 0, 0, 1, 
                            0, 1, 0, 0, 0, 0, 0, 1, 
                            0, 0, 1, 0, 1, 1, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 0, 1, 0, 1, 0, 
                            0, 0, 0, 0, 1, 1, 0, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0 };
  cv::Mat test_mat(8, 8, CV_8U, values);

  BitmapGraphBuilder builder;
  BitmapRun run;
  run.Run(&test_mat, &builder);
}

TEST(BitmapRun, NiceAlgo) {
  unsigned char values[] = {0, 0, 0, 
                            0, 1, 0, 
                            1, 1, 1, 
                            1, 0, 1, 
                            1, 0, 1, 
                            1, 0, 0, 
                            1, 1, 1};
  cv::Mat test_mat(7, 3, CV_8U, values);

  cv::Mat graph;
  BuildBitmapGraph(test_mat, &graph);

  std::cout << "M = " << graph << std::endl;
}

}  // bitmap_graph