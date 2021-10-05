using Microsoft.Win32;
using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PosCube.Items
{
  /// <summary>
  /// Interaction logic for Items_Alert.xaml
  /// </summary>
  public partial class Items_Alert : UserControl
  {
    ResourceManager res_man;
    CultureInfo cul;

    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    public Items_Alert()
    {
      InitializeComponent();
    }

    private void Items_alert_Form_Loaded(object sender, RoutedEventArgs e)
    {
      try { switch_language(); }
      catch { }
    }


    private void btnLowQuantityItem_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        string sql = " select  product_id as 'Item Code' , product_name as 'Item Name' , product_quantity as 'Quantity',  " +
                     " cost_price as 'Buy Price' , retail_price as 'Sales Price' ,   category as 'Category' , supplier as 'Supplier'   " +
                     " from purchase where product_quantity <= 15   order by product_quantity ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgrdviewitemsList.ItemsSource = dt1.DefaultView;
        lblitems.Text = "Low Quantity Items " + dtgrdviewitemsList.Items.Count.ToString();

      }
      catch
      {
      }
    }

    private void btnExpiredItem_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        string sql = " select  product_id as 'Item Code' , product_name as 'Item Name' ,  " +
                " retail_price as 'Sales Price' , product_quantity as 'Quantity', cost_price as 'Buy Price' ,  " +
                " category as 'Category' , supplier as 'Supplier',  Shopid as 'Location', weight , edate as 'Expired' " +
                " from purchase where edate <  '" + DateTime.Now.ToString("yyyy-MM-dd") + "'   and edate <> ''   order by edate ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgrdviewitemsList.ItemsSource = dt1.DefaultView;
        lblitems.Text = "Expired Items " + dtgrdviewitemsList.Items.Count.ToString();
      }
      catch
      {
      }
    }

    private void btnExpiringItem_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        DateTime dtd = DateTime.Now;
        dtd = dtd.AddDays(+30);

        string sql = " select  product_id as 'Item Code' , product_name as 'Item Name' ,  " +
                " retail_price as 'Sales Price' , product_quantity as 'Quantity', cost_price as 'Buy Price' ,  " +
                " category as 'Category' , supplier as 'Supplier',  Shopid as 'Location', weight , edate as 'Expire Date' " +
                " from purchase where edate <  '" + dtd.ToString("yyyy-MM-dd") + "'   and edate <> ''  order by edate ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        dtgrdviewitemsList.ItemsSource = dt1.DefaultView;
        lblitems.Text = "Expiring next 30 days " + dtgrdviewitemsList.Items.Count.ToString();
      }
      catch
      {
      }
    }


    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //Exporting to xls.     
        saveFileDialog1.FileName = lblitems.Text + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xls";
        saveFileDialog1.ShowDialog();

        //string fileName = "LowQuantity_Items_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xls";
        //string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //string destFile = System.IO.Path.Combine(targetPath, fileName);

        dtgrdviewitemsList.SelectAllCells();
        dtgrdviewitemsList.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dtgrdviewitemsList);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dtgrdviewitemsList.UnselectAllCells();
        string name = saveFileDialog1.FileName;
        System.IO.StreamWriter file1 = new System.IO.StreamWriter(name);
        file1.WriteLine(result.Replace(',', ' '));
        file1.Close();
      }
      catch
      {

      }




    }

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);

        btnLowQuantityItem.Content = res_man.GetString("btnLowQuantityItem", cul);
        btnExpiredItem.Content = res_man.GetString("btnExpiredItem", cul);
        btnExpiringItem.Content = res_man.GetString("btnExpiringItem", cul);
        btnExport.Content = res_man.GetString("btnExport", cul);

      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }

  }
}
