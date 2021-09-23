using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RestPOS
{
  /// <summary>
  /// Interaction logic for HomeToolbar.xaml
  /// </summary>
  public partial class HomeToolbar : UserControl
  {
    public HomeToolbar()
    {
      InitializeComponent();
      lblUsername.Text = UserInfo.UserName;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        DispatcherTimer Timerdatetime = new DispatcherTimer();
        Timerdatetime.Tick += new EventHandler(Timerdatetime_Tick);
        Timerdatetime.Interval = new TimeSpan(0, 0, 0);
        Timerdatetime.Start();


      }
      catch
      {

      }
    }

    //// Timerdatetime Synchronization 
    private void Timerdatetime_Tick(object sender, EventArgs e)
    {
      try
      {
        lbldatetime.Text = DateTime.Now.ToString("dddd,  MMMM dd,  yyyy  hh:mm:ss  tt");
      }
      catch
      {

      }
    }

    private void btnHelpIndex_Click(object sender, RoutedEventArgs e)
    {
      Help.HelpIndex go = new Help.HelpIndex();
      go.Show();
    }

    private void btnkitchendisplay_Click(object sender, RoutedEventArgs e)
    {
      Items.Kitchen_Display go = new Items.Kitchen_Display();
      go.AllowsTransparency = false;
      go.WindowStyle = WindowStyle.SingleBorderWindow;
      go.Topmost = true;
      go.Show();
    }

  }
}
