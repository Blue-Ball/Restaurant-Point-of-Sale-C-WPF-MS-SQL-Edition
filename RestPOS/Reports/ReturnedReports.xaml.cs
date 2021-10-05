using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PosCube.Reports
{
  /// <summary>
  /// Interaction logic for TopSales.xaml
  /// </summary>
  public partial class ReturnedReports : UserControl
  {
    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    public ReturnedReports()
    {
      InitializeComponent();
    }

    //Date to date
    private void dtENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {

        string sql1 = "select  return_time as 'Date', itemname as 'Item Name', retailsprice  as 'Price', qty,  Total, custno as CustID, " +
                        " SoldInvoiceNo as 'Invoice No', emp as 'Return by'  from return_item where return_time " +
                        " BETWEEN '" + dtStartdate.Text + "' AND    '" + dtENDdate.Text + "' Order  by return_time ";
        DataAccess.ExecuteSQL(sql1);
        DataTable dt1 = DataAccess.GetDataTable(sql1);
        dtviewReturnsTrnx.ItemsSource = dt1.DefaultView;

        string sql3 = "select SUM(Total), SUM(disamt), SUM(vatamt) from return_item " +
                     " where return_time   >='" + dtStartdate.Text + "' AND  return_time <='" + dtENDdate.Text + "' ";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);

        DataRow dr = dt1.NewRow();
        dr[0] = " ";
        dt1.Rows.Add(dr);

        DataRow dr4 = dt1.NewRow();
        dr4[0] = "Total  :";
        dr4[4] = Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString());
        dt1.Rows.Add(dr4);

        //DataRow dr6 = dt1.NewRow();
        //dr6[0] = " ";
        //dt1.Rows.Add(dr6);

        DataRow dr5 = dt1.NewRow();
        dr5[0] = "Total Discount: ";
        dr5[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString());
        dt1.Rows.Add(dr5);

        DataRow drvat = dt1.NewRow();
        drvat[0] = "Total TAX: ";
        drvat[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[2].ToString());
        dt1.Rows.Add(drvat);

        DataRow drtotalreturned = dt1.NewRow();
        drtotalreturned[0] = "Total Returned: ";
        drtotalreturned[4] = (Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString()) - Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString())) + Convert.ToDouble(dt3.Rows[0].ItemArray[2].ToString());
        dt1.Rows.Add(drtotalreturned);

        DataRow dr7 = dt1.NewRow();
        dr7[0] = " ";
        dt1.Rows.Add(dr7);

        DataRow rep = dt1.NewRow();
        rep[0] = "Return Report ";
        dt1.Rows.Add(rep);

        DataRow repdt = dt1.NewRow();
        repdt[0] = "From : " + dtStartdate.Text;
        repdt[1] = "To : " + dtENDdate.Text;
        dt1.Rows.Add(repdt);
      }
      catch
      { }
    }

    //Las t 30 days Return sales
    private void btn30days_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        dtStartdate.Text = DateTime.Now.AddDays(-30).ToString();
        dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        string sql1 = "select  return_time as 'Date', itemname as 'itemName', retailsprice  as 'Price', Qty,  Total, custno as CustID, " +
         " SoldInvoiceNo as 'Invoice no', emp as 'Return by'  from return_item where return_time " +
         "  BETWEEN  '" + dtStartdate.Text + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  Order  by return_time ";
        DataAccess.ExecuteSQL(sql1);
        DataTable dt1 = DataAccess.GetDataTable(sql1);
        dtviewReturnsTrnx.ItemsSource = dt1.DefaultView;

        string sql3 = "select SUM(Total), SUM(disamt), SUM(vatamt) from return_item " +
                     " where return_time   >='" + dtStartdate.Text + "' AND  return_time <='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);

        DataRow dr = dt1.NewRow();
        dr[0] = " ";
        dt1.Rows.Add(dr);

        DataRow dr4 = dt1.NewRow();
        dr4[0] = "Total: ";
        dr4[4] = Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString());
        dt1.Rows.Add(dr4);


        DataRow dr5 = dt1.NewRow();
        dr5[0] = "Total Discount: ";
        dr5[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString());
        dt1.Rows.Add(dr5);

        DataRow drvat = dt1.NewRow();
        drvat[0] = "Total TAX: ";
        drvat[5] = Convert.ToDouble(dt3.Rows[0].ItemArray[2].ToString());
        dt1.Rows.Add(drvat);

        DataRow drtotalreturned = dt1.NewRow();
        drtotalreturned[0] = "Total Returned: ";
        drtotalreturned[4] = (Convert.ToDouble(dt3.Rows[0].ItemArray[0].ToString()) - Convert.ToDouble(dt3.Rows[0].ItemArray[1].ToString())) + Convert.ToDouble(dt3.Rows[0].ItemArray[2].ToString());
        dt1.Rows.Add(drtotalreturned);

        DataRow dr7 = dt1.NewRow();
        dr7[0] = " ";
        dt1.Rows.Add(dr7);

        DataRow rep = dt1.NewRow();
        rep[0] = "Return Report ";
        dt1.Rows.Add(rep);

        DataRow repdt = dt1.NewRow();
        repdt[0] = "From : " + dtStartdate.Text;
        repdt[1] = "To : " + dtENDdate.Text;
        dt1.Rows.Add(repdt);


      }
      catch
      { }
    }

    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      if (dtviewReturnsTrnx.Items.Count <= 0)
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
          dtviewReturnsTrnx.Measure(pageSize);
          //  Printdlg.PageRangeSelection = PageRangeSelection.AllPages;
          Printdlg.UserPageRangeEnabled = true;
          Printdlg.PageRange = new PageRange(1, 113);
          dtviewReturnsTrnx.Arrange(new Rect(5, 0, pageSize.Width, pageSize.Height));
          //  Printdlg.ShowDialog();
          Printdlg.PrintVisual(dtviewReturnsTrnx, "Returned_Report_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
        }
      }
    }

    // // Export to Excel
    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //Exporting to xls.     
        saveFileDialog1.FileName = "Return_Report_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xls";
        saveFileDialog1.ShowDialog();


        dtviewReturnsTrnx.SelectAllCells();
        dtviewReturnsTrnx.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dtviewReturnsTrnx);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dtviewReturnsTrnx.UnselectAllCells();
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
