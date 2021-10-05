using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Windows;

namespace PosCube.Sales_Register
{
  /// <summary>
  /// Interaction logic for InvoiceView.xaml
  /// </summary>
  public partial class InvoiceView : Window
  {
    public InvoiceView()
    {
      InitializeComponent();
      this.winformhostINVO.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
      this.winformhostINVO.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
      this.winformhostTK.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
      this.winformhostRPT.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
      this.winformhostINVORPT.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
    }

    private void invoiceviewFormLoad(object sender, RoutedEventArgs e)
    {
      try
      {
        string sql = " select  *  " +
                        " from sales_item si  " +
                        " INNER JOIN   sales_payment sp  " +
                        " ON sp.sales_id  = si.sales_id    " +
                        " INNER JOIN tbl_terminallocation tl    " +
                        " ON sp.shopid  = tl.shopid    " +
                        " INNER JOIN tbl_customer c    " +
                        " ON  sp.c_id  = c.ID " +
                        " where si.sales_id =  '" + UserInfo.invoiceNo + "' ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt = DataAccess.GetDataTable(sql);

        ReportDataSource reportDSDetail = new ReportDataSource("DataSetInvo", dt);
        this.reportViewer1.LocalReport.DataSources.Clear();
        //   this.reportViewer1.LocalReport.ReportPath = "E:\\Data_laptop_1\\D_drive\\G_Drive\\Files_1\\dotproject\\Web\\Codecany_project\\PosCube_MS_SQL\\RestoPOS_Source_code\\PosCube\\Sales_Register\\InvoiceView.rdlc";
        this.reportViewer1.LocalReport.DataSources.Add(reportDSDetail);

        this.reportViewer1.LocalReport.Refresh();
        this.reportViewer1.LocalReport.ReportEmbeddedResource = "PosCube.Sales_Register.InvoiceView.rdlc";
        this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
        this.reportViewer1.ZoomMode = ZoomMode.PageWidth;
        //this.reportViewer1.ZoomPercent = 80;
        this.reportViewer1.LocalReport.DisplayName = "Invoice_" + UserInfo.invoiceNo + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        this.reportViewer1.RefreshReport();

        // //// Invoice Receipt View format
        ReportDataSource reportDSDetailINVORPT = new ReportDataSource("DataSetReceipt", dt);
        this.reportViewerINVORPT.LocalReport.DataSources.Clear();
        this.reportViewerINVORPT.LocalReport.DataSources.Add(reportDSDetailINVORPT);
        this.reportViewerINVORPT.LocalReport.Refresh();
        this.reportViewerINVORPT.LocalReport.ReportEmbeddedResource = "PosCube.Sales_Register.InvoiceReceipt.rdlc";
        this.reportViewerINVORPT.SetDisplayMode(DisplayMode.PrintLayout);
        this.reportViewerINVORPT.ZoomMode = ZoomMode.PageWidth;
        this.reportViewerINVORPT.LocalReport.DisplayName = "Invoice_Receipt_View_" + UserInfo.invoiceNo + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        this.reportViewerINVORPT.RefreshReport();

        // /// Ticket
        ReportDataSource reportDSDetailTK = new ReportDataSource("DataSetTicket", dt);
        this.reportViewerTicket.LocalReport.DataSources.Clear();
        this.reportViewerTicket.LocalReport.DataSources.Add(reportDSDetailTK);
        this.reportViewerTicket.LocalReport.Refresh();
        this.reportViewerTicket.LocalReport.ReportEmbeddedResource = "PosCube.Sales_Register.TicketRpt.rdlc";
        this.reportViewerTicket.SetDisplayMode(DisplayMode.PrintLayout);
        this.reportViewerTicket.ZoomMode = ZoomMode.PageWidth;
        this.reportViewerTicket.LocalReport.DisplayName = "Ticket_" + UserInfo.invoiceNo + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        this.reportViewerTicket.RefreshReport();

        // //// Receipt print
        ReportDataSource reportDSDetailRPT = new ReportDataSource("DataSetReceipt", dt);
        this.reportViewerRPT.LocalReport.DataSources.Clear();
        this.reportViewerRPT.LocalReport.DataSources.Add(reportDSDetailRPT);
        this.reportViewerRPT.LocalReport.Refresh();
        this.reportViewerRPT.LocalReport.ReportEmbeddedResource = "PosCube.Sales_Register.ReceiptRpt.rdlc";
        this.reportViewerRPT.SetDisplayMode(DisplayMode.PrintLayout);
        this.reportViewerRPT.ZoomMode = ZoomMode.PageWidth;
        this.reportViewerRPT.LocalReport.DisplayName = "Receipt_" + UserInfo.invoiceNo + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        this.reportViewerRPT.RefreshReport();
      }
      catch
      {
      }
    }
  }
}
