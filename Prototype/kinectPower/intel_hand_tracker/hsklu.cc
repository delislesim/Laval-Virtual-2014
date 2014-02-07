#ifdef USE_INTEL_CAMERA

#include "hsklu.h"
#include "kinect_wrapper/constants.h"

namespace hskl{

void Tracker::ObserveDepth(const cv::Mat& depthMat, const kinect_wrapper::KinectSensorData& data)
{
	kinectDepth = reinterpret_cast<const unsigned short*>(depthMat.ptr());
	cv::Mat* colorMat = new cv::Mat();
	if(data.QueryColor(colorMat))
	{
		kinectColor = reinterpret_cast<const unsigned short*>(colorMat->ptr());
		Update();
	}
}

void Tracker::ActivateKinect()
{
	IsKinectActivated = true;
	kinect_wrapper::KinectWrapper::instance()->AddObserver(0, this);
  kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0)->color_stream_type(NUI_IMAGE_TYPE_COLOR_INFRARED);
  kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(0)->depth_stream_type(NUI_IMAGE_TYPE_DEPTH);
	dimx = kinect_wrapper::kKinectDepthWidth;
	dimy = kinect_wrapper::kKinectDepthHeight;

	depth = new unsigned short[dimx*dimy];
	kinectColor  = new unsigned short[dimx*dimy*3];
	
	fovx = maths::DegreesToRad(NUI_CAMERA_COLOR_NOMINAL_HORIZONTAL_FOV);
	fovy = maths::DegreesToRad(NUI_CAMERA_COLOR_NOMINAL_VERTICAL_FOV);

	hsklSetSensorProperties(tracker, HSKL_SENSOR_CREATIVE, dimx, dimy, fovx, fovy);
}

}

#endif