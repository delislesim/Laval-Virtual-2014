using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class KinectPowerInterop
{

	[DllImport(@"kinect_power.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool Initialize ();

	[DllImport(@"kinect_power.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool Shutdown ();

	[DllImport(@"kinect_power.dll", EntryPoint = "RecordSensor", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool RecordSensor (int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport(@"kinect_power.dll", EntryPoint = "StartPlaySensor", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool StartPlaySensor (int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

	[DllImport(@"kinect_power.dll", EntryPoint = "PlayNextFrame", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool PlayNextFrame (int sensor_index);

	[DllImport(@"kinect_power.dll", EntryPoint = "GetJointsPosition", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetJointsPosition (int skeleton_id, float[] joint_positions);

	[DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetNiceDepthMap (byte[] pixels, uint pixels_size);

	[DllImport(@"kinect_power.dll", EntryPoint = "GetPianoInfo", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetPianoInfo (byte[] notes, uint notes_size, byte[] pixels, uint pixels_size);

	// Kinect SDK constants.
	public const int NUI_SKELETON_POSITION_COUNT = 20;

	// Wrapper around the Kinect SDK functions.
	public static bool GetBonesOrientation(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations) {
		System.Diagnostics.Debug.Assert (bone_orientations.Length == NUI_SKELETON_POSITION_COUNT);
		return GetBonesOrientationInternal (skeleton_id, bone_orientations);
	}

	// Direct access to the Kinect SDK functions.
	[DllImport(@"kinect_power.dll", EntryPoint = "GetBonesOrientation")]
	private static extern bool GetBonesOrientationInternal(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations);

	// Structures and enumerations for the arguments of the Kinect SDK functions.
	public enum NuiSkeletonPositionIndex : int
	{
		HipCenter = 0,
		Spine,
		ShoulderCenter,
		Head,
		ShoulderLeft,
		ElbowLeft,
		WristLeft,
		HandLeft,
		ShoulderRight,
		ElbowRight,
		WristRight,
		HandRight,
		HipLeft,
		KneeLeft,
		AnkleLeft,
		FootLeft,
		HipRight,
		KneeRight,
		AnkleRight,
		FootRight,
		Count
	}

	public struct NuiSkeletonBoneRotation
	{
		public Matrix4x4 rotationMatrix;
		public Quaternion rotationQuaternion;
	}

	public struct NuiSkeletonBoneOrientation
	{
		public NuiSkeletonPositionIndex endJoint;
		public NuiSkeletonPositionIndex startJoint;
		public NuiSkeletonBoneRotation hierarchicalRotation;
		public NuiSkeletonBoneRotation absoluteRotation;
	}
}
