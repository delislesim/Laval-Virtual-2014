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

	[DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetNiceDepthMap (byte[] pixels, uint pixels_size);

	[DllImport(@"kinect_power.dll", EntryPoint = "GetPianoInfo", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetPianoInfo (byte[] notes, uint notes_size, byte[] pixels, uint pixels_size);

	// Kinect SDK constants.
	public const int NUI_SKELETON_POSITION_COUNT = 20;

	// Wrapper around the Kinect SDK functions.
	public static bool GetJointsPosition(int skeleton_id, float[] joint_positions, KinectHelpers.Skeleton.JointStatus[] joint_status) {
		System.Diagnostics.Debug.Assert (joint_positions.Length == NUI_SKELETON_POSITION_COUNT);
		System.Diagnostics.Debug.Assert (joint_status.Length == NUI_SKELETON_POSITION_COUNT);
		return GetJointsPositionInternal (skeleton_id, joint_positions, joint_status);
	}

	public static bool GetBonesOrientation(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations) {
		System.Diagnostics.Debug.Assert (bone_orientations.Length == NUI_SKELETON_POSITION_COUNT);
		return GetBonesOrientationInternal (skeleton_id, bone_orientations);
	}

	// Direct access to the Kinect SDK functions.
	[DllImport(@"kinect_power.dll", EntryPoint = "GetJointsPosition", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetJointsPositionInternal (int skeleton_id, float[] joint_positions, KinectHelpers.Skeleton.JointStatus[] joint_status);

	[DllImport(@"kinect_power.dll", EntryPoint = "GetBonesOrientation")]
	private static extern bool GetBonesOrientationInternal(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations);

	// Enumerations specific to Kinect Power.


	// Structures and enumerations for the arguments of the Kinect SDK functions.
	public enum NuiErrorCodes : uint
	{
		FrameNoData = 0x83010001,
		StreamNotEnabled = 0x83010002,
		ImageStreamInUse = 0x83010003,
		FrameLimitExceeded = 0x83010004,
		FeatureNotInitialized = 0x83010005,
		DeviceNotGenuine = 0x83010006,
		InsufficientBandwidth = 0x83010007,
		DeviceNotSupported = 0x83010008,
		DeviceInUse = 0x83010009,
		
		DatabaseNotFound = 0x8301000D,
		DatabaseVersionMismatch = 0x8301000E,
		HardwareFeatureUnavailable = 0x8301000F,
		
		DeviceNotConnected = 0x83010014,
		DeviceNotReady = 0x83010015,
		SkeletalEngineBusy = 0x830100AA,
		DeviceNotPowered = 0x8301027F,
	}

	public struct NuiSkeletonBoneRotation
	{
		public Matrix4x4 rotationMatrix;
		public Quaternion rotationQuaternion;
	}

	public struct NuiSkeletonBoneOrientation
	{
		public KinectHelpers.Skeleton.Joint endJoint;
		public KinectHelpers.Skeleton.Joint startJoint;
		public NuiSkeletonBoneRotation hierarchicalRotation;
		public NuiSkeletonBoneRotation absoluteRotation;
	}

	public enum NuiSkeletonPositionTrackingState
	{
		NotTracked = 0,
		Inferred,
		Tracked
	}
	
	public enum NuiSkeletonTrackingState
	{
		NotTracked = 0,
		PositionOnly,
		SkeletonTracked
	}
	
	public enum NuiImageType
	{
		DepthAndPlayerIndex = 0,	// USHORT
		Color,						// RGB32 data
		ColorYUV,					// YUY2 stream from camera h/w, but converted to RGB32 before user getting it.
		ColorRawYUV,				// YUY2 stream from camera h/w.
		Depth						// USHORT
	}
	
	public enum NuiImageResolution
	{
		resolutionInvalid = -1,
		resolution80x60 = 0,
		resolution320x240,
		resolution640x480,
		resolution1280x960                        // for hires color only
	}
	
	public enum NuiImageStreamFlags
	{
		None = 0x00000000,
		SupressNoFrameData = 0x0001000,
		EnableNearMode = 0x00020000,
		TooFarIsNonZero = 0x0004000
	}

	public enum FrameEdges
	{
		None = 0,
		Right = 1,
		Left = 2,
		Top = 4,
		Bottom = 8
	}
}
