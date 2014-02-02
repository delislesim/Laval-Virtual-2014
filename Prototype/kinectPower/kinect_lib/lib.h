#include "hsklu.h"
#include "kinect_wrapper/kinect_include.h"

extern "C" bool __declspec(dllexport)
Initialize(bool near_mode, bool with_sensor_thread);

extern "C" bool __declspec(dllexport)
Shutdown();

extern "C" bool __declspec(dllexport)
RecordSensor(int sensor_index, const char* filename);

extern "C" bool __declspec(dllexport)
StartPlaySensor(int sensor_index, const char* filename);

extern "C" bool __declspec(dllexport)
PlayNextFrame(int sensor_index);

extern "C" bool __declspec(dllexport)
GetDepthImage(unsigned char* pixels, unsigned int pixels_size);

extern "C" bool __declspec(dllexport)
GetColorImage(unsigned char* pixels, unsigned int pixels_size);

extern "C" bool __declspec(dllexport)
GetJointsPosition(int skeleton_id, float* joint_positions,
                  unsigned char* joint_status);

extern "C" bool __declspec(dllexport)
GetBonesOrientation(int skeleton_id,
                    NUI_SKELETON_BONE_ORIENTATION* bone_orientations);

extern "C" bool __declspec(dllexport)
GetJointsPositionDepth(int skeleton_id, int* joint_positions);

extern "C" bool __declspec(dllexport)
GetHandsInteraction(int skeleton_id, NUI_HANDPOINTER_INFO* hands);

// Hand tracker.

extern "C" bool __declspec(dllexport)
InitializeHandTracker();

extern "C" bool __declspec(dllexport)
GetHandsSkeletons(hskl::float3* joint_positions, float* tracking_error);

// Piano experiments.
extern "C" bool __declspec(dllexport)
GetPianoImage(unsigned char* pixels, unsigned int pixels_size);

extern "C" bool __declspec(dllexport)
GetPianoHands(unsigned int* positions, unsigned char* known);


