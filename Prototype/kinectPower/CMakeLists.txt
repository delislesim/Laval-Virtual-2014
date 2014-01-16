project(kinect_power CXX)
cmake_minimum_required(VERSION 2.6.2)

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
	
# Linker search path
#link_directories(
#  $(KINECTSDK10_DIR)/lib/x86
#  ) 

####################
# Libraries
####################

add_library(kinect_power SHARED
  kinect_power/main.cpp
  )
	
target_link_libraries(kinect_power
  $ENV{KINECTSDK10_DIR}/lib/x86/Kinect10.lib)
	
####################
# Executables
####################

include_external_msproject(
    sample sample/sample.csproj
    TYPE FAE04EC0-301F-11D3-BF4B-00C04F79EFBC)
	
####################
# Unittests
####################

# none for now
