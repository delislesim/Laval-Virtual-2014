#include "algos/stable_matching.h"

#include "base/logging.h"

namespace algos {

namespace {


}  // namespace

bool StableMatchingPair::operator<(const StableMatchingPair& other) const {
  return distance > other.distance;
}

void StableMatching(size_t num_left, size_t num_right,
                    int max_distance,
                    StableMatchingQueue* potential_pairs,
                    std::vector<int>* best_pairs) {
  assert(potential_pairs);
  assert(best_pairs);
  assert(best_pairs->empty());

  *best_pairs = std::vector<int>(num_left, -1);

  std::vector<bool> left_taken(num_left, false);
  std::vector<bool> right_taken(num_right, false);

  size_t num_taken_left = 0;
  size_t num_taken_right = 0;

  while (!potential_pairs->empty() &&
         num_taken_left != num_left &&
         num_taken_right != num_right) {
    StableMatchingPair next_pair = potential_pairs->top();
    potential_pairs->pop();

    if (next_pair.distance > max_distance) {
      return;
    }

    if (left_taken[next_pair.left] || right_taken[next_pair.right]) {
      continue;
    }

    left_taken[next_pair.left] = true;
    right_taken[next_pair.right] = true;

    (*best_pairs)[next_pair.left] = next_pair.right;
  }
}

}  // namespace algos