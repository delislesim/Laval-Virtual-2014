#include "base/timer.h"

#include "base/logging.h"

namespace base {

Timer::Timer()
    : started_(false) {
  frequency_.QuadPart = 0;
  start_time_.QuadPart = 0;
}

Timer::~Timer() {
}

void Timer::Start() {
  QueryPerformanceFrequency(&frequency_);
  QueryPerformanceCounter(&start_time_);
}

double Timer::ElapsedTime() {
  LARGE_INTEGER stop_time;
  QueryPerformanceCounter(&stop_time);
  double elapsed_time = (stop_time.QuadPart - start_time_.QuadPart)
      * 1000.0 / frequency_.QuadPart;
  return elapsed_time;
}

}  // namespace base

