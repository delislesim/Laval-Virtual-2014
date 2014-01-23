#include "kinect_wrapper/kinect_sensor.h"

extern "C" bool __declspec(dllexport)
Initialize();

extern "C" bool __declspec(dllexport)
Shutdown();

extern "C" bool __declspec(dllexport)
RecordSensor(int sensor_index, const char* filename);

extern "C" bool __declspec(dllexport)
StartPlaySensor(int sensor_index, const char* filename);

extern "C" bool __declspec(dllexport)
PlayNextFrame(int sensor_index);

extern "C" bool __declspec(dllexport)
GetNiceDepthMap(unsigned char* pixels, unsigned int pixels_size);

extern "C" bool __declspec(dllexport)
GetJointsPosition(int skeleton_id, float* joint_positions);

extern "C" bool __declspec(dllexport)
GetBonesOrientation(int skeleton_id,
                    NUI_SKELETON_BONE_ORIENTATION* bone_orientations);

extern "C" bool __declspec(dllexport)
GetPianoInfo(unsigned char* notes, unsigned int notes_size,
             unsigned char* pixels, unsigned int pixels_size);
