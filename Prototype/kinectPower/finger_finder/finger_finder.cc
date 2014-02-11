#include "finger_finder/finger_finder.h"

#include <opencv2/imgproc/imgproc.hpp>

#include "finger_finder/canny_contour.h"
#include "finger_finder/depth_translator.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"

namespace finger_finder {

namespace {

// Région de l'image de couleur qui nous intéresse pour la détection des
// mains.
const cv::Rect kRegionOfInterest(0, 100,
                                 kinect_wrapper::kKinectColorWidth, 280);

}  // namespace

FingerFinder::FingerFinder(unsigned short target_depth,
                           unsigned short depth_tolerance)
    : min_hands_depth_(target_depth - depth_tolerance),
      max_hands_depth_(target_depth + depth_tolerance) {
  assert(depth_tolerance <= target_depth);
}

void FingerFinder::ObserveData(const cv::Mat& depth_mat,
                               const cv::Mat& color_mat,
                               cv::Mat* nice_image) {
  // S'assurer que les paramètres reçus sont du type voulu.
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat.cols ==
      static_cast<int>(kinect_wrapper::kKinectDepthWidth));
  assert(depth_mat.rows ==
      static_cast<int>(kinect_wrapper::kKinectDepthHeight));

  assert(color_mat.type() == CV_8UC4);
  assert(color_mat.cols ==
      static_cast<int>(kinect_wrapper::kKinectColorWidth));
  assert(color_mat.rows ==
      static_cast<int>(kinect_wrapper::kKinectColorHeight));

  assert(nice_image);

  // Générer une image de profondeur utilisant les coordonnées de l'image
  // couleur.
  cv::Mat depth_mat_color_coordinates;
  DepthTranslator(0, depth_mat, &depth_mat_color_coordinates);

  // Tronquer les matrices utilisées pour le reste de l'algorithme pour ne
  // garder que la portion qui nous intéresse.
  cv::Mat depth_mat_truncated = depth_mat_color_coordinates(kRegionOfInterest);
  cv::Mat color_mat_truncated = color_mat(kRegionOfInterest);

  // Passer le résultat à la méthode qui exécute le coeur de l'algorithme.
  ObserveDataInternal(depth_mat_truncated, color_mat_truncated, nice_image);
}

void FingerFinder::ObserveDataInternal(const cv::Mat& depth_mat,
                                       const cv::Mat& color_mat,
                                       cv::Mat* nice_image) {
  assert(depth_mat.size() == kRegionOfInterest.size());
  assert(depth_mat.type() == CV_16U);

  assert(color_mat.size() == kRegionOfInterest.size());
  assert(color_mat.type() == CV_8UC4);

  assert(nice_image);

  // Extraire des contours de l'image de couleur.
  cv::Mat contours;
  CannyContour(color_mat, &contours);

  // Enlever tout ce qui n'est pas à la bonne profondeur. Étant donné que
  // l'image de profondeur n'est pas très précise, on la dilate un peu avant
  // d'appliquer le masque.
  cv::Mat target_depth = depth_mat < max_hands_depth_ & depth_mat > 0;
  cv::dilate(target_depth, target_depth, image::kRoundedDilater,
             image::kRoundedDilaterCenter, image::kIteration6);
  contours = contours & target_depth;

  // Affichage.
  cv::cvtColor(contours, *nice_image, CV_GRAY2RGBA);

}

void FingerFinder::QueryFingers(std::vector<FingerInfo>* fingers) {
  assert(fingers);
  assert(fingers->empty());

  // Copier les dernières données calculées dans le vecteur passée en paramètre.
  *fingers = fingers_;
}

}  // namespace finger_finder
