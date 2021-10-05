using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PosCube.User_mgt
{
  /// <summary>
  /// Interaction logic for WorkRecords.xaml
  /// </summary>
  public partial class WorkRecords : UserControl
  {
    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    public WorkRecords()
    {
      InitializeComponent();
    }

    public void Databind(string dtstart, string dtend)
    {
      string sql = " SELECT * FROM vw_workrecords " +
                   " where Date BETWEEN '" + dtstart + "' AND    '" + dtend + "'  and " +
                   " username = '" + UserInfo.usernamWK + "'    order by  Date ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);
      dtgrdWorkingHoursList.ItemsSource = dt1.DefaultView;
    }

    private void WorkRecords_Form_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        dtStartdate.Text = DateTime.Now.AddDays(-7).ToString();
        dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        Databind(DateTime.Now.AddDays(-7).ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
      }
      catch
      {

      }

    }

    private void dtENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        Databind(dtStartdate.Text, dtENDdate.Text);
      }
      catch
      {

      }
    }

    //Last 30 days Worked Hours  
    private void btn30days_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //DateTime dtd = DateTime.Now;
        //dtd = dtd.AddDays(-30);

        //string sql = " select  username , logdate as 'Date' ,  min( logtime) as 'IN' , max( logtime)  as 'OUT' " +
        //              " , CAST(((strftime('%s', max(logtime)  ) - strftime('%s',  min(logtime) )) % (60 * 60 * 24)) / (60 * 60) AS TEXT) || ':' || " +
        //              "   CAST((((strftime('%s', max(logtime)  ) - strftime('%s',  min(logtime) )) % (60 * 60 * 24)) % (60 * 60)) / 60 AS TEXT) as 'HOURS - HH:MM'  " +
        //              " from tbl_workrecords where logdate BETWEEN '" + dtd.ToString("yyyy-MM-dd") + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  and " +
        //              " username = '" + UserInfo.usernamWK + "'   group by username , logdate ";
        //DataAccess.ExecuteSQL(sql);
        //DataTable dt1 = DataAccess.GetDataTable(sql);
        //dtgrdWorkingHoursList.ItemsSource = dt1.DefaultView;


        dtStartdate.Text = DateTime.Now.AddDays(-30).ToString();
        dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        Databind(DateTime.Now.AddDays(-30).ToString(), DateTime.Now.ToString("yyyy-MM-dd"));

      }
      catch { }
    }

    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //Exporting to xls.     
        saveFileDialog1.FileName = "WorkedHours_" + UserInfo.usernamWK + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xls";
        saveFileDialog1.ShowDialog();


        dtgrdWorkingHoursList.SelectAllCells();
        dtgrdWorkingHoursList.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dtgrdWorkingHoursList);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dtgrdWorkingHoursList.UnselectAllCells();
        string name = saveFileDialog1.FileName;
        System.IO.StreamWriter file1 = new System.IO.StreamWriter(name);
        file1.WriteLine(result.Replace(',', ' '));
        file1.Close();
      }
      catch
      {

      }




    }


  }
}
