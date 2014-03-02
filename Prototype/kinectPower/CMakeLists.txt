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
        hskl/lib/Win32/hskl
        $ENV{PCSDK_DIR}/lib/Win32/libpxc.lib
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
  algos/disjoint_set.h
  algos/kalman_filter.cc
  algos/kalman_filter.h
  algos/maths.cc
  algos/maths.h
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
  
add_library(creative
  creative/creative_wrapper.cc
  creative/creative_wrapper.h
  creative/hand_info.h
  creative/joint_info.h
  )
  
add_library(bitmap_graph
  bitmap_graph/bitmap_graph.cc
  bitmap_graph/bitmap_graph.h
  bitmap_graph/bitmap_graph_builder.cc
  bitmap_graph/bitmap_graph_builder.h
  bitmap_graph/bitmap_run.cc
  bitmap_graph/bitmap_run.h
  )

add_library(finger_finder
  finger_finder/average_depth.cc
  finger_finder/average_depth.h
  finger_finder/bitmap_graph.cc
  finger_finder/bitmap_graph.h
  finger_finder/bitmap_graph_to_contours_list.cc
  finger_finder/bitmap_graph_to_contours_list.h
  finger_finder/canny_contour.cc
  finger_finder/canny_contour.h
  finger_finder/continuity_filter.cc
  finger_finder/continuity_filter.h
  finger_finder/contour_angle.cc
  finger_finder/contour_angle.h
  finger_finder/depth_translator.cc
  finger_finder/depth_translator.h
  finger_finder/finger_finder.cc
  finger_finder/finger_finder.h
  finger_finder/finger_info.h
  )
  
add_library(finger_finder_thinning
  finger_finder_thinning/canny_contour.cc
  finger_finder_thinning/canny_contour.h
  finger_finder_thinning/finger_finder.cc
  finger_finder_thinning/finger_finder.h
  finger_finder_thinning/segmenter.cc
  finger_finder_thinning/segmenter.h
  )
  
add_library(image
  image/contour_center.cc
  image/contour_center.h
  image/image_constants.cc
  image/image_constants.h
  image/image_utility.cc
  image/image_utility.h
  )
  
add_library(kinect_gesture_recognition
  kinect_gesture_recognition/gesture_controller.h
  kinect_gesture_recognition/gesture_controller.cc
  kinect_gesture_recognition/gesture.h
  kinect_gesture_recognition/gesture.cc
  kinect_gesture_recognition/gesture_piano.h
  kinect_gesture_recognition/gesture_piano.cc
  kinect_gesture_recognition/gesture_guitar.cc
  kinect_gesture_recognition/gesture_guitar.h
  kinect_gesture_recognition/gesture_drum.h
  kinect_gesture_recognition/gesture_drum.cc
  )

add_library(kinect_interaction
  kinect_interaction/interaction_client_base.h
  kinect_interaction/interaction_client_menu.cc
  kinect_interaction/interaction_client_menu.h
  kinect_interaction/interaction_frame.cc
  kinect_interaction/interaction_frame.h
  )
  
add_library(kinect_face_tracker
  kinect_face_tracker/face_tracker.cc
  kinect_face_tracker/face_tracker.h
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
  creative
  bitmap_graph
  finger_finder
  finger_finder_thinning
  image
  kinect_gesture_recognition
  kinect_interaction
  kinect_face_tracker
  kinect_replay
  kinect_wrapper
  piano
  ${OpenCV_LIBS}
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib
  $ENV{KINECT_TOOLKIT_DIR}/Lib/x86/KinectInteraction180_32.lib
  $ENV{KINECT_TOOLKIT_DIR}/Lib/x86/FaceTrackLib.lib
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

####################
# Unittests
####################

add_executable(unittests
  algos/disjoint_set_unittests.cc
  algos/maths_unittests.cc
  algos/sliding_window_unittests.cc
  bitmap_graph/bitmap_graph_unittest.cc
  bitmap_graph/bitmap_graph_builder_unittest.cc
  bitmap_graph/bitmap_run_unittest.cc
  ${GMOCK_ROOT}/gtest/src/gtest-all.cc
  ${GMOCK_ROOT}/src/gmock-all.cc
  ${GMOCK_ROOT}/src/gmock_main.cc
  )

target_link_libraries(unittests
  kinect_lib
  )