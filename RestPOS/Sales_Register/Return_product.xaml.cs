using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
namespace RestPOS.Sales_Register
{
  /// <summary>
  /// Interaction logic for Return_product.xaml
  /// </summary>
  public partial class Return_product : Window
  {
    ResourceManager res_man;
    CultureInfo cul;

    private const double topOffset = 180;
    private const double leftOffset = 780;
    readonly GrowlNotifiactions growlNotifications = new GrowlNotifiactions();

    DataTable t = new DataTable();
    public Return_product()
    {
      InitializeComponent();
      growlNotifications.Top = SystemParameters.WorkArea.Top + topOffset;
      growlNotifications.Left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - leftOffset;
    }

    private void Return_product_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        //Customer Info
        string sqlCust = "select   DISTINCT  *   from tbl_customer where peopletype = 'Customer'";
        DataAccess.ExecuteSQL(sqlCust);
        DataTable dtCust = DataAccess.GetDataTable(sqlCust);
        txtCustName.ItemsSource = dtCust.DefaultView;
        txtCustName.DisplayMemberPath = "name";
        txtCustName.SelectedValuePath = "id";
        txtCustName.Text = "Guest";


        CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
        ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        Thread.CurrentThread.CurrentCulture = ci;
        dtReturnDate.SelectedDate = DateTime.Today;
        //  txtbarcodeinputer.Focus();
        switch_language();
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

        lblpagetitle.Content = res_man.GetString("lblpagetitle", cul);
        lblscannerTitle.Text = res_man.GetString("lblscannerTitle", cul);
        btnSubmit.Content = res_man.GetString("btnSubmit", cul);
        lbltotalpayabletitle.Content = res_man.GetString("lbltotalpayabletitle", cul);
        lbltotaltitle.Content = res_man.GetString("lbltotaltitle", cul);
        lbldiscounttitle.Content = res_man.GetString("lbldiscounttitle", cul);
        lbltattitle.Content = res_man.GetString("lbltattitle", cul);
        lblsubtotaltitle.Content = res_man.GetString("lblsubtotaltitle", cul);
        lbloveralldiscounttitle.Text = res_man.GetString("lbloveralldiscounttitle", cul);
        lblduetitle.Text = res_man.GetString("lblduetitle", cul);
        lblChangetitle.Text = res_man.GetString("lblChangetitle", cul);
        lblpaidamtRITM.Text = res_man.GetString("lblPaidAmounttitle", cul);
        lbltrxTypetitle.Text = res_man.GetString("lbltrxTypetitle", cul);
        lblShopidtitle.Text = res_man.GetString("lblShopidtitle", cul);
        lblNotetitle.Text = res_man.GetString("lblNotetitle", cul);
        lblpaytypetitle.Text = res_man.GetString("lblpaytypetitle", cul);
        lblsalestimetitle.Text = res_man.GetString("lblsalestimetitle", cul);
        lbldatetitle.Text = res_man.GetString("lbldatetitle", cul);
        lblreturntytitle.Text = res_man.GetString("lblreturntytitle", cul);
        lblReturnAmounttitleRITM.Text = res_man.GetString("lblPaidAmounttitle1", cul);
        lblInvoiceNotitle.Text = res_man.GetString("lblInvoiceNotitle", cul);
        lblCustomertitle.Text = res_man.GetString("lblCustomertitle", cul);
        lblCommenttitle.Text = res_man.GetString("lblCommenttitle", cul);
        lblSalesbytitle.Text = res_man.GetString("lblSalesbytitle", cul);
        btnHomeMenuLink.Content = res_man.GetString("btnHomeMenuLink", cul);
        btnSaveOnly.Content = res_man.GetString("btnSave", cul);

      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }

    }

    public void overalldiscountcalculation()
    {
      double Discountvalue = Convert.ToDouble(txtDiscountRate.Text);
      txtDiscountRate.Text = Discountvalue.ToString();
      double subtotal = Convert.ToDouble(lblTotal.Text) - Convert.ToDouble(lblTotalDisCount.Text); // total - item discount  100 - 5 = 95        
      double totaldiscount = (subtotal * Discountvalue) / 100;  //Counter discount  // 95 * 5 /100 = 4.75  
      double disPlusOverallDiscount = totaldiscount + Convert.ToDouble(lblTotalDisCount.Text); // 4.75 + 5 = 9.75
      disPlusOverallDiscount = Math.Round(disPlusOverallDiscount, 2);
      lbloveralldiscount.Text = disPlusOverallDiscount.ToString();  // Overall discount 9.75

      double subtotalafteroveralldiscount = subtotal - totaldiscount; // 95 - 4.75 = 90.25
      subtotalafteroveralldiscount = Math.Round(subtotalafteroveralldiscount, 2);
      lblsubtotal.Text = subtotalafteroveralldiscount.ToString();

      //double payable = subtotalafteroveralldiscount + Convert.ToDouble(lblTotalVAT.Text);
      //payable = Math.Round(payable, 2);
      //lblTotalPayable.Text = payable.ToString();

      //txtPaidAmount.Text = lblTotalPayable.Text;
    }

    public void salePaymentinfo()
    {
      try
      {
        string sqlCmd = " Select  sales_id , change_amount , due_amount , dis, vat , sales_time , " +
                          " c_id, emp_id , comment , TrxType, ShopId , payment_type , payment_amount, ovdisrate, vaterate " +
                          "  from  sales_payment  where sales_id  = '" + txtbarcodeinputer.Text + "'";
        DataAccess.ExecuteSQL(sqlCmd);
        DataTable dt = DataAccess.GetDataTable(sqlCmd);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

          DataRow dataReader = dt.Rows[i];
          txtDiscountRate.Text = dataReader["ovdisrate"].ToString();
          txtVATRate.Text = dataReader["vaterate"].ToString();

          lblDue.Text = dataReader["due_amount"].ToString();
          lblChange.Text = dataReader["change_amount"].ToString();
          lblsalestime.Text = dataReader["sales_time"].ToString();
          lbltrxType.Text = dataReader["TrxType"].ToString();
          lblShopid.Text = dataReader["ShopId"].ToString();
          lblNote.Text = dataReader["comment"].ToString();
          //double Ovdiscount = Convert.ToDouble(dataReader["dis"].ToString()); //total discount amount
          // lbloveralldiscount.Text = Math.Round(Ovdiscount, 2).ToString();
          lblCustID.Text = dataReader["c_id"].ToString();
          lblSalesby.Text = dataReader["emp_id"].ToString();
          lblpaytype.Text = dataReader["payment_type"].ToString();
          double Paid = Convert.ToDouble(dataReader["payment_amount"].ToString()) - Convert.ToDouble(dataReader["due_amount"].ToString());
          lblPaidAmount.Text = Paid.ToString();

          txtCustName.SelectedValue = dataReader["c_id"].ToString();
          lblCustID.Text = dataReader["c_id"].ToString();

        }

      }
      catch
      {
      }
    }

    // Total VAT , and Discount Calculation
    private void total()
    {
      t = ((DataView)dtgrdviewReturnItem.ItemsSource).ToTable();

      // // subtotal without dis vat sum 
      double totalsum = 0;
      for (int i = 0; i < dtgrdviewReturnItem.Items.Count; ++i)
      {
        totalsum += Convert.ToDouble(t.Rows[i]["Total"].ToString());
        // (Convert.ToDouble((dtgrdviewReturnItem.Columns[3].GetCellContent(dtgrdviewReturnItem.Items[i]) as TextBlock).Text));                 
      }
      lblTotal.Text = totalsum.ToString();
      double total = Convert.ToDouble(totalsum.ToString());


      ////  Discount amount sum Calculation              
      double DisCount = 0.00;
      for (int i = 0; i < dtgrdviewReturnItem.Items.Count; ++i)
      {
        DisCount += Convert.ToDouble(t.Rows[i]["Disamt"].ToString());
      }
      DisCount = Math.Round(DisCount, 2);
      lblTotalDisCount.Text = DisCount.ToString();

      //// Overall sold discount / counter discount calculation
      double Discountvalue = Convert.ToDouble(txtDiscountRate.Text);
      double subtotal = Convert.ToDouble(lblTotal.Text) - DisCount;   //  total - item discount  100 - 5 = 95        
      double totaldiscount = (subtotal * Discountvalue) / 100;        //  Total Counter discount amount // 95 * 5 /100 = 4.75  

      double disPlusOverallDiscount = totaldiscount + DisCount;           // 4.75 + 5 = 9.75
      disPlusOverallDiscount = Math.Round(disPlusOverallDiscount, 2);
      lbloveralldiscount.Text = disPlusOverallDiscount.ToString();        // Overall discount 9.75

      double subtotalafteroveralldiscount = subtotal - totaldiscount;     // 95 - 4.75 = 90.25
      subtotalafteroveralldiscount = Math.Round(subtotalafteroveralldiscount, 2);
      lblsubtotal.Text = subtotalafteroveralldiscount.ToString();


      //// VAT Calculation              
      double VAT = 0.00;
      for (int i = 0; i < t.Rows.Count; ++i)
      {
        VAT += Convert.ToDouble(t.Rows[i]["Taxamt"].ToString());
      }
      VAT = Math.Round(VAT, 2);
      lblTotalVAT.Text = VAT.ToString();

      //// double Subtotal = total - DisCount;
      double sum = subtotalafteroveralldiscount + VAT;
      sum = Math.Round(sum, 2);
      lblTotalPayable.Text = sum.ToString();
      txtPaidAmount.Text = lblTotalPayable.Text;
    }

    private void btnSubmit_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        salePaymentinfo();
        double Taxrate = Convert.ToDouble(txtVATRate.Text);    // Convert.ToDouble(vatdisvalue.vat);

        // Items rows
        string sqlitems = " Select ItemName, RetailsPrice, Qty, Total , (((RetailsPrice * Qty ) * discount) / 100.00) as Disamt ,  " +
        " CASE     " +
        " WHEN taxapply = 1 THEN   ((((RetailsPrice * Qty )  - (((RetailsPrice * Qty ) * discount) / 100.00)) * " + Taxrate + " ) / 100.00 )  " +
        " ELSE 0.00  " +
        " END Taxamt, discount , taxapply as Tax , itemcode, item_id " +
        " FROM sales_item where (sales_id = '" + txtbarcodeinputer.Text + "'  and Qty != 0) ";
        //" or (sales_id = '" + txtbarcodeinputer.Text + "' and status = 3  and Qty != 0)";
        DataAccess.ExecuteSQL(sqlitems);
        DataTable dtItems = DataAccess.GetDataTable(sqlitems);
        dtgrdviewReturnItem.ItemsSource = dtItems.DefaultView;


        //Hide fields                 
        dtgrdviewReturnItem.Columns[6].Visibility = Visibility.Hidden; // Disamt        
        dtgrdviewReturnItem.Columns[7].Visibility = Visibility.Hidden; // taxamt        
        dtgrdviewReturnItem.Columns[8].Visibility = Visibility.Hidden; // Discount rate   
        dtgrdviewReturnItem.Columns[10].Visibility = Visibility.Hidden; // itemcode         
        dtgrdviewReturnItem.Columns[11].Visibility = Visibility.Hidden; // sold_item_ID    item_id  

        dtgrdviewReturnItem.Columns[0].Width = 32; // X Button
        dtgrdviewReturnItem.Columns[1].Width = 32; // X Button
        dtgrdviewReturnItem.Columns[2].Width = 220; // Item Name
        dtgrdviewReturnItem.Columns[9].Width = 40; // Tax


        //overalldiscountcalculation();
        total();
        txtInvoiceNo.Text = txtbarcodeinputer.Text;
      }
      catch
      {
        //lblCustID.Text = "10000009";
        //lblTotalReturn.Text = "0";
        //txtReturnAmount.Text = "0";
        //lbldis.Text = "0";
        //lblvat.Text = "0";
        //txtComment.Text = "0";
        //CmbPayType.Text = " ";
      }
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
      DataRowView drv = (DataRowView)dtgrdviewReturnItem.SelectedItem;
      drv.Row.Delete();
      total();
    }

    private void Minus_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        DataRowView dataRow = (DataRowView)dtgrdviewReturnItem.SelectedItem;
        int i = dtgrdviewReturnItem.CurrentCell.Column.DisplayIndex;
        for (i = 0; i < dtgrdviewReturnItem.SelectedItems.Count; i++)
        {
          if (Convert.ToDouble(dataRow.Row[2].ToString()) != 1)
          {
            double Qty = Convert.ToDouble(dataRow.Row.ItemArray[2].ToString()) - 1;
            dataRow.Row[2] = Qty;  // Qty Decrease 

            double CurrentQty = Convert.ToDouble(dataRow.Row[2]);
            double Price = Convert.ToDouble(dataRow.Row[1].ToString());
            double disrate = Convert.ToDouble(dataRow.Row[6].ToString());
            double Taxrate = Convert.ToDouble(txtVATRate.Text);

            //// show total price   Qty  * Rprice
            dataRow.Row[3] = Price * CurrentQty;   // Total Price

            if (disrate != 0)
            {
              double Disamt = (((Price * CurrentQty) * disrate) / 100.00);      // Total Discount amount of this item
              dataRow.Row[4] = Disamt;
            }

            if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
            {
              double Taxamt = ((((Price * CurrentQty) - (((Price * CurrentQty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
              dataRow.Row[5] = Taxamt;
            }

            total();
          }
        }
      }
      catch
      {

      }

    }

    private void txtCustName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        lblCustID.Text = txtCustName.SelectedValue.ToString();

      }
      catch
      {
      }
    }

    /// //// Add/insert Return items  //////////////////////
    public void Return_item()
    {
      DataTable dt = new DataTable();
      dt = ((DataView)dtgrdviewReturnItem.ItemsSource).ToTable();
      int rows = dtgrdviewReturnItem.Items.Count;
      for (int i = 0; i < rows; i++)
      {
        //ItemName, RetailsPrice, Qty, Total , item_id
        string itemName = dt.Rows[i]["ItemName"].ToString();
        double RetailsPrice = Convert.ToDouble(dt.Rows[i]["RetailsPrice"].ToString());
        double Qty = Convert.ToDouble(dt.Rows[i]["Qty"].ToString());
        double Total = Convert.ToDouble(dt.Rows[i]["Total"].ToString());
        double disamt = Math.Round(Convert.ToDouble(lbloveralldiscount.Text));
        double vatamt = Math.Round(Convert.ToDouble(dt.Rows[i]["Taxamt"].ToString()));
        string itemcode = dt.Rows[i]["itemcode"].ToString();  //itemcode - Item Barcode
        string SoldID = dt.Rows[i]["item_id"].ToString();   //Single sales item id
        string return_time = dtReturnDate.Text;
        string InvoiceNo = txtInvoiceNo.Text;
        string emp = lblSalesby.Text;


        string sql1 = " insert into return_item (item_id, itemName, Qty, RetailsPrice, Total, return_time, custno, emp, SoldInvoiceNo, Comment, disamt , vatamt) " +
                      " values ('" + itemcode + "', '" + itemName + "', '" + Qty + "', '" + RetailsPrice + "' , '" + Total + "', '" + return_time + "',   " +
                      " '" + lblCustID.Text + "', '" + emp + "' , '" + InvoiceNo + "', '" + txtcomment.Text + "', '" + disamt + "', '" + vatamt + "')";
        DataAccess.ExecuteSQL(sql1);



        // Update Quantity | Increase Quantity to Purchase table 
        string sqlupdateQty = "select product_quantity  from purchase where product_id = '" + itemcode + "'";
        DataAccess.ExecuteSQL(sqlupdateQty);
        DataTable dtUqty = DataAccess.GetDataTable(sqlupdateQty);
        double product_quantity = Convert.ToDouble(dtUqty.Rows[0].ItemArray[0].ToString()) + Qty;

        string sql = " update purchase set " +
                        " product_quantity = '" + product_quantity + "'  where product_id = '" + itemcode + "' ";
        DataAccess.ExecuteSQL(sql);

        // Decrease Sales Item into Sales_item table
        //Update sales_item Qty , status . Status 1 = Sold 2 = Sold item has been returned
        string sqlSalesQTY = " select Qty from sales_item  where item_id = '" + SoldID + "' ";
        DataAccess.ExecuteSQL(sqlSalesQTY);
        DataTable dtSalesQTY = DataAccess.GetDataTable(sqlSalesQTY);
        double SalesQTY = Convert.ToDouble(dtSalesQTY.Rows[0].ItemArray[0].ToString()) - Qty;
        double totalsale = SalesQTY * RetailsPrice;

        string sqlSIstatus = " update sales_item set " +
                              //   " Qty = (select Qty from sales_item  where item_id = '" + SoldID + "' ) - '" + Qty + "' , " +
                              //   " Total  = ((select Qty from sales_item  where item_id = '" + SoldID + "' )  - '" + Qty + "' )  * RetailsPrice   " + 
                              " Qty = '" + SalesQTY + "' , Total  = '" + totalsale + "'   " +
                              " where item_id = '" + SoldID + "' ";
        DataAccess.ExecuteSQL(sqlSIstatus);
      }


    }

    private void btnSaveOnly_Click(object sender, RoutedEventArgs e)
    {
      if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
      {
        //  MessageBox.Show("Please insert Return amount", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert Return amount", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });

      }
      else
      {
        try
        {
          if (MessageBox.Show("Do you want to Complete Return ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {
            Return_item();
            //  MessageBox.Show("Successfully Returned Items  \n   ....... ", "Successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            growlNotifications.AddNotification(new Notification { Title = "Successful ", Message = "Item has been Successfully Returned", ImageUrl = "pack://application:,,,/Notifications/notification-icon.png" });
            Sales_Register.Return_product go = new Sales_Register.Return_product();
            go.Show();
            this.Close();
          }

        }
        catch (Exception exp)
        {
          MessageBox.Show(exp.Message);
        }
      }
    }

    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      Home go = new Home();
      go.Show();

    }



  }
}
