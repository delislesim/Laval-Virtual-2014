#include "kinect_wrapper/kinect_sensor.h"

extern "C" bool __declspec(dllexport)
GetNiceDepthMap(const char* pixels);

extern "C" int __declspec(dllexport)
tutu() {
   return 2;
}