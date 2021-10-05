using Microsoft.Win32;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PosCube.User_mgt
{
  /// <summary>
  /// Interaction logic for User_list.xaml
  /// </summary>
  public partial class User_list : Window
  {
    OpenFileDialog openFileDialog1 = new OpenFileDialog();
    ResourceManager res_man;
    CultureInfo cul;
    public User_list()
    {
      InitializeComponent();
    }

    //bind terminal
    public void Userslist(string value)
    {
      // var userimagepath =   new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\USERSIMAGE\\", UriKind.RelativeOrAbsolute);
      string sqlUserslist = " select * ,  imagename as  'userimagepath'  from usermgt  " +
                              " where name like '" + value + "%' OR username like '" + value + "%' " +
                              " OR contact like '" + value + "%' OR position like '" + value + "%' ";
      DataAccess.ExecuteSQL(sqlUserslist);
      DataTable dtUserslist = DataAccess.GetDataTable(sqlUserslist);
      lstvwUserslist.ItemsSource = dtUserslist.DefaultView;
      lblUserscount.Text = lstvwUserslist.Items.Count.ToString() + " Items Found";

      if (tabcontrolUserspanel.SelectedItem != tabUsersList)
      {
        tabcontrolUserspanel.SelectedItem = tabUsersList;
      }
      txtSearch.Focus();
    }

    // Load User Info for Update 
    public void UserDetailsData(string Uid)
    {
      string sql3 = "select * from usermgt where id = '" + Uid + "'";
      DataAccess.ExecuteSQL(sql3);
      DataTable dt1 = DataAccess.GetDataTable(sql3);

      txtUserFullName.Text = dt1.Rows[0].ItemArray[1].ToString();
      txtFatherName.Text = dt1.Rows[0].ItemArray[2].ToString();
      txtAddress.Text = dt1.Rows[0].ItemArray[3].ToString();
      txtEmailaddress.Text = dt1.Rows[0].ItemArray[4].ToString();
      txtContact.Text = dt1.Rows[0].ItemArray[5].ToString();
      dtDOB.Text = dt1.Rows[0].ItemArray[6].ToString();
      txtUsername.Text = dt1.Rows[0].ItemArray[7].ToString();
      textUserPass.Text = dt1.Rows[0].ItemArray[8].ToString();



      if (dt1.Rows[0].ItemArray[9].ToString() == "1")
      {
        rdbtnAdmin.IsChecked = true;
      }
      else if (dt1.Rows[0].ItemArray[9].ToString() == "2")
      {
        rdbtnManager.IsChecked = true;
      }
      else if (dt1.Rows[0].ItemArray[9].ToString() == "3")
      {
        rdbtnSalesMan.IsChecked = true;
      }
      else if (dt1.Rows[0].ItemArray[9].ToString() == "0")
      {
        rdbtnblock.IsChecked = true;
      }
      else
      {
        // rdbtnInactive.Checked = true;
      }
      cmboShopid.SelectedValue = dt1.Rows[0].ItemArray[12].ToString();
      lblimagename.Text = dt1.Rows[0].ItemArray[11].ToString();

      btnAddnew.Visibility = Visibility.Visible;
      btnDelete.Visibility = Visibility.Visible;
      lbluserid.Visibility = Visibility.Visible;
      txtUsername.IsReadOnly = true;
      // btnSave.Content = "Update";

      byte[] img = (byte[])(dt1.Rows[0].ItemArray[11]);
      if (img == null)
      {
        picUserimage.Source = null;
      }
      else
      {
        Stream StreamObj = new MemoryStream(img);
        BitmapImage BitObj = new BitmapImage();
        BitObj.BeginInit();
        BitObj.StreamSource = StreamObj;
        BitObj.EndInit();
        this.picUserimage.Source = BitObj;
      }


      if (dt1.Rows[0].ItemArray[13].ToString() == "1")
      {
        rdbtnSRdefault.IsChecked = true;
      }
      else if (dt1.Rows[0].ItemArray[13].ToString() == "2")
      {
        rdbtnSRpink.IsChecked = true;
      }

    }

    //public static ImageSource BitmapFromUri(Uri source)
    //{
    //    var bitmap = new BitmapImage();
    //    bitmap.BeginInit();
    //    bitmap.UriSource = source;
    //    bitmap.CacheOption = BitmapCacheOption.OnLoad;
    //    bitmap.EndInit();
    //    return bitmap;
    //}

    private void UsersList_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        string sqlloc = "select   *   from tbl_terminallocation ";
        DataAccess.ExecuteSQL(sqlloc);
        DataTable dtloc = DataAccess.GetDataTable(sqlloc);
        cmboShopid.ItemsSource = dtloc.DefaultView;
        cmboShopid.DisplayMemberPath = "branchname";
        cmboShopid.SelectedValuePath = "shopid";

        Userslist("");
        //   var uriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\user.png", UriKind.RelativeOrAbsolute);
        //   picUserimage.Source = new BitmapImage(uriSource); 

        btnAddnew.Visibility = Visibility.Hidden;
        btnDelete.Visibility = Visibility.Hidden;
        lbluserid.Visibility = Visibility.Hidden;
        lblmsg.Visibility = Visibility.Hidden;

        switch_language();
      }
      catch
      {

      }
    }

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);
        lblusertitle.Content = res_man.GetString("lblusertitle", cul);
        tabUsersList.Header = res_man.GetString("lblusertitle", cul);
        tabUsersDetails.Header = res_man.GetString("btnAddnew", cul);

        lblnametitle.Text = res_man.GetString("lblNametitle", cul);
        lblfnametitle.Text = res_man.GetString("lblcontacttitle", cul);
        lblemailtitle.Text = res_man.GetString("lblemailtitle", cul);
        lbladdresstitle.Text = res_man.GetString("lbladdresstitle", cul);
        lblcontacttitle.Text = res_man.GetString("lblcontacttitle", cul);
        lbldobtitle.Text = res_man.GetString("lbldobtitle", cul);
        lbllogidtitle.Text = res_man.GetString("lbllogidtitle", cul);
        lblpasswordtitle.Text = res_man.GetString("lblpasswordtitle", cul);
        lbluserttypetitle.Text = res_man.GetString("lbltypetitle", cul);
        lblassigntitle.Text = res_man.GetString("lblassigntitle", cul);
        lblselectsrtypetitle.Text = res_man.GetString("lblselectsrtypetitle", cul);
        btnSave.Content = res_man.GetString("btnSave", cul);
        btnAddnew.Content = res_man.GetString("btnAddnew", cul);
        btnDelete.Content = res_man.GetString("btnDelete", cul);
        btnBrowse.Content = res_man.GetString("btnBrowse", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }

    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        Userslist(txtSearch.Text);
      }
      catch
      {
      }
    }

    private void lstvwUserslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string userid = "";
        string username = "";
        foreach (DataRowView row in lstvwUserslist.SelectedItems)
        {
          userid = row.Row[0].ToString();
          username = row.Row[7].ToString();
        }

        tabcontrolUserspanel.SelectedItem = tabUsersDetails;
        tabUsersDetails.Header = "User Details View";
        lbluserid.Text = userid;
        lblmsg.Text = "-";
        UserDetailsData(userid);

        tabworklogs.Visibility = Visibility.Visible;
        UserInfo.usernamWK = username;
      }
      catch
      {

      }
    }

    private void lstvwUserslist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      try
      {
        string userid = "";
        foreach (DataRowView row in lstvwUserslist.SelectedItems)
        {
          userid = row.Row[0].ToString();
        }

        tabcontrolUserspanel.SelectedItem = tabUsersDetails;
        tabUsersDetails.Header = "User Details View";
        lbluserid.Text = userid;
        lblmsg.Text = "-";
        UserDetailsData(userid);
      }
      catch
      {

      }

    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {

      openFileDialog1.CheckFileExists = true;
      openFileDialog1.CheckPathExists = true;

      openFileDialog1.DefaultExt = ".jpg";
      // openFileDialog1.Filter = "GIF files (*.gif)|*.gif| jpg files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
      openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg| PNG files (*.png)|*.png";

      openFileDialog1.FilterIndex = 2;
      openFileDialog1.RestoreDirectory = true;


      if (openFileDialog1.ShowDialog() == true)
      {
        //  lblFileExtension.Text =  openFileDialog.FileName;
        picUserimage.Source = new BitmapImage(new Uri(openFileDialog1.FileName));
        //// textBox1.Text = openFileDialog1.FileName;
        //picItemimage.Source = openFileDialog1.FileName;
        lblFileExtension.Text = System.IO.Path.GetExtension(openFileDialog1.FileName);
      }


    }

    // Save if not UID | Update if UID present
    private void btnSave_Click(object sender, RoutedEventArgs e)
    {

      if (txtUserFullName.Text == "")
      {
        MessageBox.Show("Please Add User full Name", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtUserFullName.Focus();
      }
      else if (txtFatherName.Text == "")
      {
        MessageBox.Show("Please fill fathers name", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtFatherName.Focus();
      }
      else if (txtAddress.Text == "")
      {
        MessageBox.Show("Please Add Address", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtAddress.Focus();
      }
      else if (txtContact.Text == "")
      {
        MessageBox.Show("Please Add Contact Number", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtContact.Focus();
      }
      else if (txtUsername.Text == "")
      {
        MessageBox.Show("Please Add Username \n Username should be unique", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtUsername.Focus();
      }
      else if (txtEmailaddress.Text == "")
      {
        MessageBox.Show("Please Add  Email address", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        txtEmailaddress.Focus();
      }
      else if (textUserPass.Text == "")
      {
        MessageBox.Show("Please Add  Password", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        textUserPass.Focus();
      }
      else if (cmboShopid.Text == "")
      {
        MessageBox.Show("Please Select  Shop Location", "Fill Field", MessageBoxButton.OK, MessageBoxImage.Information);
        cmboShopid.Focus();
      }
      else
      {
        try
        {

          int flag;
          if (rdbtnAdmin.IsChecked == true)
          {
            flag = 1;
          }
          else if (rdbtnManager.IsChecked == true)
          {
            flag = 2;
          }
          else if (rdbtnSalesMan.IsChecked == true)
          {
            flag = 3;
          }
          else if (rdbtnblock.IsChecked == true)
          {
            flag = 0;
          }
          else
          {
            flag = 0;
          }
          string posi;
          if (rdbtnAdmin.IsChecked == true)
          {
            posi = "Admin";
          }
          else if (rdbtnManager.IsChecked == true)
          {
            posi = "Manager";
          }
          else if (rdbtnSalesMan.IsChecked == true)
          {
            posi = "Salesman";
          }
          else if (rdbtnblock.IsChecked == true)
          {
            posi = "Block";
          }
          else
          {
            posi = "0";
          }

          int srstyle = 1;
          if (rdbtnSRdefault.IsChecked == true)
          {
            srstyle = 1;
          }
          else if (rdbtnSRpink.IsChecked == true)
          {
            srstyle = 2;
          }

          //New Insert / New Entry
          if (lbluserid.Text == "-")
          {
            string imageName = openFileDialog1.FileName;
            if (lblFileExtension.Text == "user.png")
            {
              string sql1 = " insert into usermgt (name, father_name, address, email, contact, dob, username, password, usertype, position, shopid, srstyle ) " +
                      " Values ('" + txtUserFullName.Text + "', '" + txtFatherName.Text + "', '" + txtAddress.Text + "', '" + txtEmailaddress.Text + "', " +
                      " '" + txtContact.Text + "',  '" + dtDOB.Text + "', '" + txtUsername.Text + "', '" + textUserPass.Text + "', " +
                      " '" + flag + "', '" + posi + "', '" + cmboShopid.SelectedValue + "', '" + srstyle + "') ";
              DataAccess.ExecuteSQL(sql1);
            }
            else
            {
              string sql1 = " insert into usermgt (imagename, name, father_name, address, email, contact, dob, username, password, usertype, position, shopid, srstyle ) " +
                      " SELECT BulkColumn, '" + txtUserFullName.Text + "', '" + txtFatherName.Text + "', '" + txtAddress.Text + "', '" + txtEmailaddress.Text + "', " +
                      " '" + txtContact.Text + "', '" + dtDOB.Text + "', '" + txtUsername.Text + "', '" + textUserPass.Text + "', " +
                      " '" + flag + "', '" + posi + "', '" + cmboShopid.SelectedValue + "', '" + srstyle + "' " +
                      " FROM Openrowset ( Bulk '" + imageName + "', Single_Blob) as img ";
              DataAccess.ExecuteSQL(sql1);
            }


            lblmsg.Visibility = Visibility.Visible;
            lblmsg.Text = "User hase been Created Successfully";
            picUserimage.Source = null;
            ClearForm();
            Userslist("");

          }
          else // Update info
          {
            string imageName;
            if (lblFileExtension.Text == "user.png")
            {
              // imageName = lblimagename.Text;  //Unchange pictures     imagename = '" + imageName + "' ,   
              string sql = " UPDATE usermgt set  name = '" + txtUserFullName.Text + "', father_name = '" + txtFatherName.Text + "', " +
              " address = '" + txtAddress.Text + "', email = '" + txtEmailaddress.Text + "', Contact = '" + txtContact.Text + "', " +
              " dob = '" + dtDOB.Text + "', username= '" + txtUsername.Text + "', password = '" + textUserPass.Text + "',  " +
              " usertype    = '" + flag + "', position = '" + posi + "', shopid = '" + cmboShopid.SelectedValue + "', srstyle = '" + srstyle + "' " +
              " where (id = '" + lbluserid.Text + "' )";
              DataAccess.ExecuteSQL(sql);
            }
            else  //When change 
            {
              imageName = " ( SELECT BulkColumn FROM OPENROWSET ( BULK  '" + openFileDialog1.FileName + "', SINGLE_BLOB) as img ) ";
              string sql = " UPDATE usermgt set  imagename = " + imageName + ",  name = '" + txtUserFullName.Text + "', father_name = '" + txtFatherName.Text + "', " +
              " address = '" + txtAddress.Text + "', email = '" + txtEmailaddress.Text + "', Contact = '" + txtContact.Text + "', " +
              " dob = '" + dtDOB.Text + "', username= '" + txtUsername.Text + "', password = '" + textUserPass.Text + "',  " +
              " usertype    = '" + flag + "', position = '" + posi + "', shopid = '" + cmboShopid.SelectedValue + "', srstyle = '" + srstyle + "' " +
              " where (id = '" + lbluserid.Text + "') ";
              DataAccess.ExecuteSQL(sql);
            }

            lblmsg.Visibility = Visibility.Visible;
            lblmsg.Text = "Successfully Data Updated";
            Userslist("");
          }

        }
        catch (Exception exp)
        {
          MessageBox.Show("Sorry\r\n" + exp.Message);
        }
      }
    }

    private void ClearForm()
    {
      txtUsername.IsReadOnly = false;
      txtUserFullName.Text = string.Empty;
      txtFatherName.Text = string.Empty;
      txtAddress.Text = string.Empty;
      txtContact.Text = string.Empty;
      txtUsername.Text = string.Empty;
      textUserPass.Text = string.Empty;
      txtEmailaddress.Text = string.Empty;
      dtDOB.Text = string.Empty;
      picUserimage.Source = null;
      btnSave.Content = "Save";
      tabUsersDetails.Header = "Add new";
      lbluserid.Text = "-";


    }

    private void btnAddnew_Click(object sender, RoutedEventArgs e)
    {
      ClearForm();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (lbluserid.Text == "-" || lbluserid.Text == "")
      {
        MessageBox.Show("You are Not able to Delete Please select item", "Question", MessageBoxButton.OK, MessageBoxImage.Warning);
      }
      else
      {
        try
        {
          if (MessageBox.Show("Do you want to Delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {

            string sql = "delete from usermgt where   (id = '" + lbluserid.Text + "' )";
            DataAccess.ExecuteSQL(sql);
            lstvwUserslist.ItemsSource = null;
            picUserimage.Source = null;

            string sqlworkrecord = " delete from tbl_workrecords  where  username = '" + txtUsername.Text + "' ";
            DataAccess.ExecuteSQL(sqlworkrecord);

            //  var destinatiopath = System.AppDomain.CurrentDomain.BaseDirectory + @"USERSIMAGE\" + lblimagename.Text;                          
            //   System.IO.File.Delete(destinatiopath);


            lblmsg.Text = "Successfully Data Delete !";
            Userslist("");
          }

        }
        catch (Exception exp)
        {
          MessageBox.Show("Sorry\r\n You have to Check the Data \n " + exp.Message);
        }
      }

    }

    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      Home go = new Home();
      go.Show();

    }


  }
}
