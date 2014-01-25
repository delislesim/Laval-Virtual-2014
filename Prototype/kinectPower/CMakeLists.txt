project(kinect_power CXX)
cmake_minimum_required(VERSION 2.6.2)

set (OpenCV_DIR $ENV{OPENCV_DIR})
FIND_PACKAGE(OpenCV REQUIRED)

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
  
add_library(hand_extractor
  hand_extractor/hand_extractor.cc
  hand_extractor/hand_extractor.h
  )
  
add_library(hand_particle_filter
  hand_particle_filter/hand_depth_renderer.cc
  hand_particle_filter/hand_depth_renderer.h
  hand_particle_filter/hand_description.cc
  hand_particle_filter/hand_description.h
  hand_particle_filter/hand_hypothesis.cc
  hand_particle_filter/hand_hypothesis.h
  hand_particle_filter/hand_skeleton.cc
  hand_particle_filter/hand_skeleton.h
  hand_particle_filter/hand_tracker.cc
  hand_particle_filter/hand_tracker.h
  )
  
add_library(image
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
  kinect_wrapper/kinect_buffer.cc
  kinect_wrapper/kinect_buffer.h
  kinect_wrapper/kinect_include.h
  kinect_wrapper/kinect_observer.h
  kinect_wrapper/kinect_sensor.cc
  kinect_wrapper/kinect_sensor.h
  kinect_wrapper/kinect_sensor_state.cc
  kinect_wrapper/kinect_sensor_state.h
  kinect_wrapper/kinect_skeleton.cc
  kinect_wrapper/kinect_skeleton.h
  kinect_wrapper/kinect_skeleton_frame.cc
  kinect_wrapper/kinect_skeleton_frame.h
  kinect_wrapper/kinect_switch.h
  kinect_wrapper/kinect_wrapper.cc
  kinect_wrapper/kinect_wrapper.h
  kinect_wrapper/utility.cc
  kinect_wrapper/utility.h
  )

add_library(maths
  maths/maths.cc
  maths/maths.h
  maths/quaternion.cc
  maths/quaternion.h
  )
  
add_library(piano
  piano/piano.cc
  piano/piano.h
  )
  
add_library(kinect_power SHARED
  kinect_power/lib.cc
  kinect_power/lib.h
  )
	
target_link_libraries(kinect_power
  base
  hand_extractor
  hand_particle_filter
  image
  kinect_replay
  kinect_wrapper
  maths
  piano
  ${OpenCV_LIBS}
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  )
	
####################
# Executables
####################

#include_external_msproject(
#    sample sample/sample.csproj
#    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
####################
# Unittests
####################

# coming soon...
