#include "kinect_wrapper/kinect_sensor.h"

extern "C" bool __declspec(dllexport)
GetNiceDepthMap(unsigned char* pixels, int buffer_size);

extern "C" int __declspec(dllexport)
tutu() {
   return 2;
}