#pragma once

#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/scoped_handle.h"
#include "base/scoped_ptr.h"
#include "kinect_replay/kinect_player.h"
#include "kinect_replay/kinect_recorder.h"
#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_buffer.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_skeleton_frame.h"
#include "kinect_wrapper/kinect_switch.h"

namespace kinect_wrapper {  

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
  void ReleaseBuffers();

  bool QueryDepth(cv::Mat* mat) const;
  bool QuerySkeletonFrame(KinectSkeletonFrame* skeleton_frame) const;

  void InsertDepthFrame(const char* depth_frame, size_t depth_frame_size);
  void InsertDepthFrame(const NUI_DEPTH_IMAGE_PIXEL* start,
                        const NUI_DEPTH_IMAGE_PIXEL* end);
  void InsertSkeletonFrame(const KinectSkeletonFrame& skeleton_frame);

 private:
  scoped_ptr<KinectSensor> sensor_;
  scoped_ptr<KinectBuffer> depth_buffer_;
  scoped_ptr<KinectSwitch<KinectSkeletonFrame> > skeleton_buffer_;
  
  base::ScopedHandle thread_;
  base::ScopedHandle close_event_;

  kinect_replay::KinectRecorder recorder;
  kinect_replay::KinectPlayer player;

  DISALLOW_COPY_AND_ASSIGN(KinectSensorState);
};

}  // namespace kinect_wrapper