#include <iostream>
#include "kinect_lib/lib.h"

int main() {
  Initialize();
  InitializeHandTracker();

  for (;;) {
    float positions[25 * 3];
    float orientations[25 * 4];
    int tracking_state[25];
    int is_new = false;

    if (GetJoints(positions, orientations, tracking_state, &is_new) && is_new) {
      std::cout << positions[3 * 3 + 0] << ", " << positions[3 * 3 + 1] << ", " << positions[3 * 3 + 2] << std::endl;
    } else {
      //std::cout << "no" << std::endl;
    }
  }
}