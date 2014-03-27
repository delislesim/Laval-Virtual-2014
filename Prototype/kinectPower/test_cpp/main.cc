#include <iostream>
#include "kinect_lib/lib.h"

int main() {
  Initialize();
  InitializeHandTracker();

  for (;;) {
    float positions[25 * 3];
    float orientations[25 * 4];
    int32_t tracking_state[25];
    int32_t is_new = false;

    if (GetJoints(positions, orientations, tracking_state, &is_new) && is_new) {
      for (int i = 0; i < 25; ++i) {
        std::cout << i << ": " << tracking_state[i] << std::endl;
      }

      std::cout << positions[3 * 3 + 0] << ", " << positions[3 * 3 + 1] << ", " << positions[3 * 3 + 2] << std::endl;
    } else {
      //std::cout << "no" << std::endl;
    }

    /*
    float positions_depth[25 * 3];
    if (GetJointsPositionDepth(positions_depth)) {
      std::cout << positions_depth[3 * 3 + 0] << ", " << positions_depth[3 * 3 + 1] << ", " << positions_depth[3 * 3 + 2] << std::endl;
    }
    */
  }
}