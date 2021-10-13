using Microsoft.Win32;
using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PosCube.Reports
{
  /// <summary>
  /// Interaction logic for SalesReportsDetails.xaml
  /// </summary>
  public partial class SalesReportsDetails : Window
  {
    ResourceManager res_man;
    CultureInfo cul;

    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    public SalesReportsDetails()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {

        CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
        ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        Thread.CurrentThread.CurrentCulture = ci;
        switch_language();
      }
      catch
      { }
    }

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);
        lblreporttitle.Text = res_man.GetString("lblreporttitle", cul);
        tabsalesreport.Header = res_man.GetString("tabsalesreport", cul);
        tabfindreceipt.Header = res_man.GetString("tabfindreceipt", cul);
        tabtopsales.Header = res_man.GetString("tabtopsales", cul);
        tabreturn.Header = res_man.GetString("tabreturn", cul);
        tabduereport.Header = res_man.GetString("tabduereport", cul);
        lblsalereporttitle.Content = res_man.GetString("lblsalereporttitle", cul);
        lblfindreporttitle.Content = res_man.GetString("lblfindreporttitle", cul);
        lblinfotofindreceipttitle.Text = res_man.GetString("lblinfotofindreceipttitle", cul);
        btnExport.Content = res_man.GetString("btnExport", cul);
        btnExportReceipt.Content = res_man.GetString("btnExport", cul);

        //btnSave.Content = res_man.GetString("btnSave", cul);
        //btnAddnew.Content = res_man.GetString("btnAddnew", cul);
        //btndelete.Content = res_man.GetString("btnDelete", cul);
        //btnBrowse.Content = res_man.GetString("btnBrowse", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }

    #region Sales item report section
    //   // sales items Report Date to date  
    private void dtENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string sql = " select   sales_time as 'Date', itemname as 'Name', retailsprice  as 'Price', qty,  total as 'Total',  " +
            " ((profit * qty) * 1.00) as 'Profit', discount as 'Dis Rate', sales_id as 'Invoice no' " +
            " from sales_item   where sales_time BETWEEN '" + dtStartdate.Text + "' AND    '" + dtENDdate.Text + "' " +
            " and status != 2  Order  by sales_time ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgrdViewSalesReport.ItemsSource = dt1.DefaultView;


        string sql3 = " select SUM(total), SUM(profit * qty) as Profit , SUM(discount) from sales_item   " +
                    " where sales_time BETWEEN '" + dtStartdate.Text + "' AND    '" + dtENDdate.Text + "' " +
                    " and status != 2  Order  by sales_time";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);

        DataRow dr = dt1.NewRow();
        dr[0] = " ";
        dt1.Rows.Add(dr);

        //Total  Sales
        DataRow dr4 = dt1.NewRow();
        dr4[0] = "Total  Sales:";
        dr4[4] = Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString());
        dt1.Rows.Add(dr4);

        DataRow dr6 = dt1.NewRow();
        dr6[0] = " ";
        dt1.Rows.Add(dr6);

        //Totla Profit
        DataRow dr5 = dt1.NewRow();
        dr5[0] = "Total Profit :";
        dr5[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString());
        dt1.Rows.Add(dr5);

        dtgrdViewSalesReport.Columns[0].Width = 120;
        dtgrdViewSalesReport.Columns[1].Width = 220;
        dtgrdViewSalesReport.Columns[2].Width = 70;
        dtgrdViewSalesReport.Columns[4].Width = 100;
        dtgrdViewSalesReport.Columns[5].Width = 90;
      }
      catch { }

    }

    private void btnTodaySales_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        dtStartdate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        string sql = " select   sales_time as 'Date', itemname as 'Name', retailsprice  as 'Price', qty, total as 'Total',   " +
            " ((profit * qty) * 1.00) as 'Profit', discount as 'Dis Rate', sales_id as 'Invoice no' " +
            " from sales_item   where sales_time BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "' " +
            " and status != 2  Order  by sales_time";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgrdViewSalesReport.ItemsSource = dt1.DefaultView;

        string sql3 = " select SUM(total), SUM(profit * qty) as Profit , SUM(discount) from sales_item   " +
                      " where sales_time BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "' " +
                      " and status != 2  Order  by sales_time";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);

        DataRow dr = dt1.NewRow();
        dr[0] = " ";
        dt1.Rows.Add(dr);

        //Total  Sales
        DataRow dr4 = dt1.NewRow();
        dr4[0] = "Total  Sales:";
        dr4[4] = Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString());
        dt1.Rows.Add(dr4);

        DataRow dr6 = dt1.NewRow();
        dr6[0] = " ";
        dt1.Rows.Add(dr6);

        //Totla Profit
        DataRow dr5 = dt1.NewRow();
        dr5[0] = "Total Profit :";
        dr5[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString());
        dt1.Rows.Add(dr5);

        dtgrdViewSalesReport.Columns[0].Width = 120;
        dtgrdViewSalesReport.Columns[1].Width = 220;
        dtgrdViewSalesReport.Columns[2].Width = 70;
        dtgrdViewSalesReport.Columns[4].Width = 100;
        dtgrdViewSalesReport.Columns[5].Width = 90;
      }
      catch { }

    }

    //Print button  | item sale report
    private void OnDataGridPrinting(object sender, RoutedEventArgs e)
    {
      if (dtgrdViewSalesReport.Items.Count <= 0)
      {
        MessageBox.Show("Please selecct date");
      }
      else
      {
        System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
        if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
        {
          Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
          // sizing of the element.
          dtgrdViewSalesReport.Measure(pageSize);
          //  Printdlg.PageRangeSelection = PageRangeSelection.AllPages;
          Printdlg.UserPageRangeEnabled = true;
          Printdlg.PageRange = new PageRange(1, 113);
          dtgrdViewSalesReport.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
          //  Printdlg.ShowDialog();
          Printdlg.PrintVisual(dtgrdViewSalesReport, "Sales_Report_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
        }
      }


    }

    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //Exporting to xls.     
        saveFileDialog1.FileName = "Sales_Report_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xls";
        saveFileDialog1.ShowDialog();


        dtgrdViewSalesReport.SelectAllCells();
        dtgrdViewSalesReport.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dtgrdViewSalesReport);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dtgrdViewSalesReport.UnselectAllCells();
        string name = saveFileDialog1.FileName;
        System.IO.StreamWriter file1 = new System.IO.StreamWriter(name);
        file1.WriteLine(result.Replace(',', ' '));
        file1.Close();
      }
      catch
      {

      }




    }
    #endregion

    #region Receipt  find report section
    //  Receipt Find  
    private void dtRptENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string sql = "select  sales_id as 'Invoice no', sales_time as Date , payment_amount as Total , emp_id as 'Sold by', " +
              " dis as Discount , vat as TAX ,  payment_type as 'Payment Type' ,  due_amount as Due, Comment as Comments " +
              "  from sales_payment where sales_time BETWEEN '" + dtRptStartdate.Text + "' AND    '" + dtRptENDdate.Text + "' " +
              "  Order  by sales_id desc ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtReceiptPrint.ItemsSource = dt1.DefaultView;
        //  dtReceiptPrint.Columns[0].Width = 110;
        // dtReceiptPrint.Columns[1].Width = 180;
      }
      catch { }

    }

    //Last 30 days Report
    private void btn30days_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        dtRptStartdate.Text = DateTime.Now.AddDays(-30).ToString();
        dtRptENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        string sql = "select  sales_id as 'Invoice no' , sales_time as Date , payment_amount as Total , emp_id as 'Sold by', " +
              " dis as Discount , vat as TAX ,  payment_type as 'Payment Type' ,  due_amount as Due, Comment as Comments " +
              "  from sales_payment where sales_time BETWEEN '" + dtRptStartdate.Text + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "' " +
              "  Order  by sales_id desc ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtReceiptPrint.ItemsSource = dt1.DefaultView;

      }
      catch { }
    }

    //View Receipt Print
    private void dtReceiptPrint_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      try
      {
        DataRowView dataRow = (DataRowView)dtReceiptPrint.SelectedItem;
        int index = dtReceiptPrint.CurrentCell.Column.DisplayIndex;
        //= dataRow.Row.ItemArray[0].ToString();

        parameter.autoprint = "0";
        Sales_Register.ReceiptPrint go = new Sales_Register.ReceiptPrint(dataRow.Row.ItemArray[0].ToString());
        go.ShowDialog();
      }
      catch
      {

      }

    }

    private void btnExportReceipt_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //Exporting to xls.     
        saveFileDialog1.FileName = "Payment_Report_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xls";
        saveFileDialog1.ShowDialog();


        dtReceiptPrint.SelectAllCells();
        dtReceiptPrint.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dtReceiptPrint);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dtReceiptPrint.UnselectAllCells();
        string name = saveFileDialog1.FileName;
        System.IO.StreamWriter file1 = new System.IO.StreamWriter(name);
        file1.WriteLine(result.Replace(',', ' '));
        file1.Close();
      }
      catch
      {

      }
    }

    #endregion

    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      Home go = new Home();
      go.Show();
    }

        private void Overview_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
