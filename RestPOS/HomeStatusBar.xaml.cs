using System.Windows;
using System.Windows.Controls;

namespace PosCube
{
  /// <summary>
  /// Interaction logic for HomeStatusBar.xaml
  /// </summary>
  public partial class HomeStatusBar : UserControl
  {
    public HomeStatusBar()
    {
      InitializeComponent();
    }



    private void btnHomeMenuLink_Click_1(object sender, RoutedEventArgs e)
    {
      //s.Visibility = Visibility.Hidden;  
      Home go = new Home();
      go.Show();

      // this.Hide();


    }



  }
}
