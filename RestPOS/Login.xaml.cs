//using Activationconfig;
using System;
using System.Data;
using System.Windows;


namespace PosCube
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// // Code Design By Tuaha - DynamicSoft
  /// citkar@live.com
  /// </summary>
  public partial class Login : Window
  {
    private const double topOffset = 180;
    private const double leftOffset = 780;
    readonly GrowlNotifiactions growlNotifications = new GrowlNotifiactions();

    public Login()
    {
      InitializeComponent();
      growlNotifications.Top = SystemParameters.WorkArea.Top + topOffset;
      growlNotifications.Left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - leftOffset;
      ResulationHW();
      //  txtUserName.Text = "admin";
      // txtPassword.Password = "admin";
      //  rdbtnLang2ndoptions.IsChecked = true;
    }

    //Dynamic Resulation
    public void ResulationHW()
    {
      if (System.Windows.SystemParameters.PrimaryScreenWidth == 1920)
      {
        double W = (System.Windows.SystemParameters.PrimaryScreenWidth * 12) / 100;
        double H = (System.Windows.SystemParameters.PrimaryScreenHeight * 16) / 100;
        MainGrid.Margin = new Thickness(W, H, 0, 0);
        Toolbargrid.Margin = new Thickness(0, 20, W, 0);
      }
      else
      {
        double W = (System.Windows.SystemParameters.PrimaryScreenWidth * 1.6) / 100;
        double H = (System.Windows.SystemParameters.PrimaryScreenHeight * 3) / 100;
        MainGrid.Margin = new Thickness(W, H, 0, 0);
      }
    }
    private void btnSignin_Click(object sender, RoutedEventArgs e)
    {
      if (txtUserName.Text == "")
      {
        //MessageBox.Show("");
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert User Name", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        //txtUserName.Focus();

      }
      else if (txtPassword.Password.ToString() == string.Empty)
      {
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert Password", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        //txtPassword.Focus();
      }
      else
      {
        try
        {
          string tkhan = " Select username , password , usertype, shopid, srstyle  from  usermgt  " +
                          " where username   = '" + txtUserName.Text + "' and password = '" + txtPassword.Password.ToString() + "'";
          DataTable dt = DataAccess.GetDataTable(tkhan);

          string username = dt.Rows[0].ItemArray[0].ToString();
          string password = dt.Rows[0].ItemArray[1].ToString();
          string usertype = dt.Rows[0].ItemArray[2].ToString();
          string Shopid = dt.Rows[0].ItemArray[3].ToString();
          string DefaultSR = dt.Rows[0].ItemArray[4].ToString();

          if (txtUserName.Text == username && txtPassword.Password.ToString() == password)
          {
            if (Shopid != string.Empty) // When Shop is available or assign
            {
              if (usertype == "1")   //usertype usertype
              {
                UserInfo.UserName = txtUserName.Text;
                UserInfo.usertype = "1"; // 1= admin
                UserInfo.Shopid = Shopid;
                UserInfo.DefaulfSRWindow = DefaultSR;
                if (rdbtnLang2ndoptions.IsChecked == true)
                {
                  language.ID = "1";  // 1 = is 2nd language  
                  language.languagecode = language.languagecodevalue;
                }
                else
                {
                  language.ID = "2";  //  2 = English                                     
                }
                workRecords();
                Home go = new Home();
                go.Show();
                this.Hide();
              }
              if (usertype == "2")
              {
                UserInfo.UserName = txtUserName.Text;
                UserInfo.usertype = "2"; //2 = Manager
                UserInfo.Shopid = Shopid;
                UserInfo.DefaulfSRWindow = DefaultSR;
                if (rdbtnLang2ndoptions.IsChecked == true)
                {
                  language.ID = "1";  // 1 = is 2nd language  
                  language.languagecode = language.languagecodevalue;
                }
                else
                {
                  language.ID = "2";  //  2 = English
                }
                workRecords();
                Home go = new Home();
                go.Show();
                this.Hide();
              }

              if (usertype == "3")
              {
                UserInfo.UserName = txtUserName.Text;
                UserInfo.usertype = "3"; //3 = salesman
                UserInfo.Shopid = Shopid;
                UserInfo.DefaulfSRWindow = DefaultSR;
                if (rdbtnLang2ndoptions.IsChecked == true)
                {
                  language.ID = "1";  // 1 = is 2nd language  
                  language.languagecode = language.languagecodevalue;
                }
                else
                {
                  language.ID = "2";  //  2 = English
                }
                workRecords();
                Home go = new Home();
                go.Show();
                this.Hide();
              }
              if (usertype == "0") // Block user
              {
                growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = " This user (" + txtUserName.Text + ") has been blocked.  Please contact to administrator.", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
              }
            }
            else  // When User has not assign Shop
            {
              growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = " This user has not assigned any Shop or Terminal or Branch. Please contact to administrator.", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
            }

          }
          else
          {
            growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Username or Password does not match", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
            //  lblmsg.Text = "Username or Password does not match";

          }
        }
        catch (Exception exe)
        {
          growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Username or Password does not match", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        }

      }
    }

    public void workRecords()
    {
      string logdate = DateTime.Now.ToString("yyyy-MM-dd");
      string logtime = DateTime.Now.ToString("HH:mm:ss");
      string logdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

      string sqlLogIn = " insert into tbl_workrecords (Username, datatype, logdate, logtime, logdatetime) " +
                           " values ('" + UserInfo.UserName + "' , 'IN' , '" + logdate + "' , " +
                            " '" + logtime + "' , '" + logdatetime + "'  )";
      DataAccess.ExecuteSQL(sqlLogIn);
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      UserInfo.UserName = "admin";
      UserInfo.usertype = "1"; // 1= admin
      UserInfo.DefaulfSRWindow = "1";
      UserInfo.Shopid = "MTQC02";
      UserInfo.DefaulfSRWindow = "2"; // 1= Gn 2 = pink 
      if (rdbtnLang2ndoptions.IsChecked == true)
      {
        language.ID = "1";  // 1 = is 2nd language  
        language.languagecode = language.languagecodevalue;
      }
      else
      {
        language.ID = "2";  //  2 = English
      }

      workRecords();
      Home go = new Home();
      go.Show();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
      Environment.Exit(0);
      //System.Windows.Application.Current.Shutdown();
    }

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
      txtUserName.Text = string.Empty;
      txtPassword.Password = string.Empty;
      txtUserName.Focus();
    }

    private void MainGrid_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        expodtclass();
      }
      catch
      {

      }
    }

    #region not use cl
    public void expodtclass()
    {
      ///////////////// Ex date ////////////////////////////
      // if (Convert.ToInt32(InvoicesManager.Activationvalue) > Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"))) // Ex date > Curnt date              
      if(true)
      {
        btnSignin.Visibility = Visibility.Visible;
        btnLogin.Visibility = Visibility.Hidden;
        btnReset.Visibility = Visibility.Visible;
      }
      else
      {
        lblusertitle.Text = "Your license has been expired. Contact: dynamicsoftorg@gmail.com";
        lblpasstitle.Visibility = Visibility.Hidden;
        btnSignin.Visibility = Visibility.Hidden;
        btnLogin.Visibility = Visibility.Hidden;
        btnReset.Visibility = Visibility.Hidden;
        txtUserName.Visibility = Visibility.Hidden;
        txtPassword.Visibility = Visibility.Hidden;
        rdbtnEnglish.Visibility = Visibility.Hidden;
        rdbtnLang2ndoptions.Visibility = Visibility.Hidden;
      }
      // btnLogin.Visibility = Visibility.Visible; // Remove this after complete
    }
    #endregion


  }
}
