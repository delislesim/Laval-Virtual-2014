#include "finger_finder/bitmap_graph.h"

#include "image/image_utility.h"

namespace finger_finder {

namespace {

// Rayon (carré) dans lequel faire la moyenne.
const int kRadius = 5;

int AverageDepthInternal(const cv::Point& position, const cv::Mat& depth_mat,
                         unsigned short min_depth, unsigned short max_depth,
                         int radius) {
  assert(depth_mat.type() == CV_16U);

  int sum = 0;
  int num_points = 0;

  for (int y = position.y - radius; y < position.y + radius; ++y) {
    for (int x = position.x - radius; x < position.x + radius; ++x) {
      cv::Point at(x, y);
      if (!image::OutOfBoundaries(depth_mat, at)) {
        int depth = depth_mat.at<unsigned short>(at);
        if (depth > min_depth && depth < max_depth) {
          sum += depth;
          ++num_points;
        }
      }
    }
  }

  if (num_points == 0)
    return 0;

  int average_depth = sum / num_points;
  return average_depth;
}

}  // namespace

int AverageDepth(const cv::Point& position, const cv::Mat& depth_mat,
                 unsigned short min_depth, unsigned short max_depth) {
  assert(depth_mat.type() == CV_16U);

  int average_depth = AverageDepthInternal(position, depth_mat,
                                           min_depth, max_depth,
                                           kRadius);

  // Si jamais aucun point n'était à la bonne profondeur, doubler le rayon
  // de recherche.
  if (average_depth == 0) {
    average_depth = AverageDepthInternal(position, depth_mat,
                                         min_depth, max_depth,
                                         kRadius * 2);
  }

  return average_depth;
}

}  // namespace finger_finder
