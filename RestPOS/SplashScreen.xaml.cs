using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PosCube
{
  /// <summary>
  /// Interaction logic for SplashScreen.xaml
  /// </summary>
  public partial class SplashScreen : Window
  {
    private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
    DispatcherTimer Timerdatetime = new DispatcherTimer();
    public SplashScreen()
    {
      InitializeComponent();

      //double W = (System.Windows.SystemParameters.PrimaryScreenWidth  * 42 ) / 100 ; 
      //double H = (System.Windows.SystemParameters.PrimaryScreenHeight * 40 ) / 100 ;           
      //lblDevelopername.Margin = new Thickness(W, H, 0, 0);

      // double W2 = (System.Windows.SystemParameters.PrimaryScreenWidth * 47) / 100;
      // double H2 = (System.Windows.SystemParameters.PrimaryScreenHeight * 44.5) / 100;
      // lblversion.Margin = new Thickness(W2, H2, 0, 0);

      // double W3 = (System.Windows.SystemParameters.PrimaryScreenWidth * 43) / 100;
      // double H3 = (System.Windows.SystemParameters.PrimaryScreenHeight * 75) / 100;
      // progressBar1.Margin = new Thickness(W3, H3, 0, 0);




      Timerdatetime.Tick += new EventHandler(Timerdatetime_Tick);
      Timerdatetime.Interval = new TimeSpan(0, 0, 0);
      Timerdatetime.Start();
    }



    //// Move to Login page 
    private void Timerdatetime_Tick(object sender, EventArgs e)
    {
      try
      {
        progressBar1.Minimum = 0;
        progressBar1.Maximum = short.MaxValue;
        progressBar1.Value = 0;

        //Stores the value of the ProgressBar
        double value = 0;

        //Create a new instance of our ProgressBar Delegate that points
        // to the ProgressBar's SetValue method.
        UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);

        //Tight Loop: Loop until the ProgressBar.Value reaches the max
        do
        {
          value += 10;
          Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });
        }
        while (progressBar1.Value != progressBar1.Maximum);

        //this.progressBar1.Value = System.DateTime.Now.Second;// +9910;
        if (progressBar1.Value == progressBar1.Maximum)
        {
          this.Visibility = Visibility.Hidden;
          Login go = new Login();
          go.Show();

          Timerdatetime.Stop();
        }
      }
      catch
      {

      }
    }


  }
}
