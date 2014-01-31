project(kinect_power CXX)
cmake_minimum_required(VERSION 2.6.2)

set (OpenCV_DIR $ENV{OPENCV_DIR})
FIND_PACKAGE(OpenCV REQUIRED)
#FIND_PACKAGE(OpenCL REQUIRED)

set(GMOCK_ROOT "../gmock/")

set(CMAKE_SUPPRESS_REGENERATION TRUE)

# Activate warning level 4.
# Disable 4481 warning C4481: nonstandard extension used: override specifier 'override'
if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
  string(REGEX REPLACE "/W[0-4]" "/W4 /wd4481" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
else()
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
endif()

# Force a unicode project
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /D _UNICODE /D UNICODE")

# Includes search path
include_directories(
  .
  $ENV{KINECTSDK10_DIR}/inc
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
  
add_library(image
  image/contour_center.cc
  image/contour_center.h
  image/image_constants.cc
  image/image_constants.h
  image/image_utility.cc
  image/image_utility.h
  )

add_library(kinect_replay
  kinect_replay/kinect_recorder.cc
  kinect_replay/kinect_recorder.h
  kinect_replay/kinect_player.cc
  kinect_replay/kinect_player.h
  )
  
add_library(kinect_wrapper
  kinect_wrapper/constants.cc
  kinect_wrapper/constants.h
  kinect_wrapper/kinect_include.h
  kinect_wrapper/kinect_observer.h
  kinect_wrapper/kinect_sensor.cc
  kinect_wrapper/kinect_sensor.h
  kinect_wrapper/kinect_sensor_data.cc
  kinect_wrapper/kinect_sensor_data.h
  kinect_wrapper/kinect_sensor_state.cc
  kinect_wrapper/kinect_sensor_state.h
  kinect_wrapper/kinect_skeleton.cc
  kinect_wrapper/kinect_skeleton.h
  kinect_wrapper/kinect_skeleton_frame.cc
  kinect_wrapper/kinect_skeleton_frame.h
  kinect_wrapper/kinect_wrapper.cc
  kinect_wrapper/kinect_wrapper.h
  kinect_wrapper/utility.cc
  kinect_wrapper/utility.h
  )
  
target_link_libraries(kinect_wrapper
  base
  )

add_library(maths
  maths/maths.cc
  maths/maths.h
  maths/quaternion.cc
  maths/quaternion.h
  )

add_library(kinect_lib SHARED
  kinect_lib/lib.cc
  kinect_lib/lib.h
  )
	
target_link_libraries(kinect_lib
  base
  image
  kinect_replay
  kinect_wrapper
  maths
  ${OpenCV_LIBS}
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  )
	
####################
# Executables
####################

include_external_msproject(
    sample2013 sample2013/sample2013.csproj
    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
####################
# Unittests
####################

add_executable(unittests
  maths/maths_unittests.cc
  ${GMOCK_ROOT}/gtest/src/gtest-all.cc
  ${GMOCK_ROOT}/src/gmock-all.cc
  ${GMOCK_ROOT}/src/gmock_main.cc
  )

target_link_libraries(unittests
  base
  image
  kinect_replay
  kinect_wrapper
  maths
  ${OpenCV_LIBS}
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  )