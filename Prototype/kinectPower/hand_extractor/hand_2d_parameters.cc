#include "hand_extractor/hand_2d_parameters.h"

namespace hand_extractor {

Hand2dParameters::Hand2dParameters()
    : joints_positions_(JOINTS_COUNT),
      joints_known_(JOINTS_COUNT) {
  // No joint is known at the beginning.
  for (int i = 0; i < JOINTS_COUNT; ++i)
    joints_known_[i] = false;
}

}  // namespace hand_extractor