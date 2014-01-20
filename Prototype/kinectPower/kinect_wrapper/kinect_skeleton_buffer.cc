#include "kinect_wrapper/kinect_skeleton_buffer.h"

#include "base/logging.h"

namespace kinect_wrapper {

KinectSkeletonBuffer::KinectSkeletonBuffer()
    : current_buffer_index_(0) {
}

KinectSkeletonBuffer::~KinectSkeletonBuffer() {
}

void KinectSkeletonBuffer::GetCurrentSkeletonFrame(
    KinectSkeletonFrame* frame) const {
  DCHECK(frame);
  *frame = frames_[current_buffer_index_];
}

// Insert the next skeleton in the buffer.
void KinectSkeletonBuffer::SetNextSkeleton(
    const KinectSkeletonFrame& next_frame,
    DWORD track_id_1, DWORD track_id_2) {
  size_t next_buffer_index = (current_buffer_index_ + 1) % kNumBuffers;
  frames_[next_buffer_index] = next_frame;
  frames_[next_buffer_index].SetTrackedSkeletons(track_id_1, track_id_2);

  current_buffer_index_ = next_buffer_index;
}

}  // namespace kinect_wrapper