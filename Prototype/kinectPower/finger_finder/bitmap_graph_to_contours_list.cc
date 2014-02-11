#include "finger_finder/bitmap_graph_to_contours_list.h"

#include "image/image_utility.h"

namespace finger_finder {

void BitmapGraphToContoursList(cv::Mat* bitmap_graph,
                               ContoursList* contours_list) {
  assert(bitmap_graph);
  assert(bitmap_graph->type() == CV_32S);
  assert(contours_list);
  assert(contours_list->empty());

  contours_list->reserve(500);

  MAT_PTR_PTR(bitmap_graph, int);

  // Commencer de la fin. C'est important de commencer du même endroit
  // que le constructeur de graphe.
  int num_pixels = bitmap_graph->total();
  for (int i = num_pixels - 1; i > 0; --i) {
    if (bitmap_graph_ptr[i] == -1)
      continue;

    // Parcourir le contour.
    size_t contour_index = contours_list->size();
    contours_list->push_back(std::vector<ContourPointInfo>());
    std::vector<ContourPointInfo>& contour = (*contours_list)[contour_index];
    contour.reserve(10);

    int next_pixel = static_cast<int>(i);
    while (next_pixel != -1) {
      int current_pixel = next_pixel;

      // Ajouter le contour à la liste.
      contour.push_back(ContourPointInfo(current_pixel, 0.0));
      next_pixel = bitmap_graph_ptr[current_pixel];

      // Se rappeler que ce pixel a été traité.
      bitmap_graph_ptr[current_pixel] = -1;
    }
  }
}

}  // namespace finger_finder
