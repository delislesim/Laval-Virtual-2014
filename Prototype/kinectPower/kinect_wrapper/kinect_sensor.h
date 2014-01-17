#pragma once

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "kinect_wrapper/kinect_depth_stream.h"
#include "kinect_wrapper/kinect_include.h"

namespace kinect_wrapper {

class KinectSensor {
 public:
  KinectSensor(INuiSensor* native_sensor);

  // The called doesn't own the depth stream.
  KinectDepthStream* GetDepthStream();

  // Interface to NUI.
  bool ImageStreamOpen(
      /* [in] */ NUI_IMAGE_TYPE eImageType,
      /* [in] */ NUI_IMAGE_RESOLUTION eResolution,
      /* [in] */ DWORD dwImageFrameFlags,
      /* [in] */ DWORD dwFrameLimit,
      /* [in] */ HANDLE hNextFrameEvent,
      /* [out] */ HANDLE *phStreamHandle);

  bool ImageStreamSetImageFrameFlags( 
      /* [in] */ HANDLE hStream,
      /* [in] */ DWORD dwImageFrameFlags);

  bool ImageStreamGetNextFrame( 
      /* [in] */ HANDLE hStream,
      /* [in] */ DWORD dwMillisecondsToWait,
      /* [retval][out] */ NUI_IMAGE_FRAME *pImageFrame);

  bool ImageStreamReleaseFrame( 
      /* [in] */ HANDLE hStream,
      /* [in] */ NUI_IMAGE_FRAME *pImageFrame);

 private:
  friend class KinectWrapper;
  ~KinectSensor();

  INuiSensor* native_sensor_;

  scoped_ptr<KinectDepthStream> depth_stream_;

  DISALLOW_COPY_AND_ASSIGN(KinectSensor);
};

}  // namespace kinect_wrapper