#pragma once

#include <opencv2/core/core.hpp>
#include <stack>

namespace bitmap_graph {

class BitmapGraphBuilder;

class BitmapRun {
 public:
  BitmapRun();

  template <typename BuilderType>
  void Run(cv::Mat* bitmap, BuilderType* builder);

 private:

};

// Utility functions.
struct IntersectionInfo {
  IntersectionInfo(int index, int num_children)
      : index(index), num_children(num_children) {}

  int index;
  int num_children;
};

inline cv::Point PositionOfIndex(int index, cv::Mat* bitmap) {
  return cv::Point(index % bitmap->cols, index / bitmap->cols);
}

inline int IndexOfPosition(const cv::Point& position, cv::Mat* bitmap) {
  return bitmap->cols * position.y + position.x;
}

inline void PushPixelIfWhite(int index, std::stack<int>* pile_pixels_a_traiter,
                             unsigned char* bitmap_ptr, int* num_voisins) {
  if (bitmap_ptr[index] != 0) {
    pile_pixels_a_traiter->push(index);
    bitmap_ptr[index] = 0;
    ++(*num_voisins);
  }
}

// Runner.
// |bitmap| est modifié au cours de l'exécution de l'algorithme.
template <typename BuilderType>
void BitmapRun::Run(cv::Mat* bitmap, BuilderType* builder) {
  assert(bitmap->type() == CV_8U);
  assert(builder);

  unsigned char* bitmap_ptr = bitmap->ptr();

  // Parcourir tous les bits de l'image à partir de la fin.
  for (int component_start_index = bitmap->total() - 1;
       component_start_index >= 0; --component_start_index) {
    // Traiter le pixel seulement s'il n'est pas noir.
    if (bitmap_ptr[component_start_index] == 0)
      continue;

    // Pile des intersections rencontrées.
    std::stack<IntersectionInfo> pile_intersections;
    pile_intersections.push(IntersectionInfo(-1, -1));

    // Pile des pixels à traiter.
    std::stack<int> pile_pixels_a_traiter;

    // Commencer une nouvelle composante du graphe.
    builder->ObserveComponentStart();

    // Mettre le premier pixel de la composante sur la pile des pixels à traiter
    // et le mettre noir pour ne plus qu'il soit considéré.
    pile_pixels_a_traiter.push(component_start_index);
    bitmap_ptr[component_start_index] = 0;

    while (!pile_pixels_a_traiter.empty()) {
      int index = pile_pixels_a_traiter.top();
      pile_pixels_a_traiter.pop();
      cv::Point position(PositionOfIndex(index, bitmap));

      // Trouver tous les voisins du pixel et les mettre sur la pile des pixels
      // à traiter.
      //
      // 1 4 6
      // 2   7
      // 3 5 8
      int index_1 = IndexOfPosition(cv::Point(position.x - 1, position.y - 1), bitmap);
      int index_2 = IndexOfPosition(cv::Point(position.x - 1, position.y    ), bitmap);
      int index_3 = IndexOfPosition(cv::Point(position.x - 1, position.y + 1), bitmap);
      int index_4 = IndexOfPosition(cv::Point(position.x    , position.y - 1), bitmap);
      int index_5 = IndexOfPosition(cv::Point(position.x    , position.y + 1), bitmap);
      int index_6 = IndexOfPosition(cv::Point(position.x + 1, position.y - 1), bitmap);
      int index_7 = IndexOfPosition(cv::Point(position.x + 1, position.y    ), bitmap);
      int index_8 = IndexOfPosition(cv::Point(position.x + 1, position.y + 1), bitmap);

      int num_voisins = 0;
      if (position.x > 0) {
        // 1
        if (position.y > 0)
          PushPixelIfWhite(index_1, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

        // 2
        PushPixelIfWhite(index_2, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

        // 3
        if (position.y < bitmap->rows - 1)
          PushPixelIfWhite(index_3, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);
      }

      // 4
      if (position.y > 0)
        PushPixelIfWhite(index_4, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

      // 5
      if (position.y < bitmap->rows - 1)
        PushPixelIfWhite(index_5, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

      if (position.x < bitmap->cols - 1) {
        // 6
        if (position.y > 0)
          PushPixelIfWhite(index_6, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

        // 7
        PushPixelIfWhite(index_7, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);

        // 8
        if (position.y < bitmap->rows - 1)
          PushPixelIfWhite(index_8, &pile_pixels_a_traiter, bitmap_ptr, &num_voisins);
      }

      if (num_voisins > 1) {
        // Si le pixel a plus d'un enfant, il s'agit d'une intersection.
        builder->ObserveIntersectionStart(index);
        pile_intersections.push(IntersectionInfo(index, num_voisins));
      } else if (num_voisins == 0) {
        // Si le pixel n'a aucun voisin, il s'agit d'une feuille.
        builder->ObserveLeaf(index);

        // Indiquer qu'on a fini de traiter une branche de l'intersection
        // précédente.
        --pile_intersections.top().num_children;
        while (pile_intersections.top().num_children == 0) {
          builder->ObserveIntersectionEnd();
          pile_intersections.pop();
          --pile_intersections.top().num_children;
        }

      } else {
        assert(num_voisins == 1);

        // Sinon, il s'agit d'un pixel bien ordinaire.
        builder->ObservePixel(index);
      }

    }

    // Finir la composante du graphe.
    builder->ObserveComponentEnd();

  }
}


}  // bitmap_graph