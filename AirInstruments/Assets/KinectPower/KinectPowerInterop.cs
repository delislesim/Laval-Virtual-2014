using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class KinectPowerInterop
{
	// Constante indiquant si on est dans la version Kinect DEUX.
	public static bool IsKinect2 = false;

	// Constante indiquant si on est dans la version Kinect UN.
	public static bool IsKinect1 = !IsKinect2;

	[DllImport(@"kinect_lib.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool Initialize();

	[DllImport(@"kinect_lib.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool Shutdown();

	[DllImport(@"kinect_lib.dll", EntryPoint = "AvoidCurrentSkeleton", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool AvoidCurrentSkeleton();

	// Wrapper around the Kinect SDK functions.
	public static bool GetJoints(float[] positions, float[] orientations, KinectHelpers.Skeleton.JointStatus[] joint_status, int[] is_new)
	{
		try {
			System.Diagnostics.Debug.Assert(positions.Length == (int)KinectHelpers.Skeleton.Joint.Count * 3);
			System.Diagnostics.Debug.Assert(orientations.Length == (int)KinectHelpers.Skeleton.Joint.Count * 4);
			System.Diagnostics.Debug.Assert(joint_status.Length == (int)KinectHelpers.Skeleton.Joint.Count);
			System.Diagnostics.Debug.Assert(is_new.Length == 1);
			return GetJointsInternal(positions, orientations, joint_status, is_new);
		} catch (Exception) {
		}
		return false;
	}

	public static bool GetJointsPositionDepth(int[] joint_positions) {
		try {
			System.Diagnostics.Debug.Assert(joint_positions.Length == (int)KinectHelpers.Skeleton.Joint.Count * 2);
			return GetJointsPositionDepthInternal(joint_positions);
		} catch (Exception) {
		}
		return false;
	}

	// Direct access to the Kinect SDK functions.
	[DllImport(@"kinect_lib.dll", EntryPoint = "GetJoints", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool GetJointsInternal(float[] positions, float[] orientations, KinectHelpers.Skeleton.JointStatus[] joint_status, int[] is_new);

	[DllImport(@"kinect_lib.dll", EntryPoint = "GetJointsPositionDepth", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool GetJointsPositionDepthInternal(int[] joint_positions);

	public enum NuiSkeletonPositionTrackingState
	{
	NotTracked = 0,
	Inferred,
	Tracked
	}

	public struct HandJointInfo {
	public float x;
	public float y;
	public float z;
	public float error;
	}

	public enum HandJointIndex {
	NO_JOINT = -1,
	FOREARM = 0,
	PALM,
	PINKY_BASE,
	PINKY_MID,
	PINKY_TIP,
	RING_BASE,
	RING_MID,
	RING_TIP,
	MIDDLE_BASE,
	MIDDLE_MID,
	MIDDLE_TIP,
	INDEX_BASE,
	INDEX_MID,
	INDEX_TIP,
	THUMB_BASE,
	THUMB_MID,
	THUMB_TIP,
	NUM_JOINTS
	}

	// Hand tracker.
	[DllImport(@"kinect_lib.dll", EntryPoint = "InitializeHandTracker", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool InitializeHandTracker();

	[DllImport(@"kinect_lib.dll", EntryPoint = "GetHandsSkeletons", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetHandsSkeletons(HandJointInfo[] hand_joints);

	[DllImport(@"kinect_lib.dll", EntryPoint = "SetHandMeasurements", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool SetHandMeasurements(float width, float height);

}
