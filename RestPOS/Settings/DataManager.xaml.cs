using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace PosCube.Settings
{
  /// <summary>
  /// Interaction logic for DataManager.xaml
  /// </summary>
  public partial class DataManager : UserControl
  {
    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    OpenFileDialog openFileDialog1 = new OpenFileDialog();

    ResourceManager res_man;
    CultureInfo cul;
    public DataManager()
    {
      InitializeComponent();
      switch_language();
    }


    ///// Select Folder Destination
    private void btnBrowsebkup_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
      if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtfilepathbackup.Text = dlg.SelectedPath;
        btnDataSaveas.IsEnabled = true;

      }
    }

    //// Save Databackup  
    private void btnDataSaveas_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        string sql = " BACKUP DATABASE RestPOS  " +
                       " TO DISK = '" + txtfilepathbackup.Text + "\\" + "PosCube_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-fff") + ".bak' ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt5 = DataAccess.GetDataTable(sql);
        MessageBox.Show("Successfully database backup has been Completed");
      }
      catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
      }
    }

    //// Reset system // Truncate Table
    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (MessageBox.Show("Do you want Reset Database ? \n you will be loss all Data", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
          string sql1 = " TRUNCATE TABLE return_item; " +
                          " DELETE FROM usermgt where usertype != '1'; " +
                          " TRUNCATE TABLE sales_item; " +
                          " TRUNCATE TABLE sales_payment; " +
                          " TRUNCATE TABLE purchase; " +
                          " TRUNCATE TABLE tbl_duepayment; ";
          DataTable dt1 = DataAccess.GetDataTable(sql1);
          MessageBox.Show("Successfully truncated !!! ", "Success", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
      }
      catch
      {
      }
    }

    // Select Database file ( .bak )
    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {
      openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      openFileDialog1.Title = "Browse Text Files";

      //openFileDialog1.CheckFileExists = true;
      //openFileDialog1.CheckPathExists = true;
      openFileDialog1.FilterIndex = 1;

      // openFileDialog1.DefaultExt = "bak";
      openFileDialog1.Filter = "SQL Backup file(.bak)|*.bak| All Files(*.*)|*.*";
      //openFileDialog1.Filter = "All files (*.*)|*.* | jpg files (*.jpg)|*.jpg";

      if (openFileDialog1.ShowDialog() == true)
      {
        // textBox1.Text = openFileDialog1.FileName;
        txtfilepath.Text = openFileDialog1.FileName;
      }
    }

    private void btnTruncate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (txtfilepath.Text == string.Empty)
        {
          MessageBox.Show("Please Select Database file first", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
          txtfilepath.Focus();
        }
        else
        {

          if (MessageBox.Show("Do you want Restore Databackup \n After Press Yes you loss your previous Database ", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            string sql = " USE master;     ALTER DATABASE RestPOS " +
                      " SET SINGLE_USER WITH " +
                      " ROLLBACK IMMEDIATE; " +
                      " RESTORE DATABASE RestPOS " +
                      " FROM DISK = '" + txtfilepath.Text + "' " +
                      " WITH REPLACE; " +
                      " ALTER DATABASE RestPOS SET MULTI_USER ";
            DataAccess.ExecuteSQL(sql);
            DataTable dt5 = DataAccess.GetDataTable(sql);

            MessageBox.Show("Database has been Successfully Restored  \n\n Please close the Software and Restart again.", "Successful", MessageBoxButton.OK, MessageBoxImage.Exclamation);
          }

        }

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

        lblresettitle.Text = res_man.GetString("lblresettitle", cul);
        lbltruncatedeslabel.Text = res_man.GetString("lbltruncatedeslabel", cul);
        lblrestoretitle.Text = res_man.GetString("lblrestoretitle", cul);
        lblrestoredescription.Text = res_man.GetString("lblrestoredescription", cul);

        btnDataSaveas.Content = res_man.GetString("btnDataSaveas", cul);
        btnReset.Content = res_man.GetString("btnReset", cul);
        btnTruncate.Content = res_man.GetString("btnTruncate", cul);
        btnBrowse.Content = res_man.GetString("btnBrowse", cul);
        btnBrowsebkup.Content = res_man.GetString("btnBrowse", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }

    }
  }
}
