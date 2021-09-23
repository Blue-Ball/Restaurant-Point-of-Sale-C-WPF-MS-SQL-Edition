using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace RestPOS.Sales_Register
{
  /// <summary>
  /// Interaction logic for PrintTicket.xaml
  /// </summary>
  public partial class PrintTicket : Window
  {
    ResourceManager res_man;
    CultureInfo cul;
    public PrintTicket()
    {
      InitializeComponent();
      this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        Close();
    }

    private void printticketForm_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        string sqlinfo = " select sales_id,  salesdate, ordertime, tableno, tokenno " +
                            "  from tbl_hold_sales_item  where  sales_id = '" + parameter.holdsalesid + "' ";
        DataAccess.ExecuteSQL(sqlinfo);
        DataTable dtinfo = DataAccess.GetDataTable(sqlinfo);

        lblTicketno.Text = dtinfo.Rows[0].ItemArray[0].ToString();
        lblDateTicket.Text = dtinfo.Rows[0].ItemArray[1].ToString();
        lblTimeTicket.Text = dtinfo.Rows[0].ItemArray[2].ToString();
        lblTableNo.Text = dtinfo.Rows[0].ItemArray[3].ToString();
        lblTokenno.Text = dtinfo.Rows[0].ItemArray[4].ToString();

        string sqlTikitem = "  select  '- ' + convert(nvarchar(50), qty)    + '  ' +    itemname  + '\nSc:' +  options  as 'Items' " +
                            "   from tbl_hold_sales_item   where sales_id = '" + parameter.holdsalesid + "' ";
        DataAccess.ExecuteSQL(sqlTikitem);
        DataTable dtTikitem = DataAccess.GetDataTable(sqlTikitem);
        dtgrdticketitem.ItemsSource = dtTikitem.DefaultView;

        switch_language();
      }
      catch
      {

      }
    }

    private void btnPrintTicket_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        dtgrdticketitem.Items.Refresh();
        System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
        if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
        {
          Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
          // Size pageSize = new Size(340, 194);
          // sizing of the element.
          grdTicketPrintPanel.Measure(pageSize);
          grdTicketPrintPanel.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
          Printdlg.MaxPage = 10;
          Printdlg.PrintVisual(grdTicketPrintPanel, "Ticket_Print_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
        }
      }
      catch
      {

      }
    }

    private void switch_language()
    {
      res_man = new ResourceManager("RestPOS.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);
        lblticketPNTtitle.Text = res_man.GetString("lblticketPNTtitle", cul);
        lbltimePNTtitle.Text = res_man.GetString("lbltimePNTtitle", cul);
        lbltablePNTtitle.Text = res_man.GetString("lbltablePNTtitle", cul);
        lblinvoiceNoPNTtitle.Text = res_man.GetString("lblinvoiceNoPNTtitle", cul);

        lbldatePNTtitle.Text = res_man.GetString("lbldatetitle", cul);
        lbltakennoPNTtitle.Text = res_man.GetString("lbltokonnoSRtitle", cul);
        btnPrintTicket.Content = res_man.GetString("btnPrint", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }
  }
}
