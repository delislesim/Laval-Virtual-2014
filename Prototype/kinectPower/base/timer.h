#pragma once

// Restrict the import to the windows basic includes.
#define WIN32_LEAN_AND_MEAN
#include <windows.h>  // NOLINT

#include "base/base.h"

namespace base {

class Timer {
 public:
  Timer();
  ~Timer();

  void Start();

  // @returns the elapsed time in milliseconds.
  double ElapsedTime();

 private:
  bool started_;
  LARGE_INTEGER frequency_;
  LARGE_INTEGER start_time_;

  DISALLOW_COPY_AND_ASSIGN(Timer);
};

}  // namespace base
