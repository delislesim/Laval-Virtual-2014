#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "base/base.h"
#include "finger_finder/finger_finder.h"
#include "finger_finder/hand_parameters.h"
#include "finger_finder_thinning/finger_finder.h"
#include "kinect_wrapper/kinect_observer.h"
#include "kinect_wrapper/kinect_switch.h"

namespace piano {

class Piano : public kinect_wrapper::KinectObserver {
 public:
  Piano();
  ~Piano();
  
  virtual void ObserveDepth(
      const cv::Mat& depth_mat,
      const kinect_wrapper::KinectSensorData& data) override;

  void QueryNiceImage(unsigned char* nice_image, size_t nice_image_size);
  void QueryHandsDepth(unsigned char* hands_depth, size_t hands_depth_size);
  void QueryHandParameters(std::vector<finger_finder::HandParameters>* hand_parameters);

  void QueryFingers(std::vector<finger_finder_thinning::FingerDescription>* fingers);

 private:
  unsigned short min_hands_depth_;
  unsigned short max_hands_depth_;

  finger_finder_thinning::FingerFinder finger_finder_;

  bool started_;
  kinect_wrapper::KinectSwitch<cv::Mat> nice_image_;
  kinect_wrapper::KinectSwitch<cv::Mat> hands_depth_;
  kinect_wrapper::KinectSwitch<std::vector<finger_finder::HandParameters> > hand_parameters_;

  kinect_wrapper::KinectSwitch<std::vector<finger_finder_thinning::FingerDescription> > fingers_;
};

}  // namespace piano