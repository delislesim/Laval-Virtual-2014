using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

      KinectPowerInterop.Initialize(false, false);
      //KinectPowerInterop.RecordSensor(0, "test.txt");
      KinectPowerInterop.StartPlaySensor(0, "test.txt");

      aTimer = new System.Timers.Timer(20);
      aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);

      aTimer.AutoReset = true;
      aTimer.Enabled = true;
    }

    bool pause_ = true;

    [MethodImpl(MethodImplOptions.Synchronized)]
    void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      if (pause_)
        return;
      pause_ = true;

      KinectPowerInterop.PlayNextFrame(0);

      KinectPowerInterop.GetPianoImage(buffer, (uint)buffer.Length);

      int stride = kImageWidth * kPixelSize;

      try
      {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
          image.Source = BitmapSource.Create(kImageWidth, kImageHeight, 96, 96,
                                             PixelFormats.Bgr32, null, buffer, stride);
        }));
      }
      catch (Exception exception)
      {
        Console.WriteLine("Exception dans aTime_Elapsed: " + exception.Message);
      }
    }

    private const int kImageWidth = 640;
    private const int kImageHeight = 280;
    private const int kPixelSize = 4;

    private static System.Timers.Timer aTimer;
    private byte[] buffer = new byte[kImageWidth * kImageHeight * kPixelSize];

    private void image_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      pause_ = false;
    }
  }
}
