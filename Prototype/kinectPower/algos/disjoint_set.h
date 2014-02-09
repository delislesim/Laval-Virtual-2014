#pragma once

#include <opencv2/core/core.hpp>

namespace algos {

class DisjointSet {
 public:

  DisjointSet(cv::Size size)
      : rank_(size, CV_8U, cv::Scalar(0)),
        kInvalidParent(-1) {
    parent_ = cv::Mat(size, CV_32S, cv::Scalar(kInvalidParent));
    rank_ptr_ = rank_.ptr();
    parent_ptr_ = reinterpret_cast<int*>(parent_.ptr());
  }

  void Union(int index_a, int index_b) {
    int root_a = Find(index_a);
    int root_b = Find(index_b);
    
    // Ne rien faire si les noeuds sont déjà dans le même set.
    if (root_a != kInvalidParent && root_a == root_b)
      return;

    // Si aucun des noeuds n'a déjà été ajouté à une composante.
    if (root_a == kInvalidParent && root_b == kInvalidParent) {
      parent_ptr_[index_a] = index_a;
      parent_ptr_[index_b] = index_a;
      rank_ptr_[index_b] += 1;
      return;
    } else if (root_a == kInvalidParent) {
      root_a = index_a;
    } else if (root_b == kInvalidParent) {
      root_b = index_b;
    }

    // Cas habituel.
    int rank_a = rank_ptr_[root_a];
    int rank_b = rank_ptr_[root_b];

    if (rank_a < rank_b) {
      parent_ptr_[root_a] = root_b;
    } else if (rank_a > rank_b) {
      parent_ptr_[root_b] = root_a;
    } else {
      parent_ptr_[root_b] = root_a;
      rank_ptr_[root_a] += 1;
    }

  }

  int Find(int index) {
    if (parent_ptr_[index] != index && parent_ptr_[index] != kInvalidParent)
      parent_ptr_[index] = Find(parent_ptr_[index]);
    return parent_ptr_[index];
  }

  bool SameSet(int index_a, int index_b) {
    return Find(index_a) == Find(index_b);
  }

 private:
  cv::Mat rank_;
  cv::Mat parent_;

  unsigned char* rank_ptr_;
  int* parent_ptr_;
  
  const int kInvalidParent;
};


}  // namespace algos