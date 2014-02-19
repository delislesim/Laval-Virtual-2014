#pragma once

#include "kinect_wrapper/kinect_include.h"
#include "kinect_wrapper/kinect_observer.h"

namespace kinect_face_tracker {

	class FaceTracker : public kinect_wrapper::KinectObserver {
	public:
		FaceTracker();
		~FaceTracker();

		virtual void ObserveDepth(const cv::Mat& depth_mat,
			const kinect_wrapper::KinectSensorData& sensor_data);

		IFTResult* FaceTrackerResult() { return face_tracker_result_; }

		// Le vecteurs contient l'angle : 
		//		pitch (mouvement vertical)
		//		yaw (mouvement horizontal)
		//		roll (incliner la tête sur le coté)
		cv::Vec3f FaceRotation();

		void initializeTracker();

		bool isTracking() { return is_tracking_; }

	private:
		IFTImage* color_image_;
		IFTImage* depth_image_;
		IFTFaceTracker* face_tracker_;
		IFTResult* face_tracker_result_;

		bool is_tracking_;
	};

} // namespace kinect_face_tracker