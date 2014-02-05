#include "hsklu.h"

namespace hskl{

void Tracker::ObserveDepth(const cv::Mat& depthMat, const kinect_wrapper::KinectSensorData& data)
{
	dimx = depthMat.cols;
	dimy = depthMat.rows;
	reinterpret_cast<unsigned short*>(const_cast<uchar*>(depthMat.ptr()));
}

void Tracker::ObserveColor( const cv::Mat& irMat, const kinect_wrapper::KinectSensorData& sensorData )
{

}

}