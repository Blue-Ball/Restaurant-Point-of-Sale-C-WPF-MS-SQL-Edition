using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PosCube.Reports
{
  /// <summary>
  /// Interaction logic for TopSales.xaml
  /// </summary>
  public partial class TopSales : UserControl
  {
    public TopSales()
    {
      InitializeComponent();
    }

    //Date to date
    private void dtENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string sql3 = " select  top 25 itemname  as 'Name'   , SUM(qty)   as 'QTY' from sales_item " +
                      " where sales_time   between  '" + dtStartdate.Text + "'  and '" + dtENDdate.Text + "'   " +
                      " GROUP BY    itemname order by  qty desc ";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);
        dtviewtopsale.ItemsSource = dt3.DefaultView;
      }
      catch
      { }
    }


    //Las t 30 days Top sales
    private void btn30days_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        dtStartdate.Text = DateTime.Now.AddDays(-30).ToString();
        dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

        string sql3 = " select   itemname  as 'Name'   , SUM(qty)   as 'QTY' from sales_item " +
                      " where sales_time   BETWEEN  '" + dtStartdate.Text + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "' " +
                      " GROUP BY    itemname order by  qty desc  ";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);
        dtviewtopsale.ItemsSource = dt3.DefaultView;

        //DataRow dr = dt3.NewRow();
        //dr[0] = "33";
        //dt3.Rows.Add(dr);


      }
      catch
      { }
    }

    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      if (dtviewtopsale.Items.Count <= 0)
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
          dtviewtopsale.Measure(pageSize);
          //  Printdlg.PageRangeSelection = PageRangeSelection.AllPages;
          Printdlg.UserPageRangeEnabled = true;
          Printdlg.PageRange = new PageRange(1, 113);
          dtviewtopsale.Arrange(new Rect(5, 0, pageSize.Width, pageSize.Height));
          //  Printdlg.ShowDialog();
          Printdlg.PrintVisual(dtviewtopsale, "Top_sale_Report_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
        }
      }
    }
  }
}
