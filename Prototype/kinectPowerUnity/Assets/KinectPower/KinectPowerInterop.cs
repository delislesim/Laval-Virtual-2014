using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class KinectPowerInterop {

  [DllImport(@"kinect_power.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Initialize();

  [DllImport(@"kinect_power.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool Shutdown();

  [DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
  public static extern bool GetNiceDepthMap(byte[] buffer, uint buffer_size);

}
