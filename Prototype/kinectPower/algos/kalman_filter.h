#pragma once

#include <opencv2/core/core.hpp>

namespace algos {

class KalmanFilter {
 public:
  KalmanFilter();

  void LoadInitialObservation(const cv::Vec3d& initial_observation) {
    x = initial_observation;
  }

  cv::Vec3d Update(const cv::Vec3d& observation);

 private:
   // Position filtrée du doigt
  cv::Vec3d x;

  // Matrice de covariance du bruit sur l'estimation.
  // À quel point notre estimation est bonne (auto-estimation de l'erreur).
  cv::Matx33d p;

};


}  // namespace algos