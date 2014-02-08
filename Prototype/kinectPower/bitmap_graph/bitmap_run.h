#pragma once

#include <opencv2/core/core.hpp>

namespace bitmap_graph {

class BitmapGraphBuilder;

class BitmapRun {
 public:
  BitmapRun();

  void Run(cv::Mat* bitmap, BitmapGraphBuilder* builder);

 private:

};


}  // bitmap_graph