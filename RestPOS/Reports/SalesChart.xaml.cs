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
    /// Interaction logic for SalesChart.xaml
    /// </summary>
    public partial class SalesChart : UserControl
  {
    public SalesChart()
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
                DataAccess.ExecuteSQL(sql3);
                DataTable dt3 = DataAccess.GetDataTable(sql3);

                string sql4 = " select sales_time, SUM(total) as total from sales_item " +
                                " where sales_time   between  '" + dtStartdate.Text + "'  and '" + dtENDdate.Text + "'   " +
                                "  GROUP BY  sales_time ";
                DataAccess.ExecuteSQL(sql4);
                DataTable dt4 = DataAccess.GetDataTable(sql4);

                int i = 0;
                KeyValuePair<String, Double>[] items = new KeyValuePair<String, Double>[dt3.Rows.Count];
                foreach (DataRow row in dt3.Rows)
                {
                    items[i++] = new KeyValuePair<String, Double>(row.ItemArray[0].ToString(), Double.Parse(row.ItemArray[1].ToString()));
                }
                ((AreaSeries)mcAreaChart.Series[0]).ItemsSource = items;


                
                int ii = 0;
                KeyValuePair<String, Double>[] pie_items = new KeyValuePair<String, Double>[dt4.Rows.Count];
                foreach (DataRow row in dt4.Rows)
                {
                    pie_items[ii++] = new KeyValuePair<String, Double>(row.ItemArray[0].ToString(), Double.Parse(row.ItemArray[1].ToString()));
                }
                ((ColumnSeries)mcColumnChart.Series[0]).ItemsSource = items;
                ((ColumnSeries)mcColumnChart.Series[1]).ItemsSource = pie_items;
                //dtviewtopsale.ItemsSource = dt3.DefaultView;
            }
            catch
            { }
        }


        //Las t 30 days Top sales
        private void btn30days_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dtStartdate.Text = DateTime.Today.AddDays(-30).ToString();
                dtENDdate.Text = DateTime.Today.ToShortDateString();
                dtENDdate_SelectedDateChanged(sender, (SelectionChangedEventArgs)e);

                //dtStartdate.Text = DateTime.Now.AddDays(-30).ToString();
                //dtENDdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

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

                double scale = Printdlg.PrintableAreaWidth / gridPrint.ActualWidth;

                // Size pageSize = new Size(280, Printdlg.PrintableAreaHeight);
                // System.Drawing.Printing.PaperSize pkCustomSize1 = new System.Drawing.Printing.PaperSize("First custom size", 280, 1200);
                // sizing of the element.
                gridPrint.Measure(pageSize);
                gridPrint.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                Printdlg.MaxPage = 10;

                gridPrint.LayoutTransform = new ScaleTransform(scale, scale);
                gridPrint.UpdateLayout();

                Printdlg.PrintVisual(gridPrint, "Report_SalesChart_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));

                gridPrint.LayoutTransform = originalScale;
            }
        }
    }
}
