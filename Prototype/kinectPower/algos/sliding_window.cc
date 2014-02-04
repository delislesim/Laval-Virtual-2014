#include "algos/sliding_window.h"

#include <cmath>

namespace algos {

void SlidingWindow(const std::vector<double>& values,
                   size_t window_size,
                   std::vector<double>* sliding_window) {
  assert(sliding_window);
  
  *sliding_window = std::vector<double>(values.size());

  double sum = 0.0;
  for (size_t i = 0; i < values.size() + window_size * 2; ++i) {
    sum += values[i % values.size()];
    if (i > window_size * 2) {
      sum -= values[abs(static_cast<int>((i - window_size * 2 - 1) % values.size()))];
    }

    if (i >= window_size * 2) {
      (*sliding_window)[abs(static_cast<int>((i - window_size) % values.size()))] = sum;
    }
  }
}

}  // namespace algos