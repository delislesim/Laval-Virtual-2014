#pragma once

#include <queue> 
#include <utility>
#include <vector>

namespace algos {

struct StableMatchingPair {
  int left;
  int right;
  int distance;

  bool operator<(const StableMatchingPair& other) const;
};

typedef std::priority_queue<StableMatchingPair> StableMatchingQueue;

// @param best_pairs vector in which indexes are indexes from the left
//    side of the pairs and values are the best match. -1 indicates that
//    there is no match for this left value.
void StableMatching(size_t num_left, size_t num_right,
                    int max_distance,
                    StableMatchingQueue* potential_pairs,
                    std::vector<int>* best_pairs);

}  // namespace hand_extractor