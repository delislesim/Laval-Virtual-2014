#include "finger_finder/finger_finder.h"

#include <opencv2/imgproc/imgproc.hpp>

#include "finger_finder/average_depth.h"
#include "finger_finder/bitmap_graph_to_contours_list.h"
#include "finger_finder/canny_contour.h"
#include "finger_finder/contour_angle.h"
#include "finger_finder/depth_translator.h"
#include "image/image_constants.h"
#include "image/image_utility.h"
#include "kinect_wrapper/constants.h"

namespace finger_finder {

namespace {

// TODO(fdoray): Ceci est un doublon de la constante déclarée dans
// contour_angle.cc.
const int kNumPixelsForAngle = 20;

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

  // Calculer les angles de chaque point des contours.
  ContoursList contours_list;
  ContourAngle(contours, &contours_list);

  // Créer une liste de doigts.
  FingerInfoVector finger_info_vector;

  for (size_t contour_index = 0; contour_index < contours_list.size();
       ++contour_index) {
    int end_point = static_cast<int>(contours_list[contour_index].size()) -
        kNumPixelsForAngle;

    for (int point_index = kNumPixelsForAngle; point_index < end_point;
         ++point_index) {
      // Considérer le point si son angle est non nul et qu'il est plus haut
      // que ses voisins.
      ContourPointInfo& contour_point_info =
          contours_list[contour_index][point_index];

      int current_index = contour_point_info.index;
      int previous_index =
          contours_list[contour_index][point_index - kNumPixelsForAngle].index;
      int next_index =
          contours_list[contour_index][point_index + kNumPixelsForAngle].index;

      cv::Point current_position(
          image::PositionOfIndex(current_index, depth_mat));
      cv::Point previous_position(
          image::PositionOfIndex(previous_index, depth_mat));
      cv::Point next_position(
          image::PositionOfIndex(next_index, depth_mat));

      if (contour_point_info.angle != 0.0 &&
          current_position.y < previous_position.y &&
          current_position.y < next_position.y) {
        // On a trouvé un doigt potentiel.

        // Calculer sa profondeur.
        cv::Vec2i direction_previous = previous_position - current_position;
        cv::Vec2i direction_next = next_position - current_position;
        cv::Vec2i direction_average = (direction_previous + direction_next) / 2;

        cv::Point depth_center(current_position.x, current_position.y - 10);

        int depth = AverageDepth(depth_center, direction_average, depth_mat,
                                 min_hands_depth_, max_hands_depth_);

        // L'ajouter à la liste des doigts potentiels.
        FingerInfo finger_info(current_position, depth);
        finger_info_vector.push_back(finger_info);
      }
    }
  }

  // Copier la liste de doigts résultante.
  fingers_ = finger_info_vector;

  // --- Affichage ---

  // Faire un contour avec seulement les éléments significatifs
  // (au moins 10 pixels).
  cv::Mat contours_clean(depth_mat.size(), CV_8U, cv::Scalar(0));
  MAT_PTR(contours_clean, unsigned char);

  for (size_t i = 0; i < contours_list.size(); ++i) {
    if (contours_list[i].size() >= 10) {
      for (size_t j = 0; j < contours_list[i].size(); ++j) {
        contours_clean_ptr[contours_list[i][j].index] = 255;
      }
    }
  }

  // Afficher l'image de profondeur en RGB.
  *nice_image = cv::Mat(depth_mat.size(), CV_8UC4, cv::Scalar(0, 0, 0, 255));

  MAT_PTR(depth_mat, const unsigned short);
  MAT_PTR_PTR(nice_image, unsigned char);

  FOR_MATRIX(i, depth_mat) {
    unsigned short depth_val = depth_mat_ptr[i];
    if (depth_val > min_hands_depth_ && depth_val < max_hands_depth_) {
      unsigned char normalized_depth = 255.0 -
          static_cast<double>(depth_val - min_hands_depth_) * 255.0 /
          (max_hands_depth_ - min_hands_depth_);
      
      nice_image_ptr[i * 4 + image::kRedIndex] = 0;
      nice_image_ptr[i * 4 + image::kGreenIndex] = 0;
      nice_image_ptr[i * 4 + image::kBlueIndex] = normalized_depth;
    }

    if (contours_clean_ptr[i] != 0) {
      nice_image_ptr[i * 4 + image::kRedIndex] = 255;
      nice_image_ptr[i * 4 + image::kGreenIndex] = 0;
      nice_image_ptr[i * 4 + image::kBlueIndex] = 0;
    }
  }


  //cv::cvtColor(contours_clean, *nice_image, CV_GRAY2RGBA);

  // Afficher l'im

}

void FingerFinder::QueryFingers(std::vector<FingerInfo>* fingers) {
  assert(fingers);
  assert(fingers->empty());

  // Copier les dernières données calculées dans le vecteur passée en paramètre.
  *fingers = fingers_;
}

}  // namespace finger_finder
