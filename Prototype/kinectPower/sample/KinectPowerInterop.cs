using System.Collections;
using System.Runtime.InteropServices;

public class KinectPowerInterop
{

  [DllImport(@"kinect_lib.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Initialize(bool near_mode, bool with_sensor_thread);

  [DllImport(@"kinect_lib.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Shutdown();

  [DllImport(@"kinect_lib.dll", EntryPoint = "RecordSensor", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool RecordSensor(int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

  [DllImport(@"kinect_lib.dll", EntryPoint = "StartPlaySensor", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool StartPlaySensor(int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

  [DllImport(@"kinect_lib.dll", EntryPoint = "PlayNextFrame", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool PlayNextFrame(int sensor_index);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetDepthImage", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetDepthImage(byte[] pixels, uint pixels_size);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetColorImage", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetColorImage(byte[] pixels, uint pixels_size);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetHandsInteraction")]
  public static extern bool GetHandsInteraction(int skeleton_id, NuiHandPointerInfo[] hands);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetFaceRotation", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetFaceRotation(float[] face_rotation);

  // Kinect SDK constants.
  public const int NUI_SKELETON_POSITION_COUNT = 20;

  // Wrapper around the Kinect SDK functions.
  public static bool GetJointsPosition(int skeleton_id, float[] joint_positions, KinectHelpers.Skeleton.JointStatus[] joint_status)
  {
    System.Diagnostics.Debug.Assert(joint_positions.Length == NUI_SKELETON_POSITION_COUNT * 3);
    System.Diagnostics.Debug.Assert(joint_status.Length == NUI_SKELETON_POSITION_COUNT);
    return GetJointsPositionInternal(skeleton_id, joint_positions, joint_status);
  }

  public static bool GetJointsPositionDepth(int skeleton_id, int[] joint_positions)
  {
    System.Diagnostics.Debug.Assert(joint_positions.Length == NUI_SKELETON_POSITION_COUNT * 3);
    return GetJointsPositionDepthInternal(skeleton_id, joint_positions);
  }

  public static bool GetBonesOrientation(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations)
  {
    System.Diagnostics.Debug.Assert(bone_orientations.Length == NUI_SKELETON_POSITION_COUNT);
    return GetBonesOrientationInternal(skeleton_id, bone_orientations);
  }

  // Direct access to the Kinect SDK functions.
  [DllImport(@"kinect_lib.dll", EntryPoint = "GetJointsPosition", CallingConvention = CallingConvention.Cdecl)]
  private static extern bool GetJointsPositionInternal(int skeleton_id, float[] joint_positions, KinectHelpers.Skeleton.JointStatus[] joint_status);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetJointsPositionDepth", CallingConvention = CallingConvention.Cdecl)]
  private static extern bool GetJointsPositionDepthInternal(int skeleton_id, int[] joint_positions);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetBonesOrientation")]
  private static extern bool GetBonesOrientationInternal(int skeleton_id, NuiSkeletonBoneOrientation[] bone_orientations);

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
    Color,						        // RGB32 data
    ColorYUV,					        // YUY2 stream from camera h/w, but converted to RGB32 before user getting it.
    ColorRawYUV,				      // YUY2 stream from camera h/w.
    Depth						          // USHORT
  }

  public enum NuiImageResolution
  {
    resolutionInvalid = -1,
    resolution80x60 = 0,
    resolution320x240,
    resolution640x480,
    resolution1280x960       // for hires color only
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

  // Interaction.
  public struct NuiHandPointerInfo
  {
    public uint State;
    public NuiHandType HandType;
    public float X;
    public float Y;
    public float PressExtent;
    public float RawX;
    public float RawY;
    public float RawZ;
    public NuiHandEventType HandEventType;
  }

  public enum NuiHandEventType
  {
    None = 0,
    Grip,
    GripRelease
  }

  public enum NuiHandType
  {
    None = 0,
    Left,
    Right
  }

  public static uint NuiHandpointerStateNotTracked = 0x00;
  public static uint NuiHandPointerStateTracked = 0x01;
  public static uint NuiHandPointerStateActive = 0x02;
  public static uint NuiHandPointerStateInteractive = 0x04;
  public static uint NuiHandPointerStatePressed = 0x08;
  public static uint NuiHandPointerStatePrimaryForUser = 0x10;

  // Hand tracker.
  [DllImport(@"kinect_lib.dll", EntryPoint = "InitializeHandTracker", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool InitializeHandTracker();

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetHandsSkeletons", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetHandsSkeletons(float[] positions, float[] tracking_error);

  // Piano experimentations.
  [DllImport(@"kinect_lib.dll", EntryPoint = "GetPianoImage", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetPianoImage(byte[] pixels, uint pixels_size);

  [DllImport(@"kinect_lib.dll", EntryPoint = "GetPianoHands", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetPianoHands(int[] positions, byte[] known);
}
