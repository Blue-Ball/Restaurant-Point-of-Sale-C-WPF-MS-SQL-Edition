using System.Diagnostics;
using System.Windows;

namespace RestPOS.Help
{
  /// <summary>
  /// Interaction logic for HelpIndex.xaml
  /// </summary>
  public partial class HelpIndex : Window
  {
    public HelpIndex()
    {
      InitializeComponent();
    }

    private void OnNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.AbsoluteUri);
      e.Handled = true;
    }


    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      //  Home go = new Home();
      //  go.Show();

    }

  }
}
