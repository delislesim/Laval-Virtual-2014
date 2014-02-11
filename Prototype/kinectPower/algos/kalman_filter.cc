#include "algos/kalman_filter.h"

#include "base/logging.h"
#include "algos/maths.h"

namespace algos {

namespace {
const cv::Matx33d identity(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);

// Matrice de covariance du bruit sur l'état
// Plus c'est gros, plus ça bouge (moins on fait confiance à l'hypothèse que ça ne bouge pas).
cv::Matx33d q(1.0, 0.0, 0.0, 0.0, 2.0, 0.0, 0.0, 0.0, 2.0);

// Matrice de covariance de l'erreur sur l'observation.
// Si l'observation shake beaucoup, le mettre gros.
cv::Matx33d r(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 4.0);

}  // namespace

KalmanFilter::KalmanFilter() {
  p = identity;
}

cv::Vec3d KalmanFilter::Update(const cv::Vec3d& observation) {
  cv::Vec3d z = observation - x;
  p += q;
  cv::Matx33d k = p * (p + r).inv();
  x = x + k * z;
  p = (identity - k) * p;
  return x;
}

}  // namespace algos