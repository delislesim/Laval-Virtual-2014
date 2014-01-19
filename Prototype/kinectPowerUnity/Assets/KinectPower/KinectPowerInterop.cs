using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class KinectPowerInterop {

  [DllImport(@"kinect_power.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Initialize();

  [DllImport(@"kinect_power.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Shutdown();

  [DllImport(@"kinect_power.dll", EntryPoint = "RecordSensor", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool RecordSensor(int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

  [DllImport(@"kinect_power.dll", EntryPoint = "StartPlaySensor", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool StartPlaySensor(int sensor_index, [MarshalAs(UnmanagedType.LPStr)] string filename);

  [DllImport(@"kinect_power.dll", EntryPoint = "PlayNextFrame", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool PlayNextFrame(int sensor_index);

  [DllImport(@"kinect_power.dll", EntryPoint = "GetJointsPosition", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetJointsPosition(int skeleton_id, float[] joint_positions);

  [DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetNiceDepthMap(byte[] pixels, uint pixels_size);
	
  [DllImport(@"kinect_power.dll", EntryPoint = "GetPianoInfo", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetPianoInfo(byte[] notes, uint notes_size, byte[] pixels, uint pixels_size);

}
