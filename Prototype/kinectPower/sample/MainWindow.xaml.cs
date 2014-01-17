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

      aTimer = new System.Timers.Timer(100);
      aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);

      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }
    
    void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      int buffer_size = 640 * 480 * 4;
      byte[] buffer = new byte[buffer_size];

      bool test = GetNiceDepthMap(buffer, buffer_size);

      Console.WriteLine("tutu");
    }
    
    [DllImport(@"kinect_power.dll", EntryPoint = "GetNiceDepthMap", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetNiceDepthMap(byte[] buffer, int buffer_size);

    private static System.Timers.Timer aTimer;
  }
}
