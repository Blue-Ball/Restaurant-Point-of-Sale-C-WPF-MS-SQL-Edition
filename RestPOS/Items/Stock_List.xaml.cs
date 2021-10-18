using Microsoft.Win32;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PosCube.Items
{
  /// <summary>
  /// Interaction logic for Stock_List.xaml
  /// </summary>
  public partial class Stock_List : Window
  {
    ResourceManager res_man;
    CultureInfo cul;
    public bool m_bFromSales;
        public Window m_prevWindow;

    OpenFileDialog openFileDialog1 = new OpenFileDialog();
    public Stock_List()
    {
            
      InitializeComponent();
            btnSaveAndReturn.Visibility = Visibility.Hidden;
            m_prevWindow = null;
        }

        public void SetFromSalesFlag(Window prevWindow, bool bFlag, string strCode)
        {
            m_bFromSales = bFlag;
            if(m_bFromSales)
            {
                btnSaveAndReturn.Visibility = Visibility.Visible;
                tabitemview.Visibility = Visibility.Visible;
                tabcontrolItemspanel.SelectedIndex = 1;
                txtItembarcode.Text = strCode;
                m_prevWindow = prevWindow;
            }
            else
            {
                btnSaveAndReturn.Visibility = Visibility.Hidden;
            }
        }


    #region Databind
    public void StockitemDatabind(string value)
    {
      //var uriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\ITEMIMAGE\\", UriKind.RelativeOrAbsolute);
      string sqlCmd = " Select product_id, product_name , product_quantity,  retail_price ,  imagename as  imagename " +
                      " from  purchase  where   product_name like '%" + value + "%'  " +
                      " OR ( product_id like '" + value + "%' ) " +
                      " OR (category like '" + value + "%')  ";
      DataAccess.ExecuteSQL(sqlCmd);
      DataTable dt1 = DataAccess.GetDataTable(sqlCmd);
      lstvwStocklist.ItemsSource = dt1.DefaultView;
      lblItemcount.Text = lstvwStocklist.Items.Count.ToString() + " Items Found";

      //if (tabcontrolItemspanel.SelectedItem != tabItemlist)
      //{
      //    tabcontrolItemspanel.SelectedItem = tabItemlist; 
      //}

      txtSearch.Focus();
    }

    private void StockformLoad(object sender, RoutedEventArgs e)
    {
      try
      {

        //Supplier Info
        string sqlCust = "select   DISTINCT  *   from tbl_customer where peopleType = 'Supplier' order by name";
        DataAccess.ExecuteSQL(sqlCust);
        DataTable dtCust = DataAccess.GetDataTable(sqlCust);
        cmboSupplier.ItemsSource = dtCust.DefaultView;
        cmboSupplier.DisplayMemberPath = "name";
        //cmboSupplier.SelectedIndex = 1;
        // cmboSupplier.Text = "Unknown";

        //Category list
        string sqlcate = "select DISTINCT   category_name from tbl_category order by category_name ";
        DataAccess.ExecuteSQL(sqlcate);
        DataTable dtcate = DataAccess.GetDataTable(sqlcate);
        cmboCategory.ItemsSource = dtcate.DefaultView;
        cmboCategory.DisplayMemberPath = "category_name";
        cmboItemCategory.ItemsSource = dtcate.DefaultView;
        cmboItemCategory.DisplayMemberPath = "category_name";
        cmboItemCategory.SelectedValuePath = "category_name";

        // Bindshop branch 
        string sqlloc = "select   *   from tbl_terminallocation ";
        DataAccess.ExecuteSQL(sqlloc);
        DataTable dtloc = DataAccess.GetDataTable(sqlloc);
        cmboLocation.ItemsSource = dtloc.DefaultView;
        cmboLocation.DisplayMemberPath = "branchname";
        cmboLocation.SelectedValuePath = "shopid";

        StockitemDatabind("");

        btnAddnew.Visibility = Visibility.Hidden;
        btnDelete.Visibility = Visibility.Hidden;
        lblItemcode.Visibility = Visibility.Hidden;

        // CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
        // ci.DateTimeFormat.ShortDatePattern = "yyyy-MMM-dd";
        //  Thread.CurrentThread.CurrentCulture = ci;
        switch_language();
      }
      catch { }

    }

    private void ItemDetailsData()
    {
      string sql = "select product_id, product_name, product_quantity, cost_price, retail_price, category,  " +
                  " supplier, imagename, discount, Shopid, taxapply, status, description, weight, mdate, edate " +
                  " from purchase where product_id = '" + lblItemcode.Text + "'";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);

      //txtProductCode.Text = dt1.Rows[0].ItemArray[0].ToString();
      txtItembarcode.Text = dt1.Rows[0].ItemArray[0].ToString();
      txtitemName.Text = dt1.Rows[0].ItemArray[1].ToString();
      txtQty.Text = dt1.Rows[0].ItemArray[2].ToString();
      txtPurchaseprice.Text = dt1.Rows[0].ItemArray[3].ToString();
      txtSalePrice.Text = dt1.Rows[0].ItemArray[4].ToString();
      cmboCategory.Text = dt1.Rows[0].ItemArray[5].ToString();
      cmboSupplier.Text = dt1.Rows[0].ItemArray[6].ToString();

      txtDiscountRate.Text = dt1.Rows[0].ItemArray[8].ToString();
      cmboLocation.SelectedValue = dt1.Rows[0].ItemArray[9].ToString();

      if (dt1.Rows[0].ItemArray[10].ToString() == "1")
      {
        chkTxaApply.IsChecked = true;
      }
      else
      {
        chkTxaApply.IsChecked = false;
      }

      if (dt1.Rows[0].ItemArray[11].ToString() == "3")  // 3 = show kitchen display 
      {
        chkkitchenDisplay.IsChecked = true;
      }
      else
      {
        chkkitchenDisplay.IsChecked = false;
      }
      lblimagename.Text = dt1.Rows[0].ItemArray[7].ToString();
      btnAddnew.Visibility = Visibility.Visible;
      btnDelete.Visibility = Visibility.Visible;
      lblItemcode.Visibility = Visibility.Visible;
      txtItembarcode.IsReadOnly = true;
      btnSave.Content = "Update";


      //  string location = System.AppDomain.CurrentDomain.BaseDirectory + @"\ITEMIMAGE\" + dt1.Rows[0].ItemArray[7].ToString();         
      //var uriSource = new Uri(@"\bin\Debug\ITEMIMAGE\" + dt1.Rows[0].ItemArray[7].ToString(), UriKind.Relative);
      //  var uriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\ITEMIMAGE\\" + dt1.Rows[0].ItemArray[7].ToString(), UriKind.RelativeOrAbsolute);
      //  picItemimage.Source = new BitmapImage(uriSource);
      txtDescription.Text = dt1.Rows[0].ItemArray[12].ToString();
      txtWeight.Text = dt1.Rows[0].ItemArray[13].ToString();
      dtMdate.Text = dt1.Rows[0].ItemArray[14].ToString();
      dtExpiredate.Text = dt1.Rows[0].ItemArray[15].ToString();

      //string path = AppDomain.CurrentDomain.BaseDirectory + "\\ITEMIMAGE\\" + dt1.Rows[0].ItemArray[7].ToString();
      //if (!File.Exists(path))
      //{
      //    picItemimage.Source = null;
      //}
      //else
      //{
      //    picItemimage.Source = BitmapFromUri(new Uri(path));
      //}

      byte[] img = (byte[])(dt1.Rows[0].ItemArray[7]);
      if (img == null)
      {
        picItemimage.Source = null;
      }
      else
      {
        Stream StreamObj = new MemoryStream(img);
        BitmapImage BitObj = new BitmapImage();
        BitObj.BeginInit();
        BitObj.StreamSource = StreamObj;
        BitObj.EndInit();
        this.picItemimage.Source = BitObj;
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

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);

        lblstockpagetitle.Content = res_man.GetString("lblstockpagetitle", cul);
        lblsearchtitle.Text = res_man.GetString("lblsearchtitlestock", cul);
        lblselectcategorytitle.Text = res_man.GetString("lblselectcategorytitle", cul);
        tabItemlist.Header = res_man.GetString("tabItemlist", cul);
        tabitemview.Header = res_man.GetString("tabitemview", cul);
        tabsauceoptions.Header = res_man.GetString("tabsauceoptions", cul);
        tabItemsCategories.Header = res_man.GetString("tabItemsCategories", cul);
        tabImportItem.Header = res_man.GetString("tabImportItem", cul);
        tabItemsAlert.Header = res_man.GetString("tabItemsAlert", cul);
        lblitembarcodetitle.Text = res_man.GetString("lblitembarcodetitle", cul);
        lblitemnametitle.Text = res_man.GetString("lblitemnametitle", cul);
        lbldescriptitle.Text = res_man.GetString("lbldescriptitle", cul);
        lblqtytitle.Text = res_man.GetString("lblqtytitle", cul);
        lblpurchasetitle.Text = res_man.GetString("lblpurchasetitle", cul);
        lblsalepricetitle.Text = res_man.GetString("lblsalepricetitle", cul);
        lbldisctitle.Text = res_man.GetString("lbldisctitle", cul);
        lblcategorytitle.Text = res_man.GetString("lblcategorytitle", cul);
        lblsuppliertitle.Text = res_man.GetString("lblsuppliertitle", cul);
        lblassingtitle.Text = res_man.GetString("lblassingtitle", cul);
        lblistaxapplytitle.Text = res_man.GetString("lblistaxapplytitle", cul);
        chkTxaApply.Content = res_man.GetString("chkTxaApply", cul);
        lbliskitchentitle.Text = res_man.GetString("lbliskitchentitle", cul);
        chkkitchenDisplay.Content = res_man.GetString("chkTxaApply", cul);
        lblweighttitle.Text = res_man.GetString("lblweighttitle", cul);
        lblmanfactdateitle.Text = res_man.GetString("lblmanfactdateitle", cul);
        lblexpdatetitle.Text = res_man.GetString("lblexpdatetitle", cul);


        btnSave.Content = res_man.GetString("btnSave", cul);
        btnAddnew.Content = res_man.GetString("btnAddnew", cul);
        btnDelete.Content = res_man.GetString("btnDelete", cul);
        btnBrowse.Content = res_man.GetString("btnBrowse", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }

    #endregion

    #region Search and Data pass
    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        StockitemDatabind(txtSearch.Text);
      }
      catch
      {
      }
    }

    private void cmboItemCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {

        StockitemDatabind(cmboItemCategory.SelectedValue.ToString());

      }
      catch
      {
      }
    }

    private void lstvwStocklist_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      string itemcode = "";
      foreach (DataRowView row in lstvwStocklist.SelectedItems)
      {
        itemcode = row.Row[0].ToString();
      }
      //  int newIndex = tabcontrolItemspanel.SelectedIndex + 1;
      tabitemview.Visibility = Visibility.Visible;
      tabcontrolItemspanel.SelectedIndex = 1;
      tabitemview.Header = "Item Details View";
      lblItemcode.Text = itemcode;
      // txtItembarcode.Text = itemcode;
      ItemDetailsData();

      ////For New tab
      //Items.Stock_List tabitem = new Items.Stock_List();
      //tabitemview.Header = "Item Details View";
      //tabcontrolItemspanel.Items.Add(tabitem);
    }
    #endregion

    #region  Save and Update data and delete
    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {

      openFileDialog1.CheckFileExists = true;
      openFileDialog1.CheckPathExists = true;

      openFileDialog1.DefaultExt = ".jpg";
      // openFileDialog1.Filter = "GIF files (*.gif)|*.gif| jpg files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
      openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg| PNG files (*.png)|*.png";

      openFileDialog1.FilterIndex = 2;
      openFileDialog1.RestoreDirectory = true;



      if (openFileDialog1.ShowDialog() == true)
      {
        // Create a BitmapImage and sets its DecodePixelWidth and DecodePixelHeight
        //BitmapImage bmpImage = new BitmapImage();
        //bmpImage.BeginInit();
        //bmpImage.UriSource = new Uri(openFileDialog1.FileName, UriKind.RelativeOrAbsolute);
        //bmpImage.DecodePixelWidth = 26;
        //bmpImage.DecodePixelHeight = 26;
        //bmpImage.EndInit();

        //// Set Source property of Image
        //picItemimage.Source = bmpImage;
        // //  lblFileExtension.Text =  openFileDialog.FileName;
        picItemimage.Source = new BitmapImage(new Uri(openFileDialog1.FileName));
        // //// textBox1.Text = openFileDialog1.FileName;
        // //picItemimage.Source = openFileDialog1.FileName;
        lblFileExtension.Text = System.IO.Path.GetExtension(openFileDialog1.FileName);
      }


    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (txtItembarcode.Text == "")
      {
        MessageBox.Show("Please Insert Product Code/ Item Bar-code");
        txtItembarcode.Focus();
      }
      else if (txtitemName.Text == "")
      {
        MessageBox.Show("Please Insert  Product Name");
        txtitemName.Focus();
      }
      else if (txtDiscountRate.Text == "")
      {
        txtDiscountRate.Text = "0";
        txtDiscountRate.Focus();
      }
      else if (txtQty.Text == "")
      {
        MessageBox.Show("Please Insert Product Quantity");
        txtQty.Focus();
      }
      else if (txtPurchaseprice.Text == "")
      {
        MessageBox.Show("Please Insert Product Cost Price / Buy price / purchase Price");
        txtPurchaseprice.Focus();
      }

      else if (txtSalePrice.Text == "")
      {
        MessageBox.Show("Please Insert Product  Sales Price");
        txtSalePrice.Focus();
      }
      else if (cmboCategory.Text == "")
      {
        MessageBox.Show("Please Insert Product Category");
        cmboCategory.Focus();
      }
      else if (cmboLocation.Text == "")
      {
        MessageBox.Show("Please Select Branch name ");
        cmboLocation.Focus();
      }
      else if (cmboSupplier.Text == "")
      {
        MessageBox.Show("Please Select Supplier Name");
        cmboSupplier.Focus();
      }
      else
      {
        try
        {
          string pid = txtItembarcode.Text;
          string pname = txtitemName.Text;
          double quan = Convert.ToDouble(txtQty.Text);
          double cprice = Convert.ToDouble(txtPurchaseprice.Text);
          double sprice = Convert.ToDouble(txtSalePrice.Text);

          double ctotalpri = quan * cprice;
          double rtotalpri = quan * sprice;
          double discount = Convert.ToDouble(txtDiscountRate.Text);

          int taxapply;
          if (chkTxaApply.IsChecked == true)
          {
            taxapply = 1;  //1 = Tax apply
          }
          else
          {
            taxapply = 0; // 0 = Tax not apply
          }

          int kitchenDisplaythisitem;
          if (chkkitchenDisplay.IsChecked == true)
          {
            kitchenDisplaythisitem = 3; // 3 = It's show display on kitchen display
          }
          else
          {
            kitchenDisplaythisitem = 1; // 1 = it's not show on ditcken display.
          }

          //New Insert / New Entry
          if (lblItemcode.Text == "-")
          {
            string imageName = openFileDialog1.FileName;
            if (lblFileExtension.Text == "item.png")
            {
              string sql1 = " insert into purchase (product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, total_retail_price, " +
                        " category, supplier, discount, taxapply, Shopid, status, description, weight, mdate, edate ) " +
                         "  Values ( '" + pid + "', '" + pname + "', '" + quan + "', '" + cprice + "', '" + sprice + "', '" + ctotalpri + "', " +
                         " '" + rtotalpri + "', '" + cmboCategory.Text + "', '" + cmboSupplier.Text + "', '" + discount + "',  " +
                         " '" + taxapply + "', '" + cmboLocation.SelectedValue + "', '" + kitchenDisplaythisitem + "', '" + txtDescription.Text + "',  " +
                         "  '" + txtWeight.Text + "', '" + dtMdate.Text + "', '" + dtExpiredate.Text + "' ) ";
              DataAccess.ExecuteSQL(sql1);
            }
            else
            {
              string sql1 = " insert into purchase (imagename, product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, total_retail_price, " +
                        " category, supplier, discount, taxapply, Shopid, status, description, weight, mdate, edate) " +
                         " SELECT BulkColumn, '" + pid + "', '" + pname + "', '" + quan + "', '" + cprice + "', '" + sprice + "', '" + ctotalpri + "', " +
                         " '" + rtotalpri + "', '" + cmboCategory.Text + "', '" + cmboSupplier.Text + "', '" + discount + "', " +
                         "  '" + taxapply + "', '" + cmboLocation.SelectedValue + "', '" + kitchenDisplaythisitem + "', '" + txtDescription.Text + "', " +
                         "  '" + txtWeight.Text + "', '" + dtMdate.Text + "', '" + dtExpiredate.Text + "' " +
                         " FROM OPENROWSET ( BULK '" + imageName + "', Single_Blob) as img ";
              DataAccess.ExecuteSQL(sql1);
            }

            MessageBox.Show("Item hase been saved Successfully", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            tabcontrolItemspanel.SelectedItem = tabItemlist;
            StockitemDatabind("");


          }
          else  //Update
          {

            string imageName;
            if (lblFileExtension.Text == "item.png")
            {
              string sql = " UPDATE purchase set product_name = '" + txtitemName.Text + "', product_quantity= '" + txtQty.Text + "', " +
                          " cost_price = '" + txtPurchaseprice.Text + "', retail_price= '" + txtSalePrice.Text + "', total_cost_price = '" + ctotalpri + "', " +
                          " total_retail_price= '" + rtotalpri + "', category = '" + cmboCategory.Text + "', supplier = '" + cmboSupplier.Text + "', " +
                          " discount   = '" + discount + "', taxapply = '" + taxapply + "', Shopid = '" + cmboLocation.SelectedValue + "', " +
                          " status =  '" + kitchenDisplaythisitem + "', description = '" + txtDescription.Text + "', weight =  '" + txtWeight.Text + "', " +
                          " mdate = '" + dtMdate.Text + "', edate =  '" + dtExpiredate.Text + "' " +
                          " where product_id = '" + lblItemcode.Text + "' ";
              DataAccess.ExecuteSQL(sql);
            }
            else
            {
              imageName = " ( SELECT BulkColumn FROM OPENROWSET ( BULK  '" + openFileDialog1.FileName + "', SINGLE_BLOB) as img ) ";
              string sql = " UPDATE purchase set  imagename = " + imageName + ", product_name = '" + txtitemName.Text + "', product_quantity= '" + txtQty.Text + "', " +
                       " cost_price = '" + txtPurchaseprice.Text + "', retail_price= '" + txtSalePrice.Text + "', total_cost_price = '" + ctotalpri + "', " +
                       " total_retail_price= '" + rtotalpri + "', category = '" + cmboCategory.Text + "', supplier = '" + cmboSupplier.Text + "', " +
                       " discount   = '" + discount + "' , taxapply = '" + taxapply + "', Shopid = '" + cmboLocation.SelectedValue + "',  " +
                       " status =  '" + kitchenDisplaythisitem + "', description = '" + txtDescription.Text + "', weight =  '" + txtWeight.Text + "',  " +
                       " mdate = '" + dtMdate.Text + "', edate =  '" + dtExpiredate.Text + "' " +
                       " where product_id = '" + lblItemcode.Text + "' ";
              DataAccess.ExecuteSQL(sql);
            }



            //  /////// picture upload  /////////////////      
            //  //if (openFileDialog1.FileName != string.Empty)
            //  //{
            //  //    string destinatiopath = System.AppDomain.CurrentDomain.BaseDirectory + @"\ITEMIMAGE\";
            //  //    string iName = openFileDialog1.SafeFileName;
            //  //    string filepath = openFileDialog1.FileName;
            //  //    System.IO.File.Copy(filepath, destinatiopath + imageName);
            //  //}
            ////  
            tabcontrolItemspanel.SelectedItem = tabItemlist;
            MessageBox.Show("Successfully Data Updated!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            StockitemDatabind("");
            lstvwStocklist.Items.Refresh();

          }
        }
        catch //(Exception exp)
        {
          // MessageBox.Show("Sorry\r\n" + exp.Message);
        }
      }
    }

    public void ClearForm()
    {
      txtItembarcode.IsReadOnly = false;
      txtItembarcode.Text = string.Empty;
      txtitemName.Text = string.Empty;
      txtQty.Text = string.Empty;
      txtPurchaseprice.Text = string.Empty;
      txtSalePrice.Text = string.Empty;
      txtDiscountRate.Text = string.Empty;
      txtDescription.Text = string.Empty;
      txtWeight.Text = string.Empty;
      cmboCategory.Text = string.Empty;
      cmboLocation.Text = string.Empty;
      cmboSupplier.Text = string.Empty;
      lblItemcode.Text = "-";
      chkkitchenDisplay.IsChecked = false;
      picItemimage.Source = null;
      tabitemview.Header = "Add New";
      btnSave.Content = "Save";
    }

    // Form Clear
    private void btnAddnew_Click(object sender, RoutedEventArgs e)
    {
      ClearForm();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (lblItemcode.Text == "-")
      {
        MessageBox.Show("You are Not able to Delete Please select item", "Question", MessageBoxButton.OK, MessageBoxImage.Warning);
      }
      else
      {
        try
        {
          if (MessageBox.Show("Do you want to Delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
          {

            string sql = " delete from purchase where product_id ='" + lblItemcode.Text + "'";
            DataAccess.ExecuteSQL(sql);

            //  lstvwStocklist.ItemsSource = null;
            picItemimage.Source = null;


            //  string destinatiopath = System.AppDomain.CurrentDomain.BaseDirectory + @"ITEMIMAGE\" + lblimagename.Text;
            //  System.IO.File.Delete(destinatiopath);
            tabcontrolItemspanel.SelectedItem = tabItemlist;
            MessageBox.Show("Successfully Data Delete !", "Successful", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            ClearForm();
            StockitemDatabind("");

          }

        }
        catch // (Exception exp)
        {
          // MessageBox.Show("Sorry\r\n You have to Check the Data" + exp.Message);
        }
      }

    }
    #endregion


    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      //PrintDialog printDlg = new PrintDialog();            
      //System.Windows.Size pageSize = new System.Windows.Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight);
      //// sizing of the element.
      //lstvwStocklist.Measure(pageSize);
      //lstvwStocklist.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
      //printDlg.PrintVisual(tabcontrolItemspanel, "StockList_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));

      System.Windows.Forms.PrintPreviewDialog MyPrintPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
      // MyPrintPreviewDialog.ClientSize = new System.Drawing.Size(990, 630);
      MyPrintPreviewDialog.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      MyPrintPreviewDialog.PrintPreviewControl.Zoom = 1.0;
      // MyPrintPreviewDialog.UseAntiAlias = true;
      //  MyPrintPreviewDialog.Document = lstvwStocklist;
      MyPrintPreviewDialog.ShowDialog();

    }

    #region Text box validation
    private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
      }
      catch
      { }
    }

    private void txtPurchaseprice_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
      }
      catch
      { }
    }

    private void txtSalePrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
      }
      catch
      { }
    }

    private void txtDiscountRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        Regex regex = new Regex("[^0-9.-]+");
        e.Handled = regex.IsMatch(e.Text);
      }
      catch
      { }
    }

    #endregion

    private void btnHomeMenuLink_Click(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      Home go = new Home();
      go.Show();

    }

        private void btnSaveAndReturn_Click(object sender, RoutedEventArgs e)
        {
            if (txtItembarcode.Text == "")
            {
                MessageBox.Show("Please Insert Product Code/ Item Bar-code");
                txtItembarcode.Focus();
            }
            else if (txtitemName.Text == "")
            {
                MessageBox.Show("Please Insert  Product Name");
                txtitemName.Focus();
            }
            else if (txtDiscountRate.Text == "")
            {
                txtDiscountRate.Text = "0";
                txtDiscountRate.Focus();
            }
            else if (txtQty.Text == "")
            {
                MessageBox.Show("Please Insert Product Quantity");
                txtQty.Focus();
            }
            else if (txtPurchaseprice.Text == "")
            {
                MessageBox.Show("Please Insert Product Cost Price / Buy price / purchase Price");
                txtPurchaseprice.Focus();
            }

            else if (txtSalePrice.Text == "")
            {
                MessageBox.Show("Please Insert Product  Sales Price");
                txtSalePrice.Focus();
            }
            else if (cmboCategory.Text == "")
            {
                MessageBox.Show("Please Insert Product Category");
                cmboCategory.Focus();
            }
            else if (cmboLocation.Text == "")
            {
                MessageBox.Show("Please Select Branch name ");
                cmboLocation.Focus();
            }
            else if (cmboSupplier.Text == "")
            {
                MessageBox.Show("Please Select Supplier Name");
                cmboSupplier.Focus();
            }
            else
            {
                try
                {
                    string pid = txtItembarcode.Text;
                    string pname = txtitemName.Text;
                    double quan = Convert.ToDouble(txtQty.Text);
                    double cprice = Convert.ToDouble(txtPurchaseprice.Text);
                    double sprice = Convert.ToDouble(txtSalePrice.Text);

                    double ctotalpri = quan * cprice;
                    double rtotalpri = quan * sprice;
                    double discount = Convert.ToDouble(txtDiscountRate.Text);

                    int taxapply;
                    if (chkTxaApply.IsChecked == true)
                    {
                        taxapply = 1;  //1 = Tax apply
                    }
                    else
                    {
                        taxapply = 0; // 0 = Tax not apply
                    }

                    int kitchenDisplaythisitem;
                    if (chkkitchenDisplay.IsChecked == true)
                    {
                        kitchenDisplaythisitem = 3; // 3 = It's show display on kitchen display
                    }
                    else
                    {
                        kitchenDisplaythisitem = 1; // 1 = it's not show on ditcken display.
                    }

                    //New Insert / New Entry
                    if (lblItemcode.Text == "-")
                    {
                        string imageName = openFileDialog1.FileName;
                        if (lblFileExtension.Text == "item.png")
                        {
                            string sql1 = " insert into purchase (product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, total_retail_price, " +
                                      " category, supplier, discount, taxapply, Shopid, status, description, weight, mdate, edate ) " +
                                       "  Values ( '" + pid + "', '" + pname + "', '" + quan + "', '" + cprice + "', '" + sprice + "', '" + ctotalpri + "', " +
                                       " '" + rtotalpri + "', '" + cmboCategory.Text + "', '" + cmboSupplier.Text + "', '" + discount + "',  " +
                                       " '" + taxapply + "', '" + cmboLocation.SelectedValue + "', '" + kitchenDisplaythisitem + "', '" + txtDescription.Text + "',  " +
                                       "  '" + txtWeight.Text + "', '" + dtMdate.Text + "', '" + dtExpiredate.Text + "' ) ";
                            DataAccess.ExecuteSQL(sql1);
                        }
                        else
                        {
                            string sql1 = " insert into purchase (imagename, product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, total_retail_price, " +
                                      " category, supplier, discount, taxapply, Shopid, status, description, weight, mdate, edate) " +
                                       " SELECT BulkColumn, '" + pid + "', '" + pname + "', '" + quan + "', '" + cprice + "', '" + sprice + "', '" + ctotalpri + "', " +
                                       " '" + rtotalpri + "', '" + cmboCategory.Text + "', '" + cmboSupplier.Text + "', '" + discount + "', " +
                                       "  '" + taxapply + "', '" + cmboLocation.SelectedValue + "', '" + kitchenDisplaythisitem + "', '" + txtDescription.Text + "', " +
                                       "  '" + txtWeight.Text + "', '" + dtMdate.Text + "', '" + dtExpiredate.Text + "' " +
                                       " FROM OPENROWSET ( BULK '" + imageName + "', Single_Blob) as img ";
                            DataAccess.ExecuteSQL(sql1);
                        }

                        MessageBox.Show("Item hase been saved Successfully", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        tabcontrolItemspanel.SelectedItem = tabItemlist;
                        StockitemDatabind("");


                    }
                    else  //Update
                    {

                        string imageName;
                        if (lblFileExtension.Text == "item.png")
                        {
                            string sql = " UPDATE purchase set product_name = '" + txtitemName.Text + "', product_quantity= '" + txtQty.Text + "', " +
                                        " cost_price = '" + txtPurchaseprice.Text + "', retail_price= '" + txtSalePrice.Text + "', total_cost_price = '" + ctotalpri + "', " +
                                        " total_retail_price= '" + rtotalpri + "', category = '" + cmboCategory.Text + "', supplier = '" + cmboSupplier.Text + "', " +
                                        " discount   = '" + discount + "', taxapply = '" + taxapply + "', Shopid = '" + cmboLocation.SelectedValue + "', " +
                                        " status =  '" + kitchenDisplaythisitem + "', description = '" + txtDescription.Text + "', weight =  '" + txtWeight.Text + "', " +
                                        " mdate = '" + dtMdate.Text + "', edate =  '" + dtExpiredate.Text + "' " +
                                        " where product_id = '" + lblItemcode.Text + "' ";
                            DataAccess.ExecuteSQL(sql);
                        }
                        else
                        {
                            imageName = " ( SELECT BulkColumn FROM OPENROWSET ( BULK  '" + openFileDialog1.FileName + "', SINGLE_BLOB) as img ) ";
                            string sql = " UPDATE purchase set  imagename = " + imageName + ", product_name = '" + txtitemName.Text + "', product_quantity= '" + txtQty.Text + "', " +
                                     " cost_price = '" + txtPurchaseprice.Text + "', retail_price= '" + txtSalePrice.Text + "', total_cost_price = '" + ctotalpri + "', " +
                                     " total_retail_price= '" + rtotalpri + "', category = '" + cmboCategory.Text + "', supplier = '" + cmboSupplier.Text + "', " +
                                     " discount   = '" + discount + "' , taxapply = '" + taxapply + "', Shopid = '" + cmboLocation.SelectedValue + "',  " +
                                     " status =  '" + kitchenDisplaythisitem + "', description = '" + txtDescription.Text + "', weight =  '" + txtWeight.Text + "',  " +
                                     " mdate = '" + dtMdate.Text + "', edate =  '" + dtExpiredate.Text + "' " +
                                     " where product_id = '" + lblItemcode.Text + "' ";
                            DataAccess.ExecuteSQL(sql);
                        }



                        //  /////// picture upload  /////////////////      
                        //  //if (openFileDialog1.FileName != string.Empty)
                        //  //{
                        //  //    string destinatiopath = System.AppDomain.CurrentDomain.BaseDirectory + @"\ITEMIMAGE\";
                        //  //    string iName = openFileDialog1.SafeFileName;
                        //  //    string filepath = openFileDialog1.FileName;
                        //  //    System.IO.File.Copy(filepath, destinatiopath + imageName);
                        //  //}
                        ////  
                        tabcontrolItemspanel.SelectedItem = tabItemlist;
                        MessageBox.Show("Successfully Data Updated!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        StockitemDatabind("");
                        lstvwStocklist.Items.Refresh();

                    }

                    this.Visibility = Visibility.Hidden;
                    m_prevWindow.Visibility = Visibility.Visible;
                }
                catch //(Exception exp)
                {
                    // MessageBox.Show("Sorry\r\n" + exp.Message);
                }
            }
        }

        private void btnBarcodeMachine_Click(object sender, RoutedEventArgs e)
        {
            BarcodeMachine go = new BarcodeMachine();
            go.ShowDialog();
        }

        private void btnBarcodeCreator_Click(object sender, RoutedEventArgs e)
        {
            BarcodeCreator go = new BarcodeCreator();
            go.ShowDialog();
        }
    }
}
