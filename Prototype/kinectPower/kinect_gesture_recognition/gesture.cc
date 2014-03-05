#include "gesture.h"

Gesture::Gesture()
{
  achievedSteps_[0] = 0;
  achievedSteps_[1] = 0;
}

void Gesture::setNbSteps(unsigned int* nbSteps )
{
  nbSteps_ = nbSteps;
}

