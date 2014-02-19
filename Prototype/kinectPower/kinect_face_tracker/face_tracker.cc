#include "face_tracker.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor_data.h"
#include "kinect_wrapper/kinect_wrapper.h"
#include "kinect_wrapper/kinect_skeleton.h"

namespace kinect_face_tracker {
	FaceTracker::FaceTracker(){
		is_tracking_ = false;
	}

	FaceTracker::~FaceTracker(){
		color_image_->Release();
		depth_image_->Release();
		face_tracker_result_->Release();
		face_tracker_->Release();
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

	void FaceTracker::initializeTracker(){
		face_tracker_ = FTCreateFaceTracker();
		color_image_ = FTCreateImage();
		depth_image_ = FTCreateImage();
		FT_CAMERA_CONFIG video_config;
		video_config.FocalLength = NUI_CAMERA_COLOR_NOMINAL_FOCAL_LENGTH_IN_PIXELS;
		video_config.Width = kinect_wrapper::kKinectColorWidth;
		video_config.Height = kinect_wrapper::kKinectColorHeight;
		FT_CAMERA_CONFIG depth_config;
		depth_config.FocalLength = NUI_CAMERA_DEPTH_NOMINAL_FOCAL_LENGTH_IN_PIXELS * 2;
		depth_config.Width = kinect_wrapper::kKinectDepthWidth;
		depth_config.Height = kinect_wrapper::kKinectDepthHeight;
		face_tracker_->Initialize(&video_config, &depth_config, NULL, NULL);
		face_tracker_->CreateFTResult(&face_tracker_result_);
	}

	void FaceTracker::ObserveDepth(const cv::Mat& depth_mat, const kinect_wrapper::KinectSensorData& sensor_data){
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
			FT_VECTOR3D* head_point_pointer = NULL;
			FT_VECTOR3D head_point;
			kinect_wrapper::KinectSkeleton skeleton;
			if (sensor_data.GetSkeletonFrame()->GetTrackedSkeleton(0, &skeleton)){
				cv::Vec3f position;
				kinect_wrapper::KinectSkeleton::JointStatus status;
				skeleton.GetJointPosition(kinect_wrapper::KinectSkeleton::Head, &position, &status);
				head_point.x = position[0];
				head_point.y = position[1];
				head_point.z = position[2];
				head_point_pointer = &head_point;
			}

			result = face_tracker_->StartTracking(&ft_sensor_data, NULL, head_point_pointer, face_tracker_result_);

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
	}


} // namespace kinect_face_tracker