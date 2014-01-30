#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/observer_list.h"
#include "base/scoped_handle.h"
#include "base/scoped_ptr.h"
#include "kinect_replay/kinect_player.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"

namespace kinect_wrapper {  

class KinectObserver;

class KinectSensorState {
 public:
  KinectSensorState();
  ~KinectSensorState();

  KinectSensor* GetSensor() {
    return sensor_.get();
  }
  void SetSensor(KinectSensor* sensor) {
    sensor_.reset(sensor);
  }

  void SetThread(HANDLE thread);
  void SetCloseEvent(HANDLE close_event);
  HANDLE GetCloseEvent();
  void SendCloseEvent();
  void WaitThreadCloseAndDelete();

  bool StartRecording(const std::string& filename);
  void StopRecording();
  bool LoadReplayFile(const std::string& filename);
  bool ReplayFrame();
  bool RecordFrame();

  void CreateBuffers();

  // Retrieves the last depth matrix.
  // @param mat the last depth matrix.
  // @returns true in case of success, false otherwise.
  bool QueryDepth(cv::Mat* mat) const;
  
  bool QuerySkeletonFrame(KinectSkeletonFrame* skeleton_frame) const;

  void InsertDepthFrame(const char* depth_frame, size_t depth_frame_size);
  void InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                        const size_t& num_pixels);
  void InsertSkeletonFrame(const KinectSkeletonFrame& skeleton_frame);

  void AddObserver(KinectObserver* obs);

 private:
  scoped_ptr<KinectSensor> sensor_;
  cv::Mat depth_buffer_;
  KinectSkeletonFrame skeleton_buffer_;
  
  base::ScopedHandle thread_;
  base::ScopedHandle close_event_;

  kinect_replay::KinectRecorder recorder_;
  kinect_replay::KinectPlayer player_;

  ObserverList<KinectObserver> observers_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensorState);
};

}  // namespace kinect_wrapper