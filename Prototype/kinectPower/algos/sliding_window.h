#pragma once

#include <opencv2/core/core.hpp>

namespace algos {

// Cree un tableau dans lequel chaque point contient la somme des points
// adjacents (� gauche et � droite).
// |window_size| est le nombre de points � consid�rer de chaque c�t�.
void SlidingWindow(const std::vector<double>& values,
                   size_t window_size,
                   std::vector<double>* sliding_window);

}  // namespace algos