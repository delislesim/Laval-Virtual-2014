#pragma once

#include <opencv2/core/core.hpp>

namespace algos {

// Cree un tableau dans lequel chaque point contient la somme des points
// adjacents (à gauche et à droite).
// |window_size| est le nombre de points à considérer de chaque côté.
void SlidingWindow(const std::vector<double>& values,
                   size_t window_size,
                   std::vector<double>* sliding_window);

}  // namespace algos