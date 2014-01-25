project(kinect_power CXX)
cmake_minimum_required(VERSION 2.6.2)

set (OpenCV_DIR $ENV{OPENCV_DIR})
FIND_PACKAGE(OpenCV REQUIRED)
#FIND_PACKAGE(OpenCL REQUIRED)

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
 target_link_libraries(kinect_wrapper
  base
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  )

add_library(kinect_replay
  kinect_replay/kinect_recorder.cc
  kinect_replay/kinect_recorder.h
  kinect_replay/kinect_player.cc
  kinect_replay/kinect_player.h
  )
 target_link_libraries(kinect_replay
  base
  kinect_wrapper
  )
  
add_library(hands_tracker
  hands_tracker/hands_tracker.cc
  hands_tracker/hands_tracker.h
  )
 target_link_libraries(hands_tracker
  base
  )
  
 add_library(piano
  piano/piano.cc
  piano/piano.h
  )
 target_link_libraries(hands_tracker
  base
  )
  
add_library(kinect_power SHARED
  kinect_power/lib.cc
  kinect_power/lib.h
  )
####################################################
# À tester, car je ne suis pas sur que ça va marcher
####################################################
# FIND_PATH(OPENCL_INCLUDE_DIR
	# NAMES
		# CL\cl.h
	# PATHS
		# $ENV{CUDA_PATH}\include\)

# SET(OPENCL_LIB_SEARCH_PATH
		# ${OPENCL_LIB_SEARCH_PATH}
		# $ENV{CUDA_PATH}\lib\Win32)

# FIND_LIBRARY(
    # OPENCL_LIBRARY
    # NAMES OpenCL
    # PATHS ${OPENCL_LIB_SEARCH_PATH})

# include_directories( ${OPENCL_INCLUDE_DIR} )
#include_directories( $ENV{CUDA_PATH}\include\CL\cl.h)

#target_link_libraries(${OPENCL_LIBRARY})
#####################################################
	
target_link_libraries(kinect_power
  hands_tracker
  kinect_replay
  kinect_wrapper
  piano
  ${OpenCV_LIBS}
  #${OPENCL_LIBRARY}
  #$ENV{CUDA_PATH}\lib\Win32\OpenCL.lib
  )
	
####################
# Executables
####################

add_executable (test_opencv
  test_opencv/main.cc
)

target_link_libraries(test_opencv
  ${OpenCV_LIBS}
  )

#include_external_msproject(
#    sample sample/sample.csproj
#    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
####################
# Unittests
####################

# none for now