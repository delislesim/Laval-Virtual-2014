project(kinect_power CXX)
cmake_minimum_required(VERSION 2.6.2)

set (OpenCV_DIR $ENV{OPENCV_DIR})
FIND_PACKAGE(OpenCV REQUIRED)
#FIND_PACKAGE(OpenCL REQUIRED)

set(USE_INTEL_CAMERA 1)

set(GMOCK_ROOT "../gmock/")

set(CMAKE_SUPPRESS_REGENERATION TRUE)

# Activate warning level 4.
# Disable 4481 warning C4481: nonstandard extension used: override specifier 'override'
if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
  string(REGEX REPLACE "/W[0-4]" "/W4 /wd4481" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
else()
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
endif()

if(USE_INTEL_CAMERA)
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /D USE_INTEL_CAMERA")
  set(INTEL_HAND_TRACKER_DEPENDENCY
        hskl/lib/x64/hskl
        $ENV{PCSDK_DIR}/lib/x64/libpxc.lib
  )
else()
  set(INTEL_HAND_TRACKER_DEPENDENCY "")  
endif()

# Force a unicode project
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /D _UNICODE /D UNICODE")

# Includes search path
include_directories(
  .
  hskl/include
  "C:/Program Files/Microsoft SDKs/Kinect/v2.0-DevPreview1311/inc"
  $ENV{PCSDK_DIR}/include
  ${GMOCK_ROOT}/include
  ${GMOCK_ROOT}
  ${GMOCK_ROOT}/gtest/include
  ${GMOCK_ROOT}/gtest
  )

####################
# Libraries
####################

add_library(base
  base/base.h
  base/logging.h
  base/observer_list.h
  base/scoped_ptr.h
  base/scoped_handle.cc
  base/scoped_handle.h
  base/timer.cc
  base/timer.h
  )
  
add_library(creative
  creative/creative_wrapper.cc
  creative/creative_wrapper.h
  creative/hand_info.h
  creative/joint_info.h
  )
  
add_library(kinect_wrapper
  kinect_wrapper/constants.cc
  kinect_wrapper/constants.h
  kinect_wrapper/kinect_include.h
  kinect_wrapper/kinect_sensor.cc
  kinect_wrapper/kinect_sensor.h
  kinect_wrapper/kinect_skeleton.cc
  kinect_wrapper/kinect_skeleton.h
  kinect_wrapper/utility.h
  )
  
target_link_libraries(kinect_wrapper
  base
  )

add_library(kinect_lib SHARED
  kinect_lib/lib.cc
  kinect_lib/lib.h
  )
	
target_link_libraries(kinect_lib
  base
  creative
  kinect_wrapper
  "C:/Program Files/Microsoft SDKs/Kinect/v2.0-DevPreview1311/lib/x64/Kinect20.lib"
  ${INTEL_HAND_TRACKER_DEPENDENCY}
  )
	
####################
# Executables
####################

#include_external_msproject(
#    sample sample2010/sample/sample.csproj
#    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
add_executable(test_cpp
  test_cpp/main.cc
  )

target_link_libraries(test_cpp
  kinect_lib
  )
