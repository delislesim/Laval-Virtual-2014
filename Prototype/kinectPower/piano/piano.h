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
      const kinect_wrapper::KinectSensorState& sensor_state) override;

  void QueryNotes(unsigned char* notes, size_t notes_size);
  void QueryNiceImage(unsigned char* nice_image, size_t nice_image_size);

 private:
  void DrawFingers(const cv::Mat& depth_mat, cv::Mat* image);
  void DrawDepth(const cv::Mat& depth_mat, cv::Mat* image);
  void DrawMotion(const cv::Mat& depth_mat,
                  const kinect_wrapper::KinectSensorState& sensor_state,
                  cv::Mat* image);
  void DrawPiano(cv::Mat* image);
  void FindNotes(const cv::Mat& depth_mat,
                 const kinect_wrapper::KinectSensorState& sensor_state);

  void DrawVerticalLine(int x, int ymin, int ymax, cv::Mat* image);
  void DrawHorizontalLine(int y, int xmin, int xmax, cv::Mat* image);

  bool started_;
  kinect_wrapper::KinectSwitch<cv::Mat> nice_image_;
  kinect_wrapper::KinectSwitch<std::vector<int> > notes_;
};

}  // namespace piano