using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace PosCube.Reports
{
  /// <summary>
  /// Interaction logic for Overview.xaml
  /// </summary>
  public partial class Overview : UserControl
  {
    public Overview()
    {
      InitializeComponent();
            dtStartdate.Text = DateTime.Today.AddMonths(-1).ToShortDateString();
            dtENDdate.Text = DateTime.Today.ToShortDateString();
            dtENDdate_SelectedDateChanged(null, null);
    }

        //Date to date
        private void dtENDdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string sql3 = " select sales_time, SUM(profit * Qty) as Profit from sales_item " +
                                " where sales_time   between  '" + dtStartdate.Text + "'  and '" + dtENDdate.Text + "'   " +
                                "  GROUP BY  sales_time ";
                //string sql3 = " select  top 25 itemname  as 'Name'   , SUM(qty)   as 'QTY' from sales_item " +
                //              " where sales_time   between  '" + dtStartdate.Text + "'  and '" + dtENDdate.Text + "'   " +
                //              " GROUP BY    itemname order by  qty desc ";
                DataAccess.ExecuteSQL(sql3);
                DataTable dt3 = DataAccess.GetDataTable(sql3);
                // ((ColumnSeries)mcColumnChart.Series[0]).ItemsSource = dt3.DefaultView;

                int i = 0;
                KeyValuePair<String, Double>[] items = new KeyValuePair<String, Double>[dt3.Rows.Count];
                foreach (DataRow row in dt3.Rows)
                {
                    items[i++] = new KeyValuePair<String, Double>(row.ItemArray[0].ToString(), Double.Parse( row.ItemArray[1].ToString()));
                }
                ((ColumnSeries)mcColumnChart.Series[0]).ItemsSource = items;
                ((PieSeries)mcPieChart1.Series[0]).ItemsSource = items;

                string sql4 = " select sales_time, SUM(total) as total from sales_item " +
                                " where sales_time   between  '" + dtStartdate.Text + "'  and '" + dtENDdate.Text + "'   " +
                                "  GROUP BY  sales_time ";
                DataAccess.ExecuteSQL(sql4);
                DataTable dt4 = DataAccess.GetDataTable(sql4);
                // ((ColumnSeries)mcColumnChart.Series[0]).ItemsSource = dt3.DefaultView;

                int ii = 0;
                KeyValuePair<String, Double>[] pie_items = new KeyValuePair<String, Double>[dt4.Rows.Count];
                foreach (DataRow row in dt4.Rows)
                {
                    pie_items[ii++] = new KeyValuePair<String, Double>(row.ItemArray[0].ToString(), Double.Parse(row.ItemArray[1].ToString()));
                }
                ((PieSeries)mcPieChart2.Series[0]).ItemsSource = pie_items;

                //new KeyValuePair<string, int>[]{
                //    new KeyValuePair<string, int>("Project Manager", 12),
                //    new KeyValuePair<string, int>("CEO", 25),
                //    new KeyValuePair<string, int>("Software Engg.", 5),
                //    new KeyValuePair<string, int>("Team Leader", 6),
                //    new KeyValuePair<string, int>("Project Leader", 10),
                //    new KeyValuePair<string, int>("Developer", 4) };
                //LoadColumnChartData(items);
            }
            catch (Exception eee)
            {
                System.Diagnostics.Debug.WriteLine(eee.Message);
            }
        }
        //private void LoadColumnChartData(KeyValuePair<string, int>[] items)
        //{
        //    ((ColumnSeries)mcColumnChart.Series[0]).ItemsSource = items;
        //}

        //Las t 30 days Top sales
        private void btn30days_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dtStartdate.Text = DateTime.Today.AddDays(-30).ToString();
                dtENDdate.Text = DateTime.Today.ToShortDateString();
                dtENDdate_SelectedDateChanged(sender, (SelectionChangedEventArgs)e);
                //string sql3 = " select   itemname  as 'Name'   , SUM(qty)   as 'QTY' from sales_item " +
                //              " where sales_time   BETWEEN  '" + dtStartdate.Text + "' AND    '" + DateTime.Now.ToString("yyyy-MM-dd") + "' " +
                //              " GROUP BY    itemname order by  qty desc  ";
                //DataAccess.ExecuteSQL(sql3);
                //DataTable dt3 = DataAccess.GetDataTable(sql3);
                //dtviewtopsale.ItemsSource = dt3.DefaultView;

                //DataRow dr = dt3.NewRow();
                //dr[0] = "33";
                //dt3.Rows.Add(dr);


            }
            catch
            { }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrinterSettings settings = new PrinterSettings();

            System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            var printers = new LocalPrintServer().GetPrintQueues();
            var selectedPrinter = printers.FirstOrDefault(p => p.Name == settings.PrinterName); //Replacement 
            //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            if (selectedPrinter != null)
            {
                Transform originalScale = gridPrint.LayoutTransform;

                Printdlg.PrintQueue = selectedPrinter;

                Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);

                double scale = Math.Min(Printdlg.PrintableAreaWidth / gridPrint.ActualWidth, Printdlg.PrintableAreaHeight /
                           gridPrint.ActualHeight);

                gridPrint.LayoutTransform = new ScaleTransform(scale, scale);

                // Size pageSize = new Size(280, Printdlg.PrintableAreaHeight);
                // System.Drawing.Printing.PaperSize pkCustomSize1 = new System.Drawing.Printing.PaperSize("First custom size", 280, 1200);
                // sizing of the element.
                gridPrint.Measure(pageSize);
                gridPrint.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                Printdlg.MaxPage = 10;
                gridPrint.UpdateLayout();
                Printdlg.PrintVisual(gridPrint, "Report_OverView_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));

                gridPrint.LayoutTransform = originalScale;
            }
        }
    }
}
