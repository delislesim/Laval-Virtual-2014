#pragma once

#include <opencv2/core/core.hpp>
#include <vector>

#include "finger_finder/finger_info.h"

namespace finger_finder {

class FingerFinder {
 public:
  FingerFinder(unsigned short target_depth,
               unsigned short depth_tolerance);

  // Re�oit les donn�es de la Kinect et fait la d�tection de la position
  // des doigts dans celles-ci.
  void ObserveData(const cv::Mat& depth_mat, const cv::Mat& color_mat,
                   cv::Mat* nice_image);
  
  // Permet d'obtenir les positions de doigts calcul�es pour la derni�re
  // image re�ue.
  void QueryFingers(FingerInfoVector* fingers);

 private:
  // Cette m�thode s'attend � recevoir 2 matrices dans le m�me syst�me de
  // coordonn�es et d�j� tronqu�es pour contenir seulement la zone d'int�r�t.
  void ObserveDataInternal(const cv::Mat& depth_mat, const cv::Mat& color_mat,
                           cv::Mat* nice_image);

  // Profondeur minimale et maximale entre lesquelles chercher des doigts.
  unsigned short min_hands_depth_;
  unsigned short max_hands_depth_;

  // Doigts trouv�s.
  FingerInfoVector fingers_;

};

}  // namespace finger_finder
