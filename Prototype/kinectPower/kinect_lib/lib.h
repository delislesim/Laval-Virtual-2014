#ifdef USE_INTEL_CAMERA
#include "hsklu.h"
#endif

#include "creative/joint_info.h"
#include "kinect_wrapper/kinect_include.h"

extern "C" bool __declspec(dllexport)
Initialize();

extern "C" bool __declspec(dllexport)
Shutdown();

extern "C" bool __declspec(dllexport)
GetJoints(float* positions, float* orientations, int* tracking_state, int* is_new);

extern "C" bool __declspec(dllexport)
GetJointsPositionDepth(int* joint_positions);

extern "C" bool __declspec(dllexport)
AvoidCurrentSkeleton();

// Hand tracker.
#ifdef USE_INTEL_CAMERA

extern "C" bool __declspec(dllexport)
InitializeHandTracker();

extern "C" bool __declspec(dllexport)
GetHandsSkeletons(creative::JointInfo* joints);

extern "C" bool __declspec(dllexport)
SetHandMeasurements(float width, float height);

#endif
