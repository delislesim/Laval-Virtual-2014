#pragma once

#include "base/base.h"
#include "base/logging.h"
#include "kinect_wrapper/constants.h"

namespace kinect_wrapper {

template <typename T>
class KinectSwitch {
 public:
  KinectSwitch();
  ~KinectSwitch();

  void GetCurrent(T* current) const;
  void SetNext(const T& next);

  T* GetNextPtr();

 private:
  size_t current_buffer_index_;
  T frames_[kNumBuffers];

  DISALLOW_COPY_AND_ASSIGN(KinectSwitch);
};

template <typename T>
KinectSwitch<T>::KinectSwitch()
    : current_buffer_index_(0) {
}

template <typename T>
KinectSwitch<T>::~KinectSwitch() {
}

template <typename T>
void KinectSwitch<T>::GetCurrent(T* current) const {
  DCHECK(current);
  *current = frames_[current_buffer_index_];
}

template <typename T>
void KinectSwitch<T>::SetNext(const T& next) {
  size_t next_buffer_index = (current_buffer_index_ + 1) % kNumBuffers;
  frames_[next_buffer_index] = next;
  current_buffer_index_ = next_buffer_index;
}

template <typename T>
T* KinectSwitch<T>::GetNextPtr() {
  size_t next_buffer_index = (current_buffer_index_ + 1) % kNumBuffers;
  return &frames_[next_buffer_index];
}


}  // namespace kinect_wrapper