﻿using System;
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

      Initialize();

      aTimer = new System.Timers.Timer(20);
      aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);

      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      uint buffer_size = kImageWidth * kImageHeight * kPixelSize;

      if (!GetNiceDepthMap(buffer, buffer_size))
        return;

      int stride = kImageWidth * kPixelSize;

      Application.Current.Dispatcher.Invoke(new Action(() =>
      {
        image1.Source = BitmapSource.Create(kImageWidth, kImageHeight, 96, 96,
                                          PixelFormats.Bgr32, null, buffer, stride);
      }));
    }

    [DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetNiceDepthMap(byte[] buffer, uint buffer_size);

    [DllImport(@"kinect_power.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool Initialize();

    private static System.Timers.Timer aTimer;

    private const int kImageWidth = 640;
    private const int kImageHeight = 480;
    private const int kPixelSize = 4;

    private byte[] buffer = new byte[kImageWidth * kImageHeight * kPixelSize];
  }
}
