#ifdef USE_INTEL_CAMERA
#include "hsklu.h"
#endif

#include "creative/joint_info.h"
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

extern "C" bool __declspec(dllexport)
GetFaceRotation(float* face_rotation);

// Hand tracker.
#ifdef USE_INTEL_CAMERA

extern "C" bool __declspec(dllexport)
InitializeHandTracker();

extern "C" bool __declspec(dllexport)
GetHandsSkeletons(creative::JointInfo* joints);

#endif
