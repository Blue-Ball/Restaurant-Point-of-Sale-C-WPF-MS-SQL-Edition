//using Activationconfig;
using IniParser;
using IniParser.Model;
using System;
using System.Data;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WPFCustomMessageBox;

namespace PosCube.Sales_Register
{
  /// <summary>
  /// Interaction logic for SalesRegisterPink.xaml
  /// </summary>
  public partial class SalesRegisterPink : Window
  {
    ResourceManager res_man;
    CultureInfo cul;

    private const double topOffset = 180;
    private const double leftOffset = 780;
    readonly GrowlNotifiactions growlNotifications = new GrowlNotifiactions();

    DataTable t = new DataTable();
    public SalesRegisterPink()
    {
      InitializeComponent();
      growlNotifications.Top = SystemParameters.WorkArea.Top + topOffset;
      growlNotifications.Left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - leftOffset;

      t.Columns.Add(new DataColumn("Items"));
      t.Columns.Add(new DataColumn("Price"));
      t.Columns.Add(new DataColumn("Qty"));
      t.Columns.Add(new DataColumn("Total"));
      t.Columns.Add(new DataColumn("ID"));
      t.Columns.Add(new DataColumn("Disamt"));
      t.Columns.Add(new DataColumn("taxamt"));
      t.Columns.Add(new DataColumn("Dis"));
      t.Columns.Add(new DataColumn("Tax"));
      t.Columns.Add(new DataColumn("KD"));
      t.Columns.Add(new DataColumn("Options"));
        t.Columns.Add(new DataColumn("Weight"));
        dgrvSalesItemList.DataContext = t.DefaultView;

      formFunctionPointer += new functioncall(Replicate); // Coin and papernotes
      currency_ShortcutsContorl.CoinandNotesFunctionPointer = formFunctionPointer;

      numformFunctionPointer += new numvaluefunctioncall(NumaricKeypad);
      currency_ShortcutsContorl.NumaricKeypad = numformFunctionPointer;
      btnholdsale.Visibility = Visibility.Hidden;
      btnDeleteholdsale.Visibility = Visibility.Hidden;
      txtbarcodescan.Focus();
    }

    //Dynamic Resulation
    public void ResulationHW()
    {
      if (System.Windows.SystemParameters.PrimaryScreenWidth == 1920)
      {
        double W = (System.Windows.SystemParameters.PrimaryScreenWidth * 15.8) / 100;
        double H = (System.Windows.SystemParameters.PrimaryScreenHeight * 16) / 100;
        Maingrid.Margin = new Thickness(W, H, 0, 0);
        Toolbargrid.Margin = new Thickness(0, 0, W, 0);
      }
      else
      {
        double W = (System.Windows.SystemParameters.PrimaryScreenWidth * 1) / 100;
        double H = (System.Windows.SystemParameters.PrimaryScreenHeight * 2) / 100;
        Maingrid.Margin = new Thickness(W, H, 0, 0);
        Toolbargrid.Margin = new Thickness(0, 8, 0, 0);
      }
    }

    // Options Data bind
    public void sauceoptionsDatabind()
    {
      string sqlsauceoptions = "select   DISTINCT  saucename, bgcolor  from tbl_sauceoptions where status = 1";
      DataAccess.ExecuteSQL(sqlsauceoptions);
      DataTable dtsauceoptions = DataAccess.GetDataTable(sqlsauceoptions);
      lstvwSauceoptions.ItemsSource = dtsauceoptions.DefaultView;
    }

    public void ReLayoutPanels()
    {
            if(dgrvSalesItemList.Items.Count > 0)
            {
                dgrvSalesItemList.Margin = new Thickness(70, dgrvSalesItemList.Margin.Top, dgrvSalesItemList.Margin.Right, dgrvSalesItemList.Margin.Bottom);
                
                gridOperation.Visibility = Visibility.Visible;

                int n = dgrvSalesItemList.SelectedIndex;
                if (n < 0)
                {
                    txtbarcodescan.Focus();
                    return;
                }
                else
                    {
                        try
                        {
                            DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;
                            if(dataRow.Row[11].ToString().Length > 0)
                            {
                                string numericValue = new String(dataRow.Row[11].ToString().Where(Char.IsDigit).ToArray());
                                double weight = double.Parse(numericValue);
                                if (weight > 0)
                                {
                                    btnItemWeight.IsEnabled = true;
                                }
                                else
                                {
                                    btnItemWeight.IsEnabled = false;
                                }
                            }
                            else
                            {
                                btnItemWeight.IsEnabled = false;
                            }
                        }
                        catch
                        {

                        }
                    }
            }
            else
            {
                dgrvSalesItemList.Margin = new Thickness(0, dgrvSalesItemList.Margin.Top, dgrvSalesItemList.Margin.Right, dgrvSalesItemList.Margin.Bottom);
                gridOperation.Visibility = Visibility.Hidden;
            }
            txtbarcodescan.Focus();
        }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        tabPayment.Visibility = Visibility.Hidden;

        // //VAT Value Get from logged user Terminal 
        txtVATRate.Text = vatdisvalue.vat; ////////////////change it////////////////////////////////////////////////////////////

        //Categoy Bind
        string sqlcategory = "select   DISTINCT  category   from purchase where product_quantity >= 1";
        DataAccess.ExecuteSQL(sqlcategory);
        DataTable dtCategory = DataAccess.GetDataTable(sqlcategory);
        lstvwCategory.ItemsSource = dtCategory.DefaultView;

        itemslist_withImage("");
        // itemlist("");

        dgrvSalesItemList.Columns[0].Width = 185;
        dgrvSalesItemList.Columns[1].Width = 65;
        dgrvSalesItemList.Columns[2].Width = 60; //dgrvSalesItemList.ActualWidth - 100 - 100 - 8.5;
        dgrvSalesItemList.Columns[3].Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                ////Hide fields
                dgrvSalesItemList.Columns[4].Visibility = Visibility.Hidden; // ID              
        dgrvSalesItemList.Columns[5].Visibility = Visibility.Hidden; // Disamt          
        dgrvSalesItemList.Columns[6].Visibility = Visibility.Hidden;   // taxamt         
        dgrvSalesItemList.Columns[7].Visibility = Visibility.Hidden;   // Discount rate   
        dgrvSalesItemList.Columns[8].Visibility = Visibility.Hidden;   // Tax
        dgrvSalesItemList.Columns[9].Visibility = Visibility.Hidden;   // KD
        dgrvSalesItemList.Columns[10].Visibility = Visibility.Hidden;   // Option  
        dgrvSalesItemList.Columns[11].Visibility = Visibility.Hidden;   // Weight


        t.Columns[0].ReadOnly = true;
        t.Columns[1].ReadOnly = false;
        t.Columns[2].ReadOnly = false;
        t.Columns[3].ReadOnly = false;
        t.Columns[4].ReadOnly = true;
        t.Columns[5].ReadOnly = false;
        t.Columns[6].ReadOnly = false;
        t.Columns[7].ReadOnly = true;
        t.Columns[8].ReadOnly = true;
        t.Columns[9].ReadOnly = true;
        //  t.Columns[10].ReadOnly = false;



        System.Windows.Threading.DispatcherTimer invoiceautoupdate = new System.Windows.Threading.DispatcherTimer();
        invoiceautoupdate.Tick += new EventHandler(invoiceautoupdate_Tick);
        invoiceautoupdate.Interval = new TimeSpan(0, 0, 0);
        invoiceautoupdate.Start();

        System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture(System.Globalization.CultureInfo.CurrentCulture.Name);
        ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        System.Threading.Thread.CurrentThread.CurrentCulture = ci;
        dtSalesDate.SelectedDate = DateTime.Today;

        tokennumberget();
        HolditemDatabind();
        ResulationHW();
        switch_language();
        txtbarcodescan.Focus();
        ReLayoutPanels();

                DisplayToPole("Welcome", "");
      }
      catch
      {
      }

    }

    public void itemslist_withImage(string category)
    {
      //var uriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\ITEMIMAGE\\", UriKind.RelativeOrAbsolute);
      string sqlCmd = " Select top 15 product_id, product_name, product_quantity, retail_price,  imagename as  imagename " +
                      " from  purchase where " +
                      " (product_name like '" + category + "%'  and product_quantity >= 1) OR " +
                      " (product_id like '" + category + "%'  and product_quantity >= 1) OR " +
                      " (category = '" + category + "' and   product_quantity >= 1) " +
                      // " ORDER BY RANDOM() LIMIT 15 ";  // RANDOM()  Sqlite
                      // " ORDER BY RAND() LIMIT 15 "; // MySQL
                      " ORDER BY NEWID() "; // SQL server and use -- top 15 after select 
      DataAccess.ExecuteSQL(sqlCmd);
      DataTable dtitems = DataAccess.GetDataTable(sqlCmd);
      lstvwStocklist.ItemsSource = dtitems.DefaultView;
      tabSauceOptions.Visibility = Visibility.Hidden;
    }

    public void addtocartitem(string product_id, double itemqty, string options)
    {
        // Default tax rate 
        double Taxrate = Convert.ToDouble(vatdisvalue.vat);


        string sql = "SELECT  product_name as Name , retail_price as Price ,  " + itemqty + "  as QTY, retail_price *  " + itemqty + " as 'Total' ,  " +
                " (((retail_price * " + itemqty + " ) * discount) / 100.00) as 'dis amt' , " +
                " CASE     " +
                " WHEN taxapply = 1 THEN   (((retail_price * " + itemqty + " )  - (((retail_price * " + itemqty + " ) * discount) / 100.00))  * " + Taxrate + " ) / 100.00   " +
                " ELSE '0.00'  " +
                " END 'taxamt' , product_id as ID , discount , taxapply, status, weight " +
                " FROM  purchase  where product_id = '" + product_id + "'  and product_quantity > 0 ";
            DataAccess.ExecuteSQL(sql);
        DataTable dt = DataAccess.GetDataTable(sql);

        string ItemsName = dt.Rows[0].ItemArray[0].ToString();
        double Rprice = Convert.ToDouble(dt.Rows[0].ItemArray[1].ToString());
        double Qty = Convert.ToDouble(dt.Rows[0].ItemArray[2].ToString());
        double Total = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString());
        string Itemid = dt.Rows[0].ItemArray[6].ToString();
        double Disamt = Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString());      //  Total Discount amount of this item
        double Taxamt = Convert.ToDouble(dt.Rows[0].ItemArray[5].ToString());     //  Total Tax amount  of this item
        double Dis = Convert.ToDouble(dt.Rows[0].ItemArray[7].ToString());       //  Discount Rate
        double Taxapply = Convert.ToDouble(dt.Rows[0].ItemArray[8].ToString());       //  VAT/TAX/TPS/TVQ apply or not
        int kitchendisplay = Convert.ToInt32(dt.Rows[0].ItemArray[9].ToString());        //  kitchen display 3= show 1= not display in kitchen 
            string Weight = dt.Rows[0].ItemArray[10].ToString();

        double i = itemqty;
        int n = Finditem(ItemsName);
        if (n == -1)
        {
            t.Rows.Add(ItemsName, Rprice, i, Total, Itemid, Disamt, Taxamt, Dis, Taxapply, kitchendisplay, options, Weight);
            n = t.Rows.Count-1;
        }
        else
        {
            Rprice = Convert.ToDouble(t.Rows[n][1]);

            int QtyInc = Convert.ToInt32(t.Rows[n][2]);
            t.Rows[n][2] = (QtyInc + 1);  //Qty Increase
            t.Rows[n][3] = Rprice * (QtyInc + 1);   // Total price
            //  dgrvSalesItemList.Rows[n].Cells[4].Value = Itemid;                     

            double qty = Convert.ToDouble(t.Rows[n][2]);
            double disrate = Convert.ToDouble(t.Rows[n][7]);

            if (disrate != 0)  // if discount has
            {
                double DisamtInc = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                t.Rows[n][5] = DisamtInc; // Discount total amount
            }

            if (Taxapply != 0)   // If apply  tax 
            {
                // Total Tax amount  of this item  (Rprice - disamount) * taxRate / 100
                double TaxamtInc = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00);
                t.Rows[n][6] = TaxamtInc; // Total Tax amount
            }
        }

        dgrvSalesItemList.SelectedIndex = n;

        txtbarcodescan.Text = "";
        txtbarcodescan.Focus();
        DiscountCalculation();
        vatcal();
        lblmsg.Visibility = Visibility.Visible;
        btnholdsale.Visibility = Visibility.Visible;

        DisplayToPole(ConvertDoubleFormat(dt.Rows[0].ItemArray[1].ToString()), "Total:"+ ConvertDoubleFormat(lblTotalPayable.Text));
    }

    private void txtbarcodescan_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (txtbarcodescan.Text == "")
      {
      }
      else
      {
        try
        {
          addtocartitem(txtbarcodescan.Text, 1.00, "");
          
        }
        catch
        {
        }
      }

    }
    // Check duplicate item 
    public int Finditem(string item)
    {
      int k = -1;
      if (t.Rows.Count > 0)
      {
        foreach (DataRow row in t.Rows)
        {
          if (row[0].Equals(item))
          {
            k = t.Rows.IndexOf(row);
            break;
          }
        }
      }
      return k;

    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        itemslist_withImage(txtSearch.Text);
      }
      catch
      {
      }
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
      DataRowView drv = (DataRowView)dgrvSalesItemList.SelectedItem;
      drv.Row.Delete();
      DiscountCalculation();
      vatcal();
      txtbarcodescan.Focus();
    }

    public void DiscountCalculation()
    {
      // // subtotal without dis vat sum 
      double totalsum = 0.00;
      for (int i = 0; i < dgrvSalesItemList.Items.Count; ++i)
      {
        totalsum += Convert.ToDouble(t.Rows[i][3].ToString());
      }
      lblTotal.Text = Math.Round(totalsum, 2).ToString();
      lblTotalItems.Text = dgrvSalesItemList.Items.Count.ToString();

      ////    Discount amount sum
      double total = Convert.ToDouble(totalsum.ToString());
      double DisCount = 0.00;
      for (int i = 0; i < dgrvSalesItemList.Items.Count; ++i)
      {
        DisCount += Convert.ToDouble(t.Rows[i][5].ToString());
      }

      DisCount = Math.Round(DisCount, 2);
      double sum = total - DisCount;
      sum = Math.Round(sum, 2);
      lblsubtotal.Text = sum.ToString();

      double payable = sum + Convert.ToDouble(lblTotalVAT.Text);
      payable = Math.Round(payable, 2);
      lblTotalPayable.Text = payable.ToString();
      lblTotalPayableTabpayment.Text = payable.ToString();
    txtTotalAmount.Text = ConvertDoubleFormat(payable.ToString());
            btnCurrentAmount.Content = ConvertDoubleFormat(payable.ToString());

      tabterminal.Header = "Terminal (" + dgrvSalesItemList.Items.Count.ToString() + ")";
      // tabPayment.Header = "Payment (" + lblTotalPayable.Text.ToString() + ")";

      lblTotalDisCount.Text = DisCount.ToString();

    }

    //VAT amount sum calculation -  
    public void vatcal()
    {
      //Subtotal = total - (discount + Globaldiscount)
      double Subtotal = Convert.ToDouble(lblsubtotal.Text);
      //double Subtotal = Convert.ToDouble(lbloveralldiscount.Text)  ;

      //VAT amount
      double VAT = 0.00;
      for (int i = 0; i < dgrvSalesItemList.Items.Count; ++i)
      {
        VAT += Convert.ToDouble(t.Rows[i][6].ToString());
      }

      VAT = Math.Round(VAT, 2);
      lblTotalVAT.Text = VAT.ToString();

      double payable = Subtotal + VAT;
      payable = Math.Round(payable, 2);
      lblTotalPayable.Text = payable.ToString();
      lblTotalPayableTabpayment.Text = payable.ToString();
            txtTotalAmount.Text = ConvertDoubleFormat(payable.ToString());
      btnCurrentAmount.Content = ConvertDoubleFormat(payable.ToString());

        ///////Pole shows Price value  | if you have pole device please UnComment   below code
        //System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort();
        //sp.PortName = "COM1";  ////Insert your pole Device Port Name E.g. COM4  -- you can find  from pole device manual  
        //sp.BaudRate = 9600;     // Pole Bound Rate 
        //sp.Parity = System.IO.Ports.Parity.None;
        //sp.DataBits = 8;   // Data Bits
        //sp.StopBits = System.IO.Ports.StopBits.One;
        //sp.Open();
        //sp.WriteLine(lblTotalPayable.Text);

        //sp.Close();
        //sp.Dispose();
        //sp = null;

        ReLayoutPanels();
    }

    #region //// Sales transaction Hold Resume

    // Hold Transaction list Data bind
    public void HolditemDatabind()
    {
      string sql = "  select distinct tableno, sales_id  from tbl_hold_sales_item ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt = DataAccess.GetDataTable(sql);
      lstvwHoldList.ItemsSource = dt.DefaultView;
    }

    private void lstvwHoldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      t.Rows.Clear();
      DiscountCalculation();
      vatcal();

      string sales_id = "";
      foreach (DataRowView row in lstvwHoldList.SelectedItems)
      {
        sales_id = row.Row[1].ToString();
        parameter.holdtableno = row.Row[0].ToString();
        lblTableNo.Text = row.Row[0].ToString();
      }
      parameter.holdtransactionID = sales_id; // holdID for  delete old holdorder
                                              //parameter.holdtableno = holdtableno;
      string sqlitems = " Select * from tbl_hold_sales_item where tableno = '" + parameter.holdtableno + "' " +
                          " and   sales_id  =  '" + sales_id + "' order by  sales_id desc ";
      DataAccess.ExecuteSQL(sqlitems);
      DataTable dtItems = DataAccess.GetDataTable(sqlitems);

      int rows = dtItems.Rows.Count;
      if (rows > 0)
      {
        for (int i = 0; i < rows; i++)
        {
          string product_id = dtItems.Rows[i].ItemArray[2].ToString();
          double itemqty = Convert.ToDouble(dtItems.Rows[i].ItemArray[3].ToString());
          string options = dtItems.Rows[i].ItemArray[4].ToString();
          addtocartitem(product_id, itemqty, options);
        }
        parameter.resumesalesstatus = "1";
      }
      btnDeleteholdsale.Visibility = Visibility.Visible;
      txtbarcodescan.Focus();
    }

    /// //// Hold sales item Insert  ////////////Store into sales_item table //////////         
    public bool Hold_sales_item(string salesdate)
    {
      string sql = " select  sales_id  from tbl_hold_sales_item order by sales_id desc ";
      DataTable dtid = DataAccess.GetDataTable(sql);
      int sales_id;
      if (dtid.Rows.Count > 0)
      {
        sales_id = Convert.ToInt32(dtid.Rows[0].ItemArray[0].ToString()) + 1;
      }
      else
      {
        sales_id = 1;
      }
      parameter.holdsalesid = sales_id; // To print Ticket

      DataTable dt = new DataTable();
      dt = ((DataView)dgrvSalesItemList.ItemsSource).ToTable();
      int rows = dgrvSalesItemList.Items.Count;
      for (int i = 0; i < rows; i++)
      {
        string itemname = dt.Rows[i].ItemArray[0].ToString();
        string product_id = dt.Rows[i].ItemArray[4].ToString();
        double qty = Convert.ToDouble(dt.Rows[i][2].ToString());
        string notes = dt.Rows[i].ItemArray[10].ToString();
        string ordertime = DateTime.Now.ToString("HH:mm");
        string tableno = parameter.holdtableno; //lblTableNo.Text;
        string tokenno = txttokenno.Text;

        string sql1 = " insert into tbl_hold_sales_item (sales_id, product_id, qty, options, salesdate, tableno, " +
                      " ordertime, itemname, tokenno)" +
                      " values ('" + sales_id + "', '" + product_id + "', '" + qty + "', " +
                      " '" + notes + "', '" + salesdate + "', '" + tableno + "' , " +
                      " '" + ordertime + "', '" + itemname + "', '" + tokenno + "')";
        DataAccess.ExecuteSQL(sql1);
      }
      return true;
    }

    private void btnholdsale_Click(object sender, RoutedEventArgs e)
    {
      if (lblTotalPayable.Text == "00" || lblTotalPayable.Text == "0" || lblTotalPayable.Text == string.Empty)
      {
            // growlNotifications.AddNotification(new Notification { Title = "Alert Message1", Message = "Sorry ! You don't have enough product in Item cart    Please Add to cart", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Product not found\nDo you want to add product list?", "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                this.Visibility = Visibility.Hidden;
                Items.Stock_List go = new Items.Stock_List();
                go.SetFromSalesFlag(this, true, txtbarcodescan.Text);
                go.Show();
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                txtbarcodescan.Text = "";
                txtbarcodescan.Focus();
            }
        }
      else
      {
        try
        {
          Hold_sales_item(DateTime.Now.ToString("yyyy-MM-dd").ToString());
          //// TokennoInsert
          tokennoInsert();
          tokennumberget();
          // // Change Hold Status to normal
          if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }
          HolditemDatabind();
          parameter.resumesalesstatus = "2";

          //Sales_Register.PrintTicket go = new Sales_Register.PrintTicket();
          //go.Show();
          tabstockcontrol.SelectedItem = tabholdtransactions;
          btnholdsale.Visibility = Visibility.Hidden;
          btnDeleteholdsale.Visibility = Visibility.Hidden;
          tabPayment.Visibility = Visibility.Hidden;
          t.Rows.Clear();
          DiscountCalculation();
          vatcal();
        }
        catch
        {

        }
      }
      txtbarcodescan.Focus();

    }


    private void btnDeleteholdsale_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        resumestatuschange(parameter.holdtransactionID);
        HolditemDatabind();

        tabPayment.Visibility = Visibility.Hidden;
        t.Rows.Clear();
        DiscountCalculation();
        vatcal();
        parameter.resumesalesstatus = "2";
      }
      catch
      { }
      txtbarcodescan.Focus();

    }

    // Delete Hold item
    public void resumestatuschange(string sales_id)
    {
      string sql1 = " Delete from tbl_hold_sales_item where tableno = '" + parameter.holdtableno + "' and   sales_id  =  '" + sales_id + "'  ";
      DataAccess.ExecuteSQL(sql1);
      btnholdsale.Visibility = Visibility.Hidden;
      btnDeleteholdsale.Visibility = Visibility.Hidden;
    }

    public void updatetablebooked()
    {
      string orderdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      string sqlUpdateCmd = " update tbl_tablezone set ordertime = '" + orderdatetime + "'    where tablename = '" + lblTableNo.Text + "'";
      DataAccess.ExecuteSQL(sqlUpdateCmd);
    }

    #endregion

    ///// Direct Sales /////////////////////////////////////////////////////////////////// Direct sale
    private void btnDirectSale_Click(object sender, RoutedEventArgs e)
    {
      if (lblTotalPayable.Text == "00" || lblTotalPayable.Text == "0" || lblTotalPayable.Text == string.Empty)
      {
            growlNotifications.AddNotification(new Notification { Title = "Alert Message2", Message = "Sorry ! You don't have enough product in Item cart    Please Add to cart", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        }
      //else if (Convert.ToInt64(txtInvoice.Text) >= InvoicesManager.InvoiceNo)
      //{
      //  MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \nhttps://goo.gl/ktvmHn ", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
      //}
      else
      {
            try
            {
                ////Save payment info into sales_payment table
                payment_item(lblTotalPayable.Text, "0", "0", "Cash", DateTime.Now.ToString("yyyy-MM-dd").ToString(), "8101", "Guest");

                ///// save sales items one by one  
                sales_item(DateTime.Now.ToString("yyyy-MM-dd").ToString());

                //// TokennoInsert
                tokennoInsert();

                // // Change Hold Status to normal
                if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }

                //// Booked Table 
                updatetablebooked();

                OpenCashDrawer();
                DisplayToPole("Welcome", "");

                    string strMessage = "Print?";
                // System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show(strMessage, "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
                MessageBoxResult dialogResult = CustomMessageBox.ShowYesNo(strMessage, "PosCube", "Yes", "No");
                if (dialogResult == MessageBoxResult.Yes)
                {
                    ///// // Open Print Invoice
                    // this.Visibility = Visibility.Hidden;
                    parameter.autoprint = "1";
                    Sales_Register.ReceiptPrint go = new Sales_Register.ReceiptPrint(txtInvoice.Text);
                    go.ShowDialog();
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                }


                tabPayment.Visibility = Visibility.Hidden;
                t.Rows.Clear();
                DiscountCalculation();
                vatcal();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
      txtbarcodescan.Focus();
    }

    ///// Open Receive payment
    private void btnReceivePayment_Click(object sender, RoutedEventArgs e)
    {
      if (lblTotalPayable.Text == "00" || lblTotalPayable.Text == "0" || lblTotalPayable.Text == string.Empty)
      {
                // growlNotifications.AddNotification(new Notification { Title = "Alert Message3", Message = "Sorry ! You don't have enough product in Item cart    Please Add to cart", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
                MessageBoxResult dialogResult = CustomMessageBox.ShowYesNo("Product not found\nDo you want to add product list?", "PosCube", "Yes", "No");
                // System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Product not found\nDo you want to add product list?", "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    this.Visibility = Visibility.Hidden;
                    Items.Stock_List go = new Items.Stock_List();
                    go.SetFromSalesFlag(this, true, txtbarcodescan.Text);
                    go.Show();
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    txtbarcodescan.Text = "";
                    txtbarcodescan.Focus();
                }
            }
      else
      {
        txtPaidAmount.Focus();
        tabPayment.Visibility = Visibility.Visible;
        tabSalesRegister.SelectedItem = tabPayment;
        PeopleDatabind();
        tablezoneDatabind();
        expndtabler.IsExpanded = true;
        // DiscountCalculation();
        //vatcal();
      }
      txtbarcodescan.Focus();
    }

    //Clear shopping cart
    private void btnSuspend_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        t.Rows.Clear();
        DiscountCalculation();
        vatcal();
        txtbarcodescan.Focus();
        tabPayment.Visibility = Visibility.Hidden;
        tabstockcontrol.SelectedItem = tabGridview;
        itemslist_withImage("");
        lstvwHoldList.SelectedItems.Clear();
        btnDeleteholdsale.Visibility = Visibility.Hidden;
        btnholdsale.Visibility = Visibility.Hidden;

        DisplayToPole("Welcome", "");
      }
      catch
      {
      }
      txtbarcodescan.Focus();
    }

    #region Click Event       

    //Edit Qty Cell
    private void dgrvSalesItemList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
            return;
      try
      {

        DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;
        int i = dgrvSalesItemList.CurrentCell.Column.DisplayIndex;
        for (i = 0; i < dgrvSalesItemList.SelectedItems.Count; i++)
          //txtbarcodescan.Focus();
        {
          if (e.Column.SortMemberPath.Equals("Qty"))
          {

            //  double Qty = Convert.ToDouble(dataRow.Row[2].ToString());
            double Price = Convert.ToDouble(dataRow.Row[1].ToString());
            double disrate = Convert.ToDouble(dataRow.Row[7].ToString());
            double Taxrate = Convert.ToDouble(vatdisvalue.vat);
            Double Qty = Double.Parse(((TextBox)e.EditingElement).Text);
            txtbarcodescan.Focus();

            //// show total price   Qty  * Rprice
            double totalPrice = Qty * Price;
            dataRow.Row[3] = totalPrice;

            if (disrate != 0)
            {
              double Disamt = (((Price * Qty) * disrate) / 100.00);      // Total Discount amount of this item
              dataRow.Row[5] = Disamt;
            }

            if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
            {
              double Taxamt = ((((Price * Qty) - (((Price * Qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
              dataRow.Row[6] = Taxamt;
            }

            DiscountCalculation();
            vatcal();
          }
        }
      }
      catch
      {
                Console.WriteLine("error1");
      }
      txtbarcodescan.Focus();
    }

    //Open Sauce Options tab
    private void dgrvSalesItemList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
            return;
      //try
      //{
      //  if (bNoSelectChange) return;
      //  tabSauceOptions.Visibility = Visibility.Visible;
      //  tabstockcontrol.SelectedItem = tabSauceOptions;
      //  sauceoptionsDatabind();
      //}
      //catch
      //{

      //}
      txtbarcodescan.Focus();
    }

    public void barcodeInput()
    {
      string Barcode = "";
      foreach (DataRowView row in lstvwStocklist.SelectedItems)
      {
        Barcode = row.Row[0].ToString();
        txtbarcodescan.Text = Barcode;
      }
      
    }

    private void lstvwStocklist_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      btnDeleteholdsale.Visibility = Visibility.Hidden;
      //barcodeInput();
      
      //txtbarcodescan.Focus(); when search raised this event.
    }

    //private void lstvwStocklist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    //{
    //  barcodeInput();
    //  txtbarcodescan.Focus();
    //}

    private void lstvwStocklist_TouchEnter(object sender, TouchEventArgs e)
    {
      // barcodeInput();
      txtbarcodescan.Focus();
    }

    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      if (lblTotalPayable.Text == "00" || lblTotalPayable.Text == "0" || lblTotalPayable.Text == string.Empty)
      {
        this.Visibility = Visibility.Hidden;
        Home go = new Home();
        go.Show();
      }
      else
      {
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please complete the current Transaction or Suspend ", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
      }
      txtbarcodescan.Focus();
    }

    //Filter by Categoy
    private void lstvwCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      string categoryname = "";
      foreach (DataRowView row in lstvwCategory.SelectedItems)
      {
        categoryname = row.Row[0].ToString();
      }

      itemslist_withImage(categoryname);
      tabstockcontrol.SelectedItem = tabGridview;
      txtbarcodescan.Focus();
    }

    //// sauce Add to cart
    private void lstvwSauceoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string Sauceoptions = "";
        foreach (DataRowView row in lstvwSauceoptions.SelectedItems)
        {
          Sauceoptions = row.Row[0].ToString();
        }

        DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;
        Sauceoptions = Sauceoptions + " ";
        // Numvalue = Numvalue.TrimEnd(',');
        dataRow.Row[10] += Sauceoptions;
      }
      catch
      {
        lblmsg.Content = "Please select item first";
      }
      txtbarcodescan.Focus();
    }


    #endregion

    #region Payment tab
    //// Invoice Synchronization from Database
    public void invoiceautoupdate_Tick(object sender, EventArgs e)
    {
      try
      {
        //Invoice id auto update
        string sql = " select  sales_id  from sales_payment order by sales_id desc";
        DataTable dt = DataAccess.GetDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          double id = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) + 1;
          txtInvoice.Text = Convert.ToString(Convert.ToInt32(id));
        }
        else
        {
          double id = 1;
          txtInvoice.Text = Convert.ToString(Convert.ToInt32(id));
        }

      }
      catch
      {
                Console.WriteLine("error2");
            }
      //  ###txtbarcodescan.Focus();### absolute unnecessoary
    }

    public void tokennumberget()
    {
      string sqltk = " select  tokenno, sales_date  from tbl_tokenno order by id desc, tokenno desc ";
      DataTable dttk = DataAccess.GetDataTable(sqltk);
      if (dttk.Rows.Count > 0)
      {
        if (dttk.Rows[0].ItemArray[1].ToString() == DateTime.Now.ToString("yyyy-MM-dd"))
        {
          double idtk = Convert.ToDouble(dttk.Rows[0].ItemArray[0].ToString()) + 1;
          txttokenno.Text = Convert.ToString(Convert.ToInt32(idtk));
        }

      }
      else
      {
        double idtk = 1;
        txttokenno.Text = Convert.ToString(Convert.ToInt32(idtk));
      }
    }

    ////Customer Info
    public void PeopleDatabind()
    {
      string sqlCust = "select   DISTINCT  *   from tbl_customer where peopletype = 'Customer'";
      DataAccess.ExecuteSQL(sqlCust);
      DataTable dtCust = DataAccess.GetDataTable(sqlCust);
      txtCustName.ItemsSource = dtCust.DefaultView;
      txtCustName.DisplayMemberPath = "name";
      txtCustName.SelectedValuePath = "id";
      txtCustName.Text = "Guest";
    }

    /// //// Add sales item  ////////////Store into sales_item table //////////         
    public bool sales_item(string salesdate)
    {
      DataTable dt = new DataTable();
      dt = ((DataView)dgrvSalesItemList.ItemsSource).ToTable();
      int rows = dgrvSalesItemList.Items.Count;
      for (int i = 0; i < rows; i++)
      {
        //string SalesDate = dtSalesDate.Text;
        string trno = txtInvoice.Text;
        string itemid = dt.Rows[i].ItemArray[4].ToString();  //dgrvSalesItemList.Rows[i].Cells[4].Value.ToString();
        string itNam = dt.Rows[i][0].ToString(); // dt.Rows[i].Cells[0].Value.ToString();
        double qty = Convert.ToDouble(dt.Rows[i][2].ToString());
        double Rprice = Convert.ToDouble(dt.Rows[i][1].ToString());
        double total = Convert.ToDouble(dt.Rows[i][3].ToString());
        double dis = Convert.ToDouble(dt.Rows[i][7].ToString()); //discount rate
        double taxapply = Convert.ToDouble(dt.Rows[i][8].ToString());
        int kitchendisplay = Convert.ToInt32(dt.Rows[i][9].ToString());
        string notes = dt.Rows[i].ItemArray[10].ToString();


        // =================================Start=====  Profit calculation =============== Start ========= 
        // Discount_amount = (Retail_price * discount) / 100                    -- 49 * 3 / 100 = 1.47
        // Retail_priceAfterDiscount = Retail_price - Discount_amount           -- 49 - 1.47 = 47.53
        // Profit = (Retail_priceAfterDiscount * QTY )   - (cost_price * qty);  ---( 47.53 * 1 ) - ( 45 * 1) = 2.53

        string sqlprofit = "Select cost_price , discount  from  purchase  where product_id  = '" + itemid + "'";
        DataAccess.ExecuteSQL(sqlprofit);
        DataTable dt1 = DataAccess.GetDataTable(sqlprofit);

        double cost_price = Convert.ToDouble(dt1.Rows[0].ItemArray[0].ToString());
        double discount = Convert.ToDouble(dt1.Rows[0].ItemArray[1].ToString());

        double Discount_amount = (Rprice * discount) / 100.00;
        double Retail_priceAfterDiscount = Rprice - Discount_amount;
        double Profit = Math.Round((Retail_priceAfterDiscount - cost_price), 2); // old calculation (Retail_priceAfterDiscount * qty) - (cost_price * qty);
                                                                                 // =================================Start=====  Profit calculation =============== Start =========  


        string sql1 = " insert into sales_item (sales_id,itemName,Qty,RetailsPrice,Total, profit,sales_time, itemcode , discount, taxapply, note, status) " +
                      " values ('" + trno + "', '" + itNam + "', '" + qty + "', '" + Rprice + "', '" + total + "', '" + Profit + "', " +
                      " '" + salesdate + "','" + itemid + "','" + dis + "','" + taxapply + "','" + notes + "','" + kitchendisplay + "')";
        DataAccess.ExecuteSQL(sql1);

        //update quantity Decrease from Stock Qty |  purchase Table
        if (txtInvoice.Text == "")
        {
          MessageBox.Show("please check sales no ");
        }
        else
        {

          string itemids = dt.Rows[i][4].ToString();
          double qtyupdate = Convert.ToDouble(dt.Rows[i][2].ToString());

          // Update Quantity
          string sqlupdateQty = "select product_quantity  from purchase where product_id = '" + itemids + "'";
          DataAccess.ExecuteSQL(sqlupdateQty);
          DataTable dtUqty = DataAccess.GetDataTable(sqlupdateQty);
          double product_quantity = Convert.ToDouble(dtUqty.Rows[0].ItemArray[0].ToString()) - qtyupdate;

          string sql = " update purchase set " +
                          " product_quantity = '" + product_quantity + "' " +
                          " where product_id = '" + itemids + "' ";
          DataAccess.ExecuteSQL(sql);
        }

      }
      return true;

    }

    /// //// Payment items Add  ///////////Store into Sales_payment table //////// 
    public void payment_item(string payamount, string changeamount, string dueamount, string salestype, string salesdate, string custid, string Comment)
    {
      string trno = txtInvoice.Text;
      //string payamount = lblTotalPayable.Text;
      // string changeamount = txtChangeAmount.Text;
      //  string due = txtDueAmount.Text;
      string vat = lblTotalVAT.Text;
      string DiscountTotal = lblOverallDiscount.Text; //lblTotalDisCount.Text;
                                                      // string Comment = txtCustName.Text + " " + txtcomment.Text;
      string overalldisRate = txtDiscountRate.Text;
      string vatRate = txtVATRate.Text;
      string ordertime = DateTime.Now.ToString("HH:mm");
      string tableno = lblTableNo.Text;
      string tokenno = txttokenno.Text;

      string sql1 = " insert into sales_payment (sales_id, payment_type, payment_amount, change_amount, due_amount, dis, vat, " +
                      " sales_time, c_id, emp_id, comment, TrxType, Shopid , ovdisrate , vaterate, ordertime , tableno, tokenno ) " +
                      "  values ('" + txtInvoice.Text + "','" + salestype + "', '" + payamount + "', '" + changeamount + "', " +
                      " '" + dueamount + "', '" + DiscountTotal + "', '" + vat + "', '" + salesdate + "', '" + custid + "', " +
                      " '" + UserInfo.UserName + "','" + Comment + "','POS', '" + UserInfo.Shopid + "', '" + overalldisRate + "' , '" + vatRate + "', " +
                      " '" + ordertime + "', '" + tableno + "', '" + tokenno + "' )";
      DataAccess.ExecuteSQL(sql1);
    }

    /// //// Token no Add  ///////////Store into tbl_tokenno table //////// 
    public void tokennoInsert()
    {
      string trno = txtInvoice.Text;
      string payamount = lblTotalPayable.Text;
      string tokenno = txttokenno.Text;

      string sqltkn = " insert into tbl_tokenno (sales_id, tokenno, sales_date) " +
                      "  values ('" + txtInvoice.Text + "','" + tokenno + "',   '" + dtSalesDate.Text + "' )";
      DataAccess.ExecuteSQL(sqltkn);
    }

    //Sale and print
    private void btnCompleteSalesAndPrint_Click(object sender, RoutedEventArgs e)
    {
      if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
      {
        //  MessageBox.Show("Please insert paid amount", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert paid amount", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        txtPaidAmount.Focus();
      }
      //else if (Convert.ToInt64(txtInvoice.Text) >= InvoicesManager.InvoiceNo)
      //{
      //  MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \nhttps://goo.gl/ktvmHn ", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
      //  txtbarcodescan.Focus();
      //}
      else
      {
        try
        {
          //if (MessageBox.Show("Do you want to Complete this transaction?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
          {
            ////Save payment info into sales_payment table
            payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, CombPayby.Text, dtSalesDate.Text, lblCustID.Text, txtcomment.Text);

            ///// save sales items one by one  
            sales_item(dtSalesDate.Text);

            //// TokennoInsert
            tokennoInsert();

            // // Change Hold Status to normal
            if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }

            //// Booked Table 
            updatetablebooked();

            ///// // Open Print Invoice
            this.Visibility = Visibility.Hidden;
            parameter.autoprint = "1";
            Sales_Register.ReceiptPrint go = new Sales_Register.ReceiptPrint(txtInvoice.Text);
            go.ShowDialog();

            tabPayment.Visibility = Visibility.Hidden;
            t.Rows.Clear();
            DiscountCalculation();
            vatcal();
            //txtbarcodescan.Focus();
          }

        }
        catch (Exception exp)
        {
          MessageBox.Show(exp.Message);
        }

      }
      txtbarcodescan.Focus();
    }

    //Only Save
    private void btnSaveOnly_Click(object sender, RoutedEventArgs e)
    {
      if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
      {
        // MessageBox.Show("Please insert paid amount", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
        growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert paid amount", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
        txtPaidAmount.Focus();
      }
      //else if (Convert.ToInt64(txtInvoice.Text) >= InvoicesManager.InvoiceNo)
      //{
      //  MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : sales@poscube.co.uk \nhttps://goo.gl/ktvmHn ", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
      //}
      else
      {
        try
        {
          ////Save payment info into sales_payment table
          payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, CombPayby.Text, dtSalesDate.Text, lblCustID.Text, txtcomment.Text);

          ///// save sales items one by one  
          sales_item(dtSalesDate.Text);

          //// TokennoInsert
          tokennoInsert();

          tokennumberget();

          // // Change Hold Status to normal
          if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }
          HolditemDatabind();
          //// Booked Table 
          updatetablebooked();
          //tablezoneDatabind();
          tabPayment.Visibility = Visibility.Hidden;
          tabSalesRegister.SelectedItem = tabterminal;
          t.Rows.Clear();
          DiscountCalculation();
          vatcal();
          txtbarcodescan.Focus();
          growlNotifications.AddNotification(new Notification { Title = "Wow ", Message = "Transaction has been successfully done", ImageUrl = "pack://application:,,,/Notifications/notification-icon.png" });
        }
        catch (Exception exp)
        {
          MessageBox.Show(exp.Message);
        }

      }
      txtbarcodescan.Focus();
    }

    #region  paid due calculation, customer info , over all discount rate calculation
    private void txtPaidAmount_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (lblTotalPayable.Text == "")
      {
        // MessageBox.Show("please insert Amount ");
      }
      else
      {
        try
        {
          if (!txtPaidAmount.Text.Equals("") && Convert.ToDouble(txtPaidAmount.Text) >= Convert.ToDouble(lblTotalPayable.Text))
          {
            double changeAmt = Convert.ToDouble(txtPaidAmount.Text) - Convert.ToDouble(lblTotalPayable.Text);
            changeAmt = Math.Round(changeAmt, 2);
            txtChangeAmount.Text = changeAmt.ToString();
            txtDueAmount.Text = "0";
          }
          if (Convert.ToDouble(txtPaidAmount.Text) <= Convert.ToDouble(lblTotalPayable.Text))
          {
            double changeAmt = Convert.ToDouble(lblTotalPayable.Text) - Convert.ToDouble(txtPaidAmount.Text);
            changeAmt = Math.Round(changeAmt, 2);
            txtDueAmount.Text = changeAmt.ToString();
            txtChangeAmount.Text = "0";
          }
          btnCompleteSalesAndPrint.Focus();
        }
        catch //(Exception exp)
        {
            Console.WriteLine("error3");
            txtPaidAmount.Text = "";
                    //btnCompleteSalesAndPrint.Focus();
                    // MessageBox.Show(exp.Message);
          txtbarcodescan.Focus();
        }

      }
    }

    private void txtPaidAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
      }
      catch
      { }

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
      txtbarcodescan.Focus();
    }

    // //Discount Overall
    private void txtDiscountRate_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        if (lblTotalPayable.Text == "")
        {
          MessageBox.Show("Please Add at least One Item");
        }
        else
        {
          double Discountvalue = Convert.ToDouble(txtDiscountRate.Text);
          txtDiscountRate.Text = Discountvalue.ToString();
          double subtotal = Convert.ToDouble(lblTotal.Text) - Convert.ToDouble(lblTotalDisCount.Text); // total - item discount  100 - 5 = 95        
          double totaldiscount = (subtotal * Discountvalue) / 100; //Counter discount  // 95 * 5 /100 = 4.75  
                                                                   // double totaldiscount = Convert.ToDouble(lblTotalDisCount.Text) + Discountvalue;   // Uncomment this line if you want to discount value and comment/delete above line 
          double disPlusOverallDiscount = totaldiscount + Convert.ToDouble(lblTotalDisCount.Text); // 4.75 + 5 = 9.75
          disPlusOverallDiscount = Math.Round(disPlusOverallDiscount, 2);
          lblOverallDiscount.Text = disPlusOverallDiscount.ToString();  // Overall discount 9.75

          double subtotalafteroveralldiscount = subtotal - totaldiscount; // 95 - 4.75 = 90.25
          subtotalafteroveralldiscount = Math.Round(subtotalafteroveralldiscount, 2);
          lblsubtotal.Text = subtotalafteroveralldiscount.ToString();

          double payable = subtotalafteroveralldiscount + Convert.ToDouble(lblTotalVAT.Text);
          payable = Math.Round(payable, 2);
          lblTotalPayable.Text = payable.ToString();
          lblTotalPayableTabpayment.Text = payable.ToString();
                    txtTotalAmount.Text = ConvertDoubleFormat(payable.ToString());
          btnCurrentAmount.Content = ConvertDoubleFormat(payable.ToString());

          txtPaidAmount.Text = ConvertDoubleFormat(payable.ToString());

        }
      }
      catch
      {
        Console.WriteLine("error5");
        txtDiscountRate.Text = "0";
        //lbloveralldiscount.Text = lblTotalDisCount.Text;
      }
    }
    #endregion

    // //Current amount Direct insert to paid amount box
    private void btnCurrentAmount_Click(object sender, RoutedEventArgs e)
    {
      txtPaidAmount.Text = ConvertDoubleFormat(lblTotalPayable.Text);
      txtbarcodescan.Focus();
    }

    //Back to Sales cart tab
    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
      //this.Visibility = Visibility.Hidden;
      //Sales_Register.SalesRegister go = new Sales_Register.SalesRegister();
      //go.Show();
      tabSalesRegister.SelectedItem = tabterminal;
      txtbarcodescan.Focus();
    }

    #region //////////////////   Currency shortcuts value get
    public delegate void functioncall(string currencyvalue);
    public delegate void numvaluefunctioncall(string Numvalue);

    private event functioncall formFunctionPointer;
    private event numvaluefunctioncall numformFunctionPointer;

    private void Replicate(string currencyvalue)
    {
      try
      {
        if (currencyvalue == "XX") // All clear
        {
          txtPaidAmount.Text = "";
        }
        else if (currencyvalue == "BXC") //Backspace
        {
          if ((String.Compare(txtPaidAmount.Text, " ") < 0))
          {
            txtPaidAmount.Text = txtPaidAmount.Text.Substring(0, txtPaidAmount.Text.Length - 1 + 1);
          }
          else
          {
            txtPaidAmount.Text = txtPaidAmount.Text.Substring(0, txtPaidAmount.Text.Length - 1);
          }
          txtPaidAmount.Focus();
        }
        else
        {
          if (string.IsNullOrEmpty(txtPaidAmount.Text))
          {
            txtPaidAmount.Text = "0.00";
            txtPaidAmount.Text = (Convert.ToDouble(txtPaidAmount.Text) + Convert.ToDouble(currencyvalue)).ToString();
          }
          else
          {
            txtPaidAmount.Text = (Convert.ToDouble(txtPaidAmount.Text) + Convert.ToDouble(currencyvalue)).ToString();
          }
          txtPaidAmount.Focus();
        }
      }
      catch
      {
                Console.WriteLine("error6");
      }
        DisplayToPole(ConvertDoubleFormat(txtPaidAmount.Text), ConvertDoubleFormat(txtChangeAmount.Text));
    }

    private void NumaricKeypad(string Numvalue)
    {

      try
      {
        txtPaidAmount.Text += Numvalue;
        if (Convert.ToDouble(txtPaidAmount.Text) == 0.0)
            txtPaidAmount.Text = "";
        txtPaidAmount.CaretIndex = txtPaidAmount.Text.Length;
        txtPaidAmount.Focus();
      }
      catch
      {

      }
        DisplayToPole(ConvertDoubleFormat(txtPaidAmount.Text), ConvertDoubleFormat(txtChangeAmount.Text));
    }

    #endregion

    // Table layout Data bind
    public void tablezoneDatabind()
    {
      string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      string sqltablezone = " select  zonenam, tablename,  convert(nvarchar(50), seatqty)  +  seatqty   as 'seat', fillcolor, " +
                               " CASE      " +
                               " WHEN  convert(int, convert(varchar(10),   DATEDIFF(MINUTE, ordertime, getdate()),    112))   < 30  THEN   'B'   " +
                               " ELSE '-'  " +
                               " END 'booked', seatqty   " +
                               "  from tbl_tablezone " +
                               " where status = 1  order by orderno ";
      DataAccess.ExecuteSQL(sqltablezone);
      DataTable dttablezone = DataAccess.GetDataTable(sqltablezone);
      lstvwTableList.ItemsSource = dttablezone.DefaultView;
    }

    //// Select Table  
    private void lstvwTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        string Tablelist = "";
        foreach (DataRowView row in lstvwTableList.SelectedItems)
        {
          Tablelist = row.Row[1].ToString();
        }

        lblTableNo.Text = Tablelist;
        parameter.holdtableno = lblTableNo.Text;
        lstvwTableList.Background = new SolidColorBrush(Colors.Cyan);
      }
      catch
      {
      }
      txtbarcodescan.Focus();
    }
    #endregion

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);

        tabterminal.Header = res_man.GetString("tabterminal", cul);
        tabPayment.Header = res_man.GetString("tabPayment", cul);
        expndtabler.Header = res_man.GetString("lblselecttableSRtitle", cul);
        lblscannerTitle.Text = res_man.GetString("lblscannerSRTitle", cul);
        lblTotalItemstypes.Content = res_man.GetString("lblTotalItemstypes", cul);
        btnDirectSale.Content = res_man.GetString("btnDirectSale", cul);
        btnReceivePayment.Content = res_man.GetString("btnReceivePayment", cul);
        btnSuspend.Content = res_man.GetString("btnSuspend", cul);
        tabGridview.Header = res_man.GetString("tabGridview", cul);
        tabSauceOptions.Header = res_man.GetString("tabSauceOptionsSR", cul);
        lblpaybytitle.Text = res_man.GetString("lblpaybytitle", cul);
        lblchngamttitle.Text = res_man.GetString("lblchngamttitle", cul);
        lblduetitle.Text = res_man.GetString("lblduetitle", cul);
        lblpaidamt.Text = res_man.GetString("lblpaidamt", cul);
        lbltokonnoSRtitle.Text = res_man.GetString("lbltokonnoSRtitle", cul);

        lbltotalSRtitle.Content = res_man.GetString("lbltotaltitle", cul);
        lbltotalSRtitle2.Content = res_man.GetString("lbltotaltitle", cul);
        lbldisSRtitle.Content = res_man.GetString("lbldiscounttitle", cul);
        lbltaxSRtitle.Content = res_man.GetString("lbltattitle", cul);
        lblsubSRtitle.Content = res_man.GetString("lblsubtotaltitle", cul);
        lblsearchSRItemtitle.Text = res_man.GetString("lblsearchtitlestock", cul);
        lbltotalSRPaytitle.Content = res_man.GetString("lbltotalpayabletitle", cul);
        lbldisrateSRtitle.Text = res_man.GetString("lbldiscounttitle", cul);
        lbldateSRtitle.Text = res_man.GetString("lbldatetitle", cul);
        lblcustomerSRtitle.Text = res_man.GetString("lblCustomertitle", cul);
        lblcommentSRtitle.Text = res_man.GetString("lblCommenttitle", cul);
        lblmsg.Content = res_man.GetString("lblmsgSR", cul);
        btnSaveOnly.Content = res_man.GetString("btnSave", cul);
        btnBack.Content = res_man.GetString("btnBack", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }



        private void lstvwStocklist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null && !(e.OriginalSource is System.Windows.Controls.ScrollViewer))
            {
                System.Diagnostics.Debug.WriteLine(item.ToString());
                barcodeInput();
                txtbarcodescan.Focus();
                (sender as ListView).SelectedIndex = -1;
            }
            txtbarcodescan.Focus();
        }

        private Boolean bNoSelectChange = false;
        private void btnMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            bNoSelectChange = true;
        }

        private void DeleteRows_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            bNoSelectChange = true;
        }

        private void DeleteRows_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            bNoSelectChange = false;
        }

        private void btnMinus_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            bNoSelectChange = false;
        }

        private void txtbarcodescan_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }

        int nLastSelectedSalesRow = -1;
        private void dgrvSalesItemList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!bNoSelectChange && nLastSelectedSalesRow == ((DataGrid)sender).SelectedIndex)
            {
                txtbarcodescan.Focus();
            }
            nLastSelectedSalesRow = ((DataGrid)sender).SelectedIndex;

            ReLayoutPanels();
        }

        private void btnItemPlus_Click(object sender, RoutedEventArgs e)
        {
            // addtocartitem
            //t.Rows.Add(ItemsName, Rprice, i, Total, Itemid, Disamt, Taxamt, Dis, Taxapply, kitchendisplay, options);

            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

            double Rprice = Convert.ToDouble(t.Rows[n][1]);
            double Taxapply = Convert.ToDouble(t.Rows[n][8]);

            int QtyInc = Convert.ToInt32(t.Rows[n][2]);
            t.Rows[n][2] = (QtyInc + 1);  //Qty Increase
            t.Rows[n][3] = Rprice * (QtyInc + 1);   // Total price
                                                    //  dgrvSalesItemList.Rows[n].Cells[4].Value = Itemid;                     

            double qty = Convert.ToDouble(t.Rows[n][2]);
            double disrate = Convert.ToDouble(t.Rows[n][7]);

            if (disrate != 0)  // if discount has
            {
                double DisamtInc = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                t.Rows[n][5] = DisamtInc; // Discount total amount
            }

            if (Taxapply != 0)   // If apply  tax 
            {
                // Total Tax amount  of this item  (Rprice - disamount) * taxRate / 100
                double TaxamtInc = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00);
                t.Rows[n][6] = TaxamtInc; // Total Tax amount
            }

            txtbarcodescan.Text = "";
            txtbarcodescan.Focus();
            DiscountCalculation();
            vatcal();
            lblmsg.Visibility = Visibility.Visible;
            btnholdsale.Visibility = Visibility.Visible;
        }

        private void btnItemMinus_Click(object sender, RoutedEventArgs e)
        {
            // addtocartitem
            //t.Rows.Add(ItemsName, Rprice, i, Total, Itemid, Disamt, Taxamt, Dis, Taxapply, kitchendisplay, options);

            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;

            if (Convert.ToDouble(dataRow.Row[2].ToString()) <= 1)
            {
                txtbarcodescan.Focus();
                return;
            }

            double Qty = Convert.ToDouble(dataRow.Row.ItemArray[2].ToString()) - 1;
            dataRow.Row[2] = Qty;  // Qty Decrease 

            double CurrentQty = Convert.ToDouble(dataRow.Row[2]);
            double Price = Convert.ToDouble(dataRow.Row[1].ToString());
            double disrate = Convert.ToDouble(dataRow.Row[7].ToString());
            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

            //// show total price   Qty  * Rprice
            dataRow.Row[3] = Price * CurrentQty;   // Total Price

            if (disrate != 0)
            {
                double Disamt = (((Price * CurrentQty) * disrate) / 100.00);      // Total Discount amount of this item
                dataRow.Row[5] = Disamt;
            }

            if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
            {
                double Taxamt = ((((Price * CurrentQty) - (((Price * CurrentQty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                dataRow.Row[6] = Taxamt;
            }

            DiscountCalculation();
            vatcal();
           
            txtbarcodescan.Focus();
        }
        private void btnItemCancel_Click(object sender, RoutedEventArgs e)
        {
            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            DataRowView drv = (DataRowView)dgrvSalesItemList.SelectedItem;
            drv.Row.Delete();
            DiscountCalculation();
            vatcal();

            dgrvSalesItemList.SelectedIndex = n <= dgrvSalesItemList.Items.Count-1 ? n : dgrvSalesItemList.Items.Count - 1;
            txtbarcodescan.Focus();
        }

        private void currency_ShortcutsContorl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPayCash_Click(object sender, RoutedEventArgs e)
        {
            if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
            {
                //  MessageBox.Show("Please insert paid amount", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
                growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert paid amount", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
                txtPaidAmount.Focus();
            }
            //else if (Convert.ToInt64(txtInvoice.Text) >= InvoicesManager.InvoiceNo)
            //{
            //    MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \nhttps://goo.gl/ktvmHn ", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    txtbarcodescan.Focus();
            //}
            else
            {
                try
                {
                    //if (MessageBox.Show("Do you want to Complete this transaction?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ////Save payment info into sales_payment table
                        payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, "Cash", dtSalesDate.Text, lblCustID.Text, txtcomment.Text);

                        ///// save sales items one by one  
                        sales_item(dtSalesDate.Text);

                        //// TokennoInsert
                        tokennoInsert();

                        // // Change Hold Status to normal
                        if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }

                        //// Booked Table 
                        updatetablebooked();

                        tabPayment.Visibility = Visibility.Hidden;
                        t.Rows.Clear();
                        DiscountCalculation();
                        vatcal();

                        OpenCashDrawer();
                        DisplayToPole("Welcome", "");
                        //txtbarcodescan.Focus();

                        string strMessage = string.Format("Change: {0} ?\nPrint?", txtChangeAmount.Text);
                        // System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show(strMessage, "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
                        MessageBoxResult dialogResult = CustomMessageBox.ShowYesNo(strMessage, "PosCube", "Yes", "No");
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            ///// // Open Print Invoice
                            // this.Visibility = Visibility.Hidden;
                            parameter.autoprint = "1";
                            Sales_Register.ReceiptPrint go = new Sales_Register.ReceiptPrint(txtInvoice.Text);
                            go.ShowDialog();
                            this.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            tabSalesRegister.SelectedItem = tabterminal;
                            txtPaidAmount.Text = "";
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void btnPayCreditCard_Click(object sender, RoutedEventArgs e)
        {
            if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
            {
                //  MessageBox.Show("Please insert paid amount", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
                growlNotifications.AddNotification(new Notification { Title = "Alert Message", Message = "Please insert paid amount", ImageUrl = "pack://application:,,,/Notifications/Radiation_warning_symbol.png" });
                txtPaidAmount.Focus();
            }
            //else if (Convert.ToInt64(txtInvoice.Text) >= InvoicesManager.InvoiceNo)
            //{
            //    MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \nhttps://goo.gl/ktvmHn ", "Yes or No", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    txtbarcodescan.Focus();
            //}
            else
            {   
                try
                {
                    //if (MessageBox.Show("Do you want to Complete this transaction?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ////Save payment info into sales_payment table
                        payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, "Credit Card", dtSalesDate.Text, lblCustID.Text, txtcomment.Text);

                        ///// save sales items one by one  
                        sales_item(dtSalesDate.Text);

                        //// TokennoInsert
                        tokennoInsert();

                        // // Change Hold Status to normal
                        if (parameter.resumesalesstatus == "1") { resumestatuschange(parameter.holdtransactionID); }

                        //// Booked Table 
                        updatetablebooked();


                        tabPayment.Visibility = Visibility.Hidden;
                        t.Rows.Clear();
                        DiscountCalculation();
                        vatcal();

                        OpenCashDrawer();
                        DisplayToPole("Welcome", "");
                        //txtbarcodescan.Focus();

                        String strMessage = string.Format("Change: {0} ?\nPrint?", txtChangeAmount.Text);
                        //System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show(strMessage, "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
                        MessageBoxResult dialogResult = CustomMessageBox.ShowYesNo(strMessage, "PosCube", "Yes", "No");
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            ///// // Open Print Invoice
                            // this.Visibility = Visibility.Hidden;
                            parameter.autoprint = "1";
                            Sales_Register.ReceiptPrint go = new Sales_Register.ReceiptPrint(txtInvoice.Text);
                            go.ShowDialog();
                            this.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            tabSalesRegister.SelectedItem = tabterminal;
                            txtPaidAmount.Text = "";
                        }
                    }

                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void txtbarcodescan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(!txtbarcodescan.Text.Equals(""))
                {
                    // System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Product not found\nDo you want to add product list?", "PosCube", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo);
                    MessageBoxResult dialogResult = CustomMessageBox.ShowYesNo("Product not found\nDo you want to add product list?", "PosCube", "Yes", "No");
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        this.Visibility = Visibility.Hidden;
                        Items.Stock_List go = new Items.Stock_List();
                        go.SetFromSalesFlag(this, true, txtbarcodescan.Text);
                        go.Show();
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
                        txtbarcodescan.Text = "";
                        txtbarcodescan.Focus();
                    }
                }
            }
        }

        private void btnItemQty_Click(object sender, RoutedEventArgs e)
        {
            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;

            EnterAmountPopup popUp = new EnterAmountPopup(0);
            //popUp.txtAmount.Text = dataRow.Row.ItemArray[2].ToString();
            //popUp.txtAmount.CaretIndex = popUp.txtAmount.Text.Length;
            bool bReturn = (bool)popUp.ShowDialog();
            if(bReturn)
            {
                if (popUp.txtAmount.Text.Length <= 0 || Convert.ToDouble(popUp.txtAmount.Text) <= 0)
                {
                    txtbarcodescan.Focus();
                    return;
                }
                double Qty = Convert.ToDouble(popUp.txtAmount.Text);
                dataRow.Row[2] = Qty;

                double CurrentQty = Convert.ToDouble(dataRow.Row[2]);
                double Price = Convert.ToDouble(dataRow.Row[1].ToString());
                double disrate = Convert.ToDouble(dataRow.Row[7].ToString());
                double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                //// show total price   Qty  * Rprice
                dataRow.Row[3] = Price * CurrentQty;   // Total Price

                if (disrate != 0)
                {
                    double Disamt = (((Price * CurrentQty) * disrate) / 100.00);      // Total Discount amount of this item
                    dataRow.Row[5] = Disamt;
                }

                if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
                {
                    double Taxamt = ((((Price * CurrentQty) - (((Price * CurrentQty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                    dataRow.Row[6] = Taxamt;
                }

                DiscountCalculation();
                vatcal();
            }
            txtbarcodescan.Focus();
        }

        private void btnItemChangePrice_Click(object sender, RoutedEventArgs e)
        {
            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;

            EnterAmountPopup popUp = new EnterAmountPopup(1);
            //popUp.txtAmount.Text = dataRow.Row.ItemArray[1].ToString();
            //popUp.txtAmount.CaretIndex = popUp.txtAmount.Text.Length;
            bool bReturn = (bool)popUp.ShowDialog();
            if (bReturn)
            {
                if (popUp.txtAmount.Text.Length <= 0 || Convert.ToDouble(popUp.txtAmount.Text) <= 0)
                {
                    txtbarcodescan.Focus();
                    return;
                }

                dataRow.Row[1] = Convert.ToDouble(popUp.txtAmount.Text);

                double CurrentQty = Convert.ToDouble(dataRow.Row[2]);
                double Price = Convert.ToDouble(dataRow.Row[1].ToString());
                double disrate = Convert.ToDouble(dataRow.Row[7].ToString());
                double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                //// show total price   Qty  * Rprice
                dataRow.Row[3] = Price * CurrentQty;   // Total Price

                if (disrate != 0)
                {
                    double Disamt = (((Price * CurrentQty) * disrate) / 100.00);      // Total Discount amount of this item
                    dataRow.Row[5] = Disamt;
                }

                if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
                {
                    double Taxamt = ((((Price * CurrentQty) - (((Price * CurrentQty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                    dataRow.Row[6] = Taxamt;
                }

                DiscountCalculation();
                vatcal();
            }
            txtbarcodescan.Focus();
        }

        private void btnItemWeight_Click(object sender, RoutedEventArgs e)
        {
            int n = dgrvSalesItemList.SelectedIndex;
            if (n < 0)
            {
                txtbarcodescan.Focus();
                return;
            }

            DataRowView dataRow = (DataRowView)dgrvSalesItemList.SelectedItem;

            EnterAmountPopup popUp = new EnterAmountPopup(2);
            //popUp.txtAmount.Text = dataRow.Row.ItemArray[1].ToString();
            //popUp.txtAmount.CaretIndex = popUp.txtAmount.Text.Length;
            double weight = 0;
            string strUnit = "";
            if (dataRow.Row[11].ToString().Length > 0)
            {
                string numericValue = new String(dataRow.Row[11].ToString().Where(Char.IsDigit).ToArray());
                weight = double.Parse(numericValue);
                strUnit = new String(dataRow.Row[11].ToString().Where(Char.IsLetter).ToArray());
            }
            popUp.m_fWeight = weight;
            popUp.m_strUnit = strUnit;
            popUp.Title = "Weight = 0" + popUp.m_strUnit;
            popUp.m_fPrice = Convert.ToDouble(dataRow.Row[1].ToString());
            bool bReturn = (bool)popUp.ShowDialog();

            if (bReturn)
            {
                if (popUp.txtAmount.Text.Length <= 0 || Convert.ToDouble(popUp.txtAmount.Text) <= 0)
                {
                    txtbarcodescan.Focus();
                    return;
                }

                dataRow.Row[1] = Convert.ToDouble(popUp.txtAmount.Text);

                double CurrentQty = Convert.ToDouble(dataRow.Row[2]);
                double Price = Convert.ToDouble(dataRow.Row[1].ToString());
                double disrate = Convert.ToDouble(dataRow.Row[7].ToString());
                double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                //// show total price   Qty  * Rprice
                dataRow.Row[3] = Price * CurrentQty;   // Total Price

                if (disrate != 0)
                {
                    double Disamt = (((Price * CurrentQty) * disrate) / 100.00);      // Total Discount amount of this item
                    dataRow.Row[5] = Disamt;
                }

                if (Convert.ToDouble(dataRow.Row[8].ToString()) != 0)
                {
                    double Taxamt = ((((Price * CurrentQty) - (((Price * CurrentQty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                    dataRow.Row[6] = Taxamt;
                }

                DiscountCalculation();
                vatcal();
            }
            txtbarcodescan.Focus();
        }

        private void btnDiscount_Click(object sender, RoutedEventArgs e)
        {
            EnterAmountPopup popUp = new EnterAmountPopup(3);
            //if(Convert.ToDouble(txtDiscountRate.Text) == 0)
            //{
            //    popUp.txtAmount.Text = "";
            //}
            //else
            //{
            //    popUp.txtAmount.Text = txtDiscountRate.Text;
            //    popUp.txtAmount.CaretIndex = popUp.txtAmount.Text.Length;
            //}
            
            bool bReturn = (bool)popUp.ShowDialog();
            if(bReturn)
            {
                txtDiscountRate.Text = popUp.txtAmount.Text;
            }
        }

        public string ConvertDoubleFormat(string strInput)
        {
            double dbTemp = double.Parse(strInput);
            string s = string.Format("{0:N2}", dbTemp);
            return s;
        }

        public void OpenCashDrawer()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");

            string port = data["COM"]["CashDrawer"];
            if(!port.Equals(""))
            {
                using (SerialPort serialPort = new SerialPort(port))
                {
                    serialPort.Close();

                    serialPort.BaudRate = 9600;     // Pole Bound Rate 
                    serialPort.Parity = System.IO.Ports.Parity.None;
                    serialPort.DataBits = 8;   // Data Bits
                    serialPort.StopBits = System.IO.Ports.StopBits.One;

                    serialPort.Open();
                    serialPort.Write("\x001B@\x001Bp\0.}");
                    serialPort.Close();
                }
            }
        }

        public void DisplayToPole(string strLine1, string strLine2)
        {
            //Console.WriteLine("Line1:" + strLine1);
            //Console.WriteLine("Line2:" + strLine2);
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");

            string port = data["COM"]["PoleDisplay"];
            if (!port.Equals(""))
            {
                using (SerialPort serialPort = new SerialPort(port))
                {
                    serialPort.Close();

                    serialPort.BaudRate = 9600;     // Pole Bound Rate 
                    serialPort.Parity = System.IO.Ports.Parity.None;
                    serialPort.DataBits = 8;   // Data Bits
                    serialPort.StopBits = System.IO.Ports.StopBits.One;

                    serialPort.Open();

                    // Line 1
                    serialPort.Write(new byte[] { 0x0C }, 0, 1);
                    serialPort.WriteLine(strLine1);

                    // Line 2
                    serialPort.Write(new byte[] { 0x0A, 0x0D }, 0, 2);
                    serialPort.Write(strLine2);

                    serialPort.Close();
                    serialPort.Dispose();
                }
            }
        }

        private void SalesRegisterForm_ContentRendered(object sender, EventArgs e)
        {

        }
    }
}
