using System;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RestPOS.Sales_Register
{
  /// <summary>
  /// Interaction logic for ReceiptPrint.xaml
  /// </summary>
  public partial class ReceiptPrint : Window
  {
    ResourceManager res_man;
    CultureInfo cul;
    public ReceiptPrint(string ReceiptNo)
    {
      InitializeComponent();
      lblInvoNo.Text = ReceiptNo;
    }

    public void topheader()
    {
      string sql3 = "select * from tbl_terminallocation where shopid = '" + UserInfo.Shopid + "'";
      DataAccess.ExecuteSQL(sql3);
      DataTable dt1 = DataAccess.GetDataTable(sql3);

      DateTime dt = DateTime.Now;
      lblCompanyName.Text = dt1.Rows[0].ItemArray[1].ToString();
      lblBranch.Text = dt1.Rows[0].ItemArray[2].ToString();
      lblAddress.Text = dt1.Rows[0].ItemArray[3].ToString();
      lblContact.Text = dt1.Rows[0].ItemArray[4].ToString();
      //  lblEmail.Text         = dt1.Rows[0].ItemArray[5].ToString();
      lblWebsite.Text = dt1.Rows[0].ItemArray[6].ToString();
      parameter.footermsg = dt1.Rows[0].ItemArray[11].ToString();


      byte[] img = (byte[])(dt1.Rows[0].ItemArray[12]);
      if (img == null)
      {
        piclogo.Source = null;
      }
      else
      {
        Stream StreamObj = new MemoryStream(img);
        BitmapImage BitObj = new BitmapImage();
        BitObj.BeginInit();
        BitObj.StreamSource = StreamObj;
        BitObj.EndInit();
        this.piclogo.Source = BitObj;
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

    //public void topinfoTicket()
    //{
     // string sqlinfo = "select sales_id,  sales_time, ordertime, tableno, tokenno  from sales_payment  where  sales_id = '" + lblInvoNo.Text + "' ";
    //  DataAccess.ExecuteSQL(sqlinfo);
    //  DataTable dtinfo = DataAccess.GetDataTable(sqlinfo);

      //lblTicketno.Text = dtinfo.Rows[0].ItemArray[0].ToString();
     // lblDateTicket.Text = dtinfo.Rows[0].ItemArray[1].ToString();
    //  lblTimeTicket.Text = dtinfo.Rows[0].ItemArray[2].ToString();
     // lblTableNo.Text = dtinfo.Rows[0].ItemArray[3].ToString();
      //lblTokenno.Text = dtinfo.Rows[0].ItemArray[4].ToString();

   //   string sqlTikitem = "  select  '- ' + convert(nvarchar(50), qty)    + '  ' +    itemName  + '\nSc:' +  note  as 'Items' " +
                         // "  from sales_item    where sales_id = '" + lblInvoNo.Text + "' ";
    //  DataAccess.ExecuteSQL(sqlTikitem);
    //  DataTable dtTikitem = DataAccess.GetDataTable(sqlTikitem);
    //  dtgrdticketitem.ItemsSource = dtTikitem.DefaultView;
    //}

    private void ReceiptForm_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        string currencysign = parameter.currencysign;
        topheader();
        string sql = " select   " +
                        " CASE     " +
                        " WHEN taxapply = 1 THEN  'TX ' + itemName " +
                        " ELSE   itemName  " +
                        " END 'Items'  " +
                        " , Qty,  RetailsPrice as Price , '" + currencysign + "' + convert(nvarchar(50), total)    as Total    " +
                        "   from sales_item " +
                        //  " where item_id BETWEEN '1' AND '36' "; G = 1150 DT = 950
                        " where (sales_id = '" + lblInvoNo.Text + "')";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgriditems.ItemsSource = dt1.DefaultView;

        string sql3 = "select SUM(Total)   from sales_item  where sales_id = '" + lblInvoNo.Text + "'";
        DataAccess.ExecuteSQL(sql3);
        DataTable dt3 = DataAccess.GetDataTable(sql3);

        string sql6 = "select * from sales_payment  where (sales_id = '" + lblInvoNo.Text + "')";
        DataAccess.ExecuteSQL(sql6);
        DataTable dt6 = DataAccess.GetDataTable(sql6);


        DataRow dr = dt1.NewRow();
        dr[0] = "";
        dt1.Rows.Add(dr);

        DataRow Total = dt1.NewRow();
        Total[0] = "Sub Total : ";
        Total[3] = currencysign + dt3.Rows[0].ItemArray[0].ToString();
        dt1.Rows.Add(Total);

        DataRow dis = dt1.NewRow();
        dis[0] = "Discount : " + dt6.Rows[0].ItemArray[13].ToString() + " %";
        dis[3] = currencysign + dt6.Rows[0].ItemArray[5].ToString();
        dt1.Rows.Add(dis);



        ////////////////////////  CANADIAN T.V.Q. and T.P.S.   ////////////////////////  START
        //double tpsamount = Math.Round(((Convert.ToDouble(dt6.Rows[0].ItemArray[6].ToString()) * 33.3891) / 100), 2);
        //DataRow drtps = dt1.NewRow();
        //drtps[0] = "T.P.S. ( " + (Convert.ToDouble(dt6.Rows[0].ItemArray[14].ToString()) - 9.975).ToString() + " )%"; // 5 %
        //drtps[3] = currencysign + tpsamount;
        //dt1.Rows.Add(drtps);

        //DataRow drtvq = dt1.NewRow();
        //drtvq[0] = "T.V.Q. ( " + (Convert.ToDouble(dt6.Rows[0].ItemArray[14].ToString()) - 5.00).ToString() + " )%";
        //drtvq[3] = currencysign + Math.Round(((Convert.ToDouble(dt6.Rows[0].ItemArray[6].ToString()) * 66.6109) / 100), 2);
        //dt1.Rows.Add(drtvq);
        ////////////////////////  CANADIAN T.V.Q. and T.P.S.   ////////////////////////  END

        ///// ////////////////////////  Indian CGST and SGST   ////////////////////////  START
        //double CGSTamount = Math.Round(((Convert.ToDouble(dt6.Rows[0].ItemArray[6].ToString()) * 50) / 100), 2);
        //DataRow drtps = dt1.NewRow();
        //drtps[0] = "CGST. ( " + (Convert.ToDouble(dt6.Rows[0].ItemArray[14].ToString()) / 2).ToString() + " )%";
        //drtps[3] = currencysign + CGSTamount;
        //dt1.Rows.Add(drtps);

        //DataRow drtvq = dt1.NewRow();
        //drtvq[0] = "SGST. ( " + (Convert.ToDouble(dt6.Rows[0].ItemArray[14].ToString()) / 2).ToString() + " )%";
        //drtvq[3] = currencysign + Math.Round(((Convert.ToDouble(dt6.Rows[0].ItemArray[6].ToString()) * 50) / 100), 2);
        //dt1.Rows.Add(drtvq);
        ///// ////////////////////////  Indian CGST and SGST   ////////////////////////  END


        ////////////////////////  General TAX =  ONE TAX  ////////////////////////  START
        DataRow dr0 = dt1.NewRow();
        dr0[0] = "TAX : " + dt6.Rows[0].ItemArray[14].ToString() + " %";
        dr0[3] = currencysign + dt6.Rows[0].ItemArray[6].ToString();
        dt1.Rows.Add(dr0);
        ////////////////////////  General TAX =  ONE TAX  ////////////////////////  END


        DataRow dr2 = dt1.NewRow();
        dr2[0] = "Net Amount :  ";
        dr2[3] = currencysign + dt6.Rows[0].ItemArray[2].ToString();
        dt1.Rows.Add(dr2);

        DataRow drLine = dt1.NewRow();
        drLine[0] = "";
        dt1.Rows.Add(drLine);

        //DataRow dr3 = dt1.NewRow();
        //dr3[0] = dt6.Rows[0].ItemArray[1].ToString() + " Pay";              
        //dt1.Rows.Add(dr3);
        ///// =((Sum(Fields!Total.Value) - Fields!dis.Value) + Fields!vat.Value) +  Fields!charAmt.Value - Fields!due.Value
        double paidamount = (Convert.ToDouble(dt6.Rows[0].ItemArray[2].ToString()) + Convert.ToDouble(dt6.Rows[0].ItemArray[3].ToString())) - Convert.ToDouble(dt6.Rows[0].ItemArray[4].ToString());
        DataRow dr4 = dt1.NewRow();
        dr4[0] = dt6.Rows[0].ItemArray[1].ToString();
        dr4[3] = currencysign + paidamount.ToString();
        dt1.Rows.Add(dr4);

        DataRow dr5 = dt1.NewRow();
        dr5[0] = "Change Amount : ";
        dr5[3] = currencysign + dt6.Rows[0].ItemArray[3].ToString();
        dt1.Rows.Add(dr5);

        DataRow due = dt1.NewRow();
        due[0] = "Due Amount : ";
        due[3] = currencysign + dt6.Rows[0].ItemArray[4].ToString();
        dt1.Rows.Add(due);

        DataRow dr6 = dt1.NewRow();
        dr6[0] = "";
        dt1.Rows.Add(dr6);

        //DataRow dr7 = dt1.NewRow();
        //dr7[0] = "*TX: TAX Apply";
        //dt1.Rows.Add(dr7);

        DataRow dr8 = dt1.NewRow();
        dr8[0] = parameter.footermsg + "\n DynamicSoft";
        dt1.Rows.Add(dr8);

        //DataRow dr9 = dt1.NewRow();
        //dr9[0] = "||| || |||| ||||||";
        //dt1.Rows.Add(dr9);

        //DataRow emp = dt1.NewRow();
        //emp[0] = "Served by: " + dt6.Rows[0].ItemArray[9].ToString();
        //dt1.Rows.Add(emp);

        //DataRow dr8 = dt1.NewRow();
        //dr8[0] = "Recipt No : " + dt6.Rows[0].ItemArray[0].ToString();
        //dt1.Rows.Add(dr8);

        //DataRow credit = dt1.NewRow();
        //credit[0] = "citkar@live.com";
        //dt1.Rows.Add(credit);

        //lblInvoNo.Text = dt6.Rows[0].ItemArray[0].ToString();
       // lbltoknNo.Text = dt6.Rows[0].ItemArray[17].ToString();
        lblservedby.Text = dt6.Rows[0].ItemArray[9].ToString();
        lblPrintDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt");

        //topinfoTicket();
        switch_language();
      }
      catch
      {
      }
            this.Visibility = Visibility.Hidden;
            Autoprint();
            btnClose_Click(null, null);
        }

    public void Autoprint()
    {
      dtgriditems.Items.Refresh();
        PrinterSettings settings = new PrinterSettings();

        System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
        var printers = new LocalPrintServer().GetPrintQueues();
        var selectedPrinter = printers.FirstOrDefault(p => p.Name == settings.PrinterName); //Replacement 
        //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
        if (selectedPrinter != null)
        {
            Printdlg.PrintQueue = selectedPrinter;

            // Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
            Size pageSize = new Size(280, Printdlg.PrintableAreaHeight);
        System.Drawing.Printing.PaperSize pkCustomSize1 = new System.Drawing.Printing.PaperSize("First custom size", 280, 1200);
        // sizing of the element.
        grdPrintPanel.Measure(pageSize);
        grdPrintPanel.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
        Printdlg.MaxPage = 10;
        grdPrintPanel.UpdateLayout();
        Printdlg.PrintVisual(grdPrintPanel, "POS_Print_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
      }
    }

    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      Autoprint();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      //// if 1 from SR it's back to SR
      if (parameter.autoprint == "1")
      {
        //this.Visibility = Visibility.Hidden;
        //Sales_Register.SalesRegister go = new Sales_Register.SalesRegister();
        //go.Show();
        this.Visibility = Visibility.Hidden;
        if (UserInfo.DefaulfSRWindow == "2")
        {
          Sales_Register.SalesRegisterPink go = new Sales_Register.SalesRegisterPink();
          go.Show();
        }
        else
        {
          this.Visibility = Visibility.Hidden;

          Sales_Register.SalesRegisterPink go = new Sales_Register.SalesRegisterPink();
          go.Show();
        }
      }
      else // if open from Report page it's close 
      {
        this.Close();
      }

    }

    //private void btnPrintTicket_Click(object sender, RoutedEventArgs e)
   // {
     // try
     // {
       // dtgrdticketitem.Items.Refresh();
        //System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
       // if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
       // {
         // Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
          // Size pageSize = new Size(340, 194);
          // sizing of the element.
         // grdTicketPrintPanel.Measure(pageSize);
          //grdTicketPrintPanel.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
         // Printdlg.MaxPage = 10;
         // Printdlg.PrintVisual(grdTicketPrintPanel, "Ticket_Print_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
       // }
      //}
     // catch
     // {

    //  }
    //}

   // private void btnViewInvoice_Click(object sender, RoutedEventArgs e)
   // {
     // UserInfo.invoiceNo = lblInvoNo.Text;
     // Sales_Register.InvoiceView go = new Sales_Register.InvoiceView();
      //go.ShowDialog();
    //}

    private void switch_language()
    {
      res_man = new ResourceManager("RestPOS.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);
        btnPrint.Content = res_man.GetString("btnPrint", cul);
        lbltokenPNTtitle.Text = res_man.GetString("lbltokenPNTtitle", cul);
        lblinvoicePNTtile.Text = res_man.GetString("lblinvoicePNTtile", cul);
        lblsvrPNTtitle.Text = res_man.GetString("lblsvrPNTtitle", cul);
       // lblticketPNTtitle.Text = res_man.GetString("lblticketPNTtitle", cul);
       // lbltimePNTtitle.Text = res_man.GetString("lbltimePNTtitle", cul);
        //lbltablePNTtitle.Text = res_man.GetString("lbltablePNTtitle", cul);
       // lblinvoiceNoPNTtitle.Text = res_man.GetString("lblinvoiceNoPNTtitle", cul);

        //lbldatePNTtitle.Text = res_man.GetString("lbldatetitle", cul);
        //lbltakennoPNTtitle.Text = res_man.GetString("lbltokonnoSRtitle", cul);
       // btnPrintTicket.Content = res_man.GetString("btnPrint", cul);
       // btnClose.Content = res_man.GetString("btnHomeMenuLink", cul);
        //lbltakennoPNTtitle.Text = res_man.GetString("lbltokonnoSRtitle", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }
  }
}
