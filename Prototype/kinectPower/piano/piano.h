#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

namespace piano {

class Piano {
 public:
  Piano();
  ~Piano();
  
  // Note: The provided |depth_mat| will be modified by the algorithm.
  void LoadDepthImage(cv::Mat depth_mat);
  void QueryNotes(unsigned char* notes, size_t notes_size);
  void QueryNiceImage(unsigned char* nice_image, size_t nice_image_size);

 private:
  void DrawPiano();
  void FindNotes();

  void DrawVerticalLine(int x, int ymin, int ymax);
  void DrawHorizontalLine(int y, int xmin, int xmax);

  unsigned short* depth_ptr() {
    return reinterpret_cast<unsigned short*>(depth_mat_.ptr());
  }

  unsigned char* nice_ptr() {
    return reinterpret_cast<unsigned char*>(nice_image_.ptr());
  }

  cv::Mat depth_mat_;
  cv::Mat nice_image_;

  std::vector<bool> notes_;
};

}  // namespace piano