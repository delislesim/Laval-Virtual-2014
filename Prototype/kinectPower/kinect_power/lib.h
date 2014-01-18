#include "kinect_wrapper/kinect_sensor.h"

extern "C" bool __declspec(dllexport)
Initialize();

extern "C" bool __declspec(dllexport)
Shutdown();

extern "C" bool __declspec(dllexport)
GetNiceDepthMap(unsigned char* pixels, unsigned int pixels_size);

extern "C" bool __declspec(dllexport)
GetPianoInfo(bool* notes, unsigned int notes_size, unsigned char* pixels, unsigned int pixels_size);
