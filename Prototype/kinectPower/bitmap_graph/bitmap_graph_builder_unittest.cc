#include "bitmap_graph/bitmap_graph_builder.h"

#include "bitmap_graph/bitmap_run.h"
#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace bitmap_graph {

class ComponentColorer {
 public:
  ComponentColorer(cv::Mat* mat_with_contours_to_remove, int num_pixels)
      : mat_with_contours_to_remove_(mat_with_contours_to_remove),
        num_pixels_(num_pixels) { num_color_ = 0; }

  void ObserveComponentStart() {
    component_indexes_.clear();
  }
  void ObserveComponentEnd() {
    if (component_indexes_.size() < num_pixels_) {
      unsigned char* mat_ptr = mat_with_contours_to_remove_->ptr();
      for (size_t i = 0; i < component_indexes_.size(); ++i) {
        mat_ptr[component_indexes_[i]] = num_color_;
      }
    }

    ++num_color_;
  }

  void ObserveIntersectionStart(int index) {
    component_indexes_.push_back(index);
  }
  void ObserveIntersectionEnd() {
  }

  void ObservePixel(int index) {
    component_indexes_.push_back(index);
  }
  void ObserveLeaf(int index) {
    component_indexes_.push_back(index);
  }

 private:
   int num_color_;

  int num_pixels_;
  cv::Mat* mat_with_contours_to_remove_;
  std::vector<int> component_indexes_;
};

TEST(BitmapBuilder, Connectivity) {
  unsigned char values[] = {1, 0, 0, 0, 0, 0, 0, 1, 
                            0, 1, 0, 0, 0, 0, 0, 1, 
                            0, 0, 1, 0, 1, 1, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 0, 1, 0, 1, 0, 
                            0, 0, 0, 0, 1, 1, 0, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0 };
  cv::Mat test_mat(8, 8, CV_8U, values);
  cv::Mat test_mat_copy;
  test_mat.copyTo(test_mat_copy);

  ComponentColorer builder(&test_mat, 25);
  BitmapRun run;
  run.Run(&test_mat_copy, &builder);
}

}  // bitmap_graph