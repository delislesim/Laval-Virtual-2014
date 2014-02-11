#include "finger_finder/bitmap_graph.h"

#include "image/image_utility.h"

namespace finger_finder {

namespace {

// Nombre de points à obtenir pour une bonne moyenne.
const int kNumPoints = 20;

// Distance maximum à parcourir.
const int kMaxDistance = 25;

// Largeur à considérer de chaque côté de la ligne de direction.
const int kAverageWidth = 5;

inline bool AddPoint(const cv::Point& point, const cv::Mat& depth_mat,
                     unsigned short min_depth, unsigned short max_depth,
                     int* sum, int* num_points) {
  if (!image::OutOfBoundaries(depth_mat, point)) {
    unsigned short depth = depth_mat.at<unsigned short>(point);
    if (depth > min_depth && depth < max_depth) {
      *sum += depth;
      *num_points += 1;
      return true;
    }
  }
  return false;
}

}  // namespace

int AverageDepth(const cv::Point& position, const cv::Vec2i direction,
                 const cv::Mat& depth_mat,
                 unsigned short min_depth, unsigned short max_depth) {
  assert(depth_mat.type() == CV_16U);

  if (direction.val[0] == 0 && direction.val[1] == 0)
    return (min_depth + max_depth) / 2;

  // Normaliser le vecteur de direction.
  double norm_direction = cv::norm(direction);
  cv::Vec2d normalized_direction(
    static_cast<double>(direction.val[0]) / norm_direction,
    static_cast<double>(direction.val[1]) / norm_direction
  );
  cv::Vec2d position_double(
    static_cast<double>(position.x),
    static_cast<double>(position.y)
  );

  int num_points = 0;
  int sum = 0;
  int distance = 0;

  int last_y = -1;

  while (distance < kMaxDistance && num_points < kNumPoints) {
    cv::Vec2d middle = position_double + normalized_direction * distance;
    cv::Point middle_point(
      static_cast<int>(middle.val[0]),
      static_cast<int>(middle.val[1])
    );
    distance += 1.0;

    if (middle_point.y == last_y)
      continue;

    // Le point milieu.
    AddPoint(middle_point, depth_mat, min_depth, max_depth, &sum, &num_points);

    // Vers la gauche.
    for (int x = middle_point.x - 1; x > middle_point.x - kAverageWidth; --x) {
      if (!AddPoint(cv::Point(x, middle_point.y), depth_mat, min_depth, max_depth, &sum, &num_points))
        break;
    }

    // Vers la droite.
    for (int x = middle_point.x + 1; x < middle_point.x + kAverageWidth; ++x) {
      if (!AddPoint(cv::Point(x, middle_point.y), depth_mat, min_depth, max_depth, &sum, &num_points))
        break;
    }

    // Se rappeler du y qu'on vient de visiter.
    last_y = middle_point.y;
  }

  if (num_points == 0)
    return (min_depth + max_depth) / 2;

  int average_depth = sum / num_points;
  return average_depth;
}

}  // namespace finger_finder
