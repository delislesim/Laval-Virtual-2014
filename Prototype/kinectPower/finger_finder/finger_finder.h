#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "finger_finder/finger_info.h"

namespace finger_finder {

class FingerFinder {
 public:
  FingerFinder(unsigned short min_hands_depth,
               unsigned short max_hands_depth);

  // Re�oit les donn�es de la Kinect et fait la d�tection de la position
  // des doigts dans celles-ci.
  void ObserveData(const cv::Mat& depth_mat, const cv::Mat& color_mat,
                   cv::Mat* nice_image);
  
  // Permet d'obtenir les positions de doigts calcul�es pour la derni�re
  // image re�ue.
  void QueryFingers(std::vector<FingerInfo>* fingers);

 private:
  // Profondeur minimale et maximale entre lesquelles chercher des doigts.
  unsigned short min_hands_depth_;
  unsigned short max_hands_depth_;

  // Doigts trouv�s.
  std::vector<FingerInfo> fingers_;

};

}  // namespace finger_finder
