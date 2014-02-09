#include "face_tracker.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor_data.h"

namespace kinect_face_tracker {
	FaceTracker::FaceTracker(){
    /*
		face_tracker_ = FTCreateFaceTracker();
		color_image_ = FTCreateImage();
		depth_image_ = FTCreateImage();
		face_tracker_->CreateFTResult(&face_tracker_result_);
		is_tracking_ = false;
    */
	}

	FaceTracker::~FaceTracker(){
    /*
		color_image_->Release();
		depth_image_->Release();
		face_tracker_result_->Release();
		face_tracker_->Release();
    */
	}

	cv::Vec3f FaceTracker::FaceRotation(){
		FLOAT scale, rotationXYZ[3], translationXYZ[3];
		face_tracker_result_->Get3DPose(&scale, rotationXYZ, translationXYZ);

		cv::Vec3f rotation;
		rotation[0] = rotationXYZ[0];
		rotation[1] = rotationXYZ[1];
		rotation[2] = rotationXYZ[2];

		return rotation;
	}

	void FaceTracker::ObserveDepth(const cv::Mat& depth_mat, const kinect_wrapper::KinectSensorData& sensor_data){
    /*
		cv::Mat color_mat;
		if (!sensor_data.QueryColor(&color_mat))
			return;

		cv::Mat depth_mat_not_const = depth_mat;
		if (!sensor_data.QueryDepth(&depth_mat_not_const))
			return;

		color_image_->Attach(kinect_wrapper::kKinectColorWidth, kinect_wrapper::kKinectColorHeight,
			color_mat.ptr(), FTIMAGEFORMAT_UINT8_B8G8R8X8, kinect_wrapper::kKinectColorWidth * 4);
		depth_image_->Attach(kinect_wrapper::kKinectDepthWidth, kinect_wrapper::kKinectDepthHeight,
			depth_mat_not_const.ptr(), FTIMAGEFORMAT_UINT16_D16, kinect_wrapper::kKinectDepthWidth * 2);

		FT_SENSOR_DATA ft_sensor_data;
		ft_sensor_data.pDepthFrame = depth_image_;
		ft_sensor_data.pVideoFrame = color_image_;
		ft_sensor_data.ViewOffset.x = 0;
		ft_sensor_data.ViewOffset.y = 0;
		ft_sensor_data.ZoomFactor = 1;

		HRESULT result;

		if (!is_tracking_){
			result = face_tracker_->StartTracking(&ft_sensor_data, NULL, NULL, face_tracker_result_);

			if (SUCCEEDED(result) && SUCCEEDED(face_tracker_result_->GetStatus()))
			{
				is_tracking_ = true;
			}
			else
			{
				// No faces found
				is_tracking_ = false;
			}
		}
		else{
			result = face_tracker_->ContinueTracking(&ft_sensor_data, NULL, face_tracker_result_);

			if (FAILED(result) || FAILED(face_tracker_result_->GetStatus()))
			{
				// Lost the face
				is_tracking_ = false;
			}
		}
    */
	}


} // namespace kinect_face_tracker