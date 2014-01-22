using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace sample
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      //Initialize();
      //RecordSensor(0, "test.txt");
      StartPlaySensor(0, "replay_piano.boubou");

      aTimer = new System.Timers.Timer(150);
      aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);

      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      PlayNextFrame(0);

      if (!GetPianoInfo(notes, (uint)notes.Length, buffer, (uint)buffer.Length))
        return;

      int stride = kImageWidth * kPixelSize;

      GetJointsPosition(0, joint_positions);

      try
      {
          Application.Current.Dispatcher.Invoke(new Action(() =>
          {
              image1.Source = BitmapSource.Create(kImageWidth, kImageHeight, 96, 96,
                                                  PixelFormats.Bgr32, null, buffer, stride);
          }));
      }
      catch (Exception exception) {
          Console.WriteLine(exception.Message);
      }
    }

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

    private static System.Timers.Timer aTimer;

    private const int kImageWidth = 640;
    private const int kImageHeight = 480;
    private const int kPixelSize = 4;

    private byte[] buffer = new byte[kImageWidth * kImageHeight * kPixelSize];
    private byte[] notes = new byte[20];

    private float[] joint_positions = new float[20 * 3];

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      aTimer.Close();
      Shutdown();
    }
  }
}
