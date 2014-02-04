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
  hskl/include
  $ENV{KINECTSDK10_DIR}/inc
  $ENV{KINECT_TOOLKIT_DIR}/inc
  $ENV{PCSDK_DIR}/include
  ${GMOCK_ROOT}/include
  ${GMOCK_ROOT}
  ${GMOCK_ROOT}/gtest/include
  ${GMOCK_ROOT}/gtest
  )

####################
# Libraries
####################

add_library(algos
  algos/contour_walk.cc
  algos/contour_walk.h
  algos/kalman_filter.cc
  algos/kalman_filter.h
  algos/sliding_window.cc
  algos/sliding_window.h
  algos/stable_matching.cc
  algos/stable_matching.h
  )

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
  
add_library(finger_finder
  finger_finder/contour_curve.cc
  finger_finder/contour_curve.h
  finger_finder/contour_curve_filter.cc
  finger_finder/contour_curve_filter.h
  finger_finder/find_fingers_in_contour.cc
  finger_finder/find_fingers_in_contour.h
  finger_finder/finger_finder.cc
  finger_finder/finger_finder.h
  finger_finder/hand_parameters.cc
  finger_finder/hand_parameters.h
  finger_finder/match_contours_to_previous_hands.cc
  finger_finder/match_contours_to_previous_hands.h
  finger_finder/segmenter.cc
  finger_finder/segmenter.h
  )
  
add_library(hand_extractor
  hand_extractor/depth_difference_finder.cc
  hand_extractor/depth_difference_finder.h
  hand_extractor/finger_finder_convex.cc
  hand_extractor/finger_finder_convex.h
  hand_extractor/finger_finder_curve.cc
  hand_extractor/finger_finder_curve.h
  hand_extractor/finger_finder_mountain.cc
  hand_extractor/finger_finder_mountain.h
  hand_extractor/hand_2d_parameters.cc
  hand_extractor/hand_2d_parameters.h
  hand_extractor/hand_extractor.cc
  hand_extractor/hand_extractor.h
  hand_extractor/segmenter.cc
  hand_extractor/segmenter.h
  )
  
add_library(image
  image/contour_center.cc
  image/contour_center.h
  image/image_constants.cc
  image/image_constants.h
  image/image_utility.cc
  image/image_utility.h
  )
  
add_library(intel_hand_tracker
  intel_hand_tracker/intel_hand_tracker.cc
  intel_hand_tracker/intel_hand_tracker.h
  intel_hand_tracker/hskl.h
  intel_hand_tracker/hsklu.h
  )

add_library(kinect_interaction
  kinect_interaction/interaction_client_base.h
  kinect_interaction/interaction_client_menu.cc
  kinect_interaction/interaction_client_menu.h
  kinect_interaction/interaction_frame.cc
  kinect_interaction/interaction_frame.h
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
  kinect_wrapper/kinect_switch.h
  kinect_wrapper/kinect_wrapper.cc
  kinect_wrapper/kinect_wrapper.h
  kinect_wrapper/utility.cc
  kinect_wrapper/utility.h
  )
  
target_link_libraries(kinect_wrapper
  base
  )

add_library(maths
  maths/curve.cc
  maths/curve.h
  maths/maths.cc
  maths/maths.h
  maths/quaternion.cc
  maths/quaternion.h
  maths/smooth.cc
  maths/smooth.h
  )
  
add_library(piano
  piano/piano.cc
  piano/piano.h
  )

add_library(kinect_lib SHARED
  kinect_lib/lib.cc
  kinect_lib/lib.h
  )
	
target_link_libraries(kinect_lib
  algos
  base
  finger_finder
  image
  intel_hand_tracker
  kinect_interaction
  kinect_replay
  kinect_wrapper
  maths
  piano
  hskl/lib/Win32/hskl
  ${OpenCV_LIBS}
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  $ENV{KINECT_TOOLKIT_DIR}/Lib/x86/KinectInteraction180_32.lib
  $ENV{PCSDK_DIR}/lib/Win32/libpxc.lib
  )
	
####################
# Executables
####################

include_external_msproject(
    sample sample/sample.csproj
    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
add_executable(test_cpp
  test_cpp/main.cc
  )

target_link_libraries(test_cpp
  kinect_lib
  )
	
####################
# Unittests
####################

add_executable(unittests
  algos/sliding_window_unittests.cc
  maths/maths_unittests.cc
  maths/smooth_unittests.cc
  ${GMOCK_ROOT}/gtest/src/gtest-all.cc
  ${GMOCK_ROOT}/src/gmock-all.cc
  ${GMOCK_ROOT}/src/gmock_main.cc
  )

target_link_libraries(unittests
  kinect_lib
  )