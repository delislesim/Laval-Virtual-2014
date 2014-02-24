#include "bitmap_graph/bitmap_run.h"

#include <stack>

#include "base/logging.h"
#include "bitmap_graph/bitmap_graph_builder.h"

namespace bitmap_graph {

namespace {

const int kNumNeighbours = 8;
const int kNeighboursOffset[kNumNeighbours][2] = {
  {-1, -1}, { 0, -1}, { 1, -1},
  {-1,  0},           { 1,  0},
  {-1,  1}, { 0,  1}, { 1,  1}
};

// @param index index du pixel a traiter.
// @returns index du pixel qui suit le pixel à traiter.
int BuildBitmapGraphInternal(int index, unsigned char* bitmap_ptr,
                             int* graph_ptr, cv::Mat* graph, int* size) {
  // Si on ne fait pas partie d'un contour ou qu'on a déjà été traité,
  // le signaler au parent.
  if (bitmap_ptr[index] == 0) {
    return -1;
  }

  // Indiquer que ce pixel a été traité.
  bitmap_ptr[index] = 0;

  // Trouver des voisins prometteurs.
  cv::Point position(PositionOfIndex(index, graph));

  int best_neighbour_index = -1;
  int best_neighbour_size = 0;

  for (int i = 0; i < kNumNeighbours; ++i) {
    cv::Point neighbour(position.x + kNeighboursOffset[i][0],
                        position.y + kNeighboursOffset[i][1]);
    int neighbour_index = IndexOfPosition(neighbour, graph);

    if (neighbour.x >= 0 && neighbour.x < graph->cols &&
        neighbour.y >= 0 && neighbour.y < graph->rows &&
        bitmap_ptr[neighbour_index] != 0) {
      int neighbour_index = IndexOfPosition(neighbour, graph);
      int neighbour_size = -1;

      graph_ptr[neighbour_index] = BuildBitmapGraphInternal(neighbour_index, bitmap_ptr,
                                                            graph_ptr, graph,
                                                            &neighbour_size);

      if (neighbour_size > best_neighbour_size) {
        best_neighbour_index = neighbour_index;
        best_neighbour_size = neighbour_size;
      }
    }
  }

  *size = best_neighbour_size + 1;
  return best_neighbour_index;
}

}  // namespace

void BuildBitmapGraph(const cv::Mat& bitmap, cv::Mat* graph) {
  assert(graph);
  assert(bitmap.type() == CV_8U);

  *graph = cv::Mat(bitmap.size(), CV_32S, -1);

  cv::Mat bitmap_copy;
  bitmap.copyTo(bitmap_copy);

  unsigned char* bitmap_ptr = bitmap_copy.ptr();
  int* graph_ptr = reinterpret_cast<int*>(graph->ptr());

  // Commencer de la fin.
  for (size_t i = bitmap.total() - 1; i < bitmap.total(); --i) {
    if (bitmap_ptr[i] == 0)
      continue;

    // On a trouvé un pixel blanc non traité.
    int size = 0;
    graph_ptr[i] = BuildBitmapGraphInternal(i, bitmap_ptr, graph_ptr, graph, &size);
  }
}




BitmapRun::BitmapRun() {
}



}  // namespace bitmap_graph