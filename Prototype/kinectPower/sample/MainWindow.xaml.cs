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
      int i = tutu();
      Console.WriteLine(i);
    }
    
    [DllImport(@"kinect_power.dll", EntryPoint = "tutu")]
    public static extern int tutu();

    private static System.Timers.Timer aTimer;
  }
}
