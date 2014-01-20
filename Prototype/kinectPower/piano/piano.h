#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/base.h"
#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_switch.h"

namespace piano {

class Piano : kinect_wrapper::KinectObserver {
 public:
  Piano();
  ~Piano();
  
  virtual void ObserveDepth(
      const cv::Mat& depth_mat,
      const kinect_wrapper::KinectSensorState& sensor_state) OVERRIDE;

  void QueryNotes(unsigned char* notes, size_t notes_size);
  void QueryNiceImage(unsigned char* nice_image, size_t nice_image_size);

 private:
  void DrawPiano();
  void FindNotes();

  void DrawVerticalLine(int x, int ymin, int ymax, unsigned char* img);
  void DrawHorizontalLine(int y, int xmin, int xmax, unsigned char* img);

  unsigned short* depth_ptr() {
    return reinterpret_cast<unsigned short*>(depth_mat_.ptr());
  }

  unsigned char* nice_ptr() {
    return reinterpret_cast<unsigned char*>(nice_image_.GetNextPtr()->ptr());
  }

  bool started_;

  cv::Mat depth_mat_;
  kinect_wrapper::KinectSwitch<cv::Mat> nice_image_;

  kinect_wrapper::KinectSwitch<std::vector<bool> > notes_;
};

}  // namespace piano