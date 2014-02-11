#include "finger_finder/contour_angle.h"

#include "algos/maths.h"
#include "finger_finder/bitmap_graph.h"
#include "finger_finder/bitmap_graph_to_contours_list.h"
#include "image/image_utility.h"

namespace finger_finder {

namespace {

// Nombre de pixels à aller voir de chaque côté pour calculer les angles.
// Exemple: l'angle du pixel 50 est l'angle 30<--50-->70
const int kNumPixelsForAngle = 20;

}  // namespace

void ContourAngle(const cv::Mat& contours, ContoursList* contours_list_param) {
  assert(contours.type() == CV_8U);
  assert(contours_list_param);

  ContoursList& contours_list = *contours_list_param;

  // Créer un graphe à partir des contours.
  cv::Mat bitmap_graph;
  BitmapGraph(contours, &bitmap_graph);

  // Créer des listes contenant les points de chaque contour dans l'ordre.
  BitmapGraphToContoursList(&bitmap_graph, &contours_list);

  // Calculer les angles pour chaque point de contour.
  for (size_t contour_index = 0; contour_index < contours_list.size();
       ++contour_index) {
    int end_point = static_cast<int>(contours_list[contour_index].size()) -
        kNumPixelsForAngle;

    for (int point_index = kNumPixelsForAngle;
         point_index < end_point; ++point_index) {
      cv::Point before(image::PositionOfIndex(
          contours_list[contour_index][point_index - kNumPixelsForAngle].index,
          contours));
      cv::Point center(image::PositionOfIndex(
          contours_list[contour_index][point_index].index,
          contours));
      cv::Point after(image::PositionOfIndex(
          contours_list[contour_index][point_index + kNumPixelsForAngle].index,
          contours));

      double angle = abs(maths::AngleBetween(before, center, after));

      contours_list[contour_index][point_index].angle = angle;
    }
  }

  // Tuer les angles qui sont inférieurs à un voisin proche.
  for (size_t contour_index = 0; contour_index < contours_list.size();
       ++contour_index) {
    int end_point = static_cast<int>(contours_list[contour_index].size()) -
        kNumPixelsForAngle;

    for (int point_index = kNumPixelsForAngle;
         point_index < end_point; ++point_index) {
      
      double angle = contours_list[contour_index][point_index].angle;
      if (angle == 0.0)
        continue;

      for (int neighbour_index = point_index - kNumPixelsForAngle;
           neighbour_index < point_index + kNumPixelsForAngle;
           ++neighbour_index) {
        if (contours_list[contour_index][neighbour_index].angle > angle) {
          contours_list[contour_index][neighbour_index].angle = 0.0;
        }
      }

      // Tant qu'à y être, aussi enlever les trop gros angles.
      if (angle > maths::kPi / 4.0)
        contours_list[contour_index][point_index].angle = 0.0;
    }
  }
}

}  // namespace finger_finder
