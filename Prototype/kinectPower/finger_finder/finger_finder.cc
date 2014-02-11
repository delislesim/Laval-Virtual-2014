#include "finger_finder/finger_finder.h"

#include <opencv2/imgproc/imgproc.hpp>

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

FingerFinder::FingerFinder(unsigned short min_hands_depth,
                           unsigned short max_hands_depth)
    : min_hands_depth_(min_hands_depth),
      max_hands_depth_(max_hands_depth) {
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

  // Retourner une nice image temporaire.
  *nice_image = cv::Mat(depth_mat_truncated.size(), CV_8UC4);
  
  MAT_PTR(depth_mat_truncated, unsigned short);
  MAT_PTR_PTR(nice_image, unsigned char);

  FOR_MATRIX(i, *nice_image) {
    unsigned char normalized_depth = 0;
    if (depth_mat_truncated_ptr[i] < 2000) {
      normalized_depth = (unsigned char)((double)depth_mat_truncated_ptr[i] * 255.0 / 2000.0);
    } else {
      normalized_depth = 255;
    }

    nice_image_ptr[i * 4 + image::kRedIndex] = normalized_depth;
    nice_image_ptr[i * 4 + image::kBlueIndex] = normalized_depth;
    nice_image_ptr[i * 4 + image::kGreenIndex] = normalized_depth;
    nice_image_ptr[i * 4 + image::kAlphaIndex] = 255;
  }
}

void FingerFinder::QueryFingers(std::vector<FingerInfo>* fingers) {
  assert(fingers);
  assert(fingers->empty());

  // Copier les dernières données calculées dans le vecteur passée en paramètre.
  *fingers = fingers_;
}

}  // namespace finger_finder
