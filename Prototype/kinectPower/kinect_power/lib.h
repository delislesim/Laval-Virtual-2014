#include "kinect_wrapper/kinect_sensor.h"

extern "C" bool __declspec(dllexport)
Initialize();

extern "C" bool __declspec(dllexport)
Shutdown();

extern "C" bool __declspec(dllexport)
GetNiceDepthMap(unsigned char* pixels, unsigned int buffer_size);
