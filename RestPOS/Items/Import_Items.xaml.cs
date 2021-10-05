using Microsoft.Win32;
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;

namespace PosCube.Items
{
  /// <summary>
  /// Interaction logic for Import_Items.xaml
  /// </summary>
  public partial class Import_Items : UserControl
  {
    private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
    // private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.8.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
    private string Excel10ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";

    OpenFileDialog openFileDialog1 = new OpenFileDialog();

    public Import_Items()
    {
      InitializeComponent();
      btnSave.IsEnabled = false;
    }


    private void btnImportPreview_Click(object sender, RoutedEventArgs e)
    {
      //openFileDialog1.ShowDialog();
      //   lblRows.Text = "Total ID = " + dtgridviewImportPreview.RowCount.ToString();
      openFileDialog1.CheckFileExists = true;
      openFileDialog1.CheckPathExists = true;

      openFileDialog1.DefaultExt = ".xls";
      // openFileDialog1.Filter = "GIF files (*.gif)|*.gif| jpg files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
      openFileDialog1.Filter = "Excel files (*.xls)|*.XLS";

      openFileDialog1.FilterIndex = 2;
      openFileDialog1.RestoreDirectory = true;



      if (openFileDialog1.ShowDialog() == true)
      {
        ////  lblFileExtension.Text =  openFileDialog.FileName;
        //picItemimage.Source = new BitmapImage(new Uri(openFileDialog1.FileName));
        ////// textBox1.Text = openFileDialog1.FileName;
        ////picItemimage.Source = openFileDialog1.FileName;
        //lblFileExtension.Text = System.IO.Path.GetExtension(openFileDialog1.FileName);
        string filePath = openFileDialog1.FileName;
        string extension = System.IO.Path.GetExtension(filePath);
        string header = "YES";
        string conStr, sheetName;

        conStr = string.Empty;
        switch (extension)
        {

          case ".xls": //Excel 97-03
            conStr = string.Format(Excel03ConString, filePath, header);
            break;

          case ".xlsx": //Excel 07
            conStr = string.Format(Excel10ConString, filePath, header);
            break;

          case ".csv": //Excel 07
            conStr = string.Format(Excel10ConString, filePath, header);
            break;
        }

        //Get the name of the First Sheet.
        using (OleDbConnection con = new OleDbConnection(conStr))
        {
          using (OleDbCommand cmd = new OleDbCommand())
          {
            cmd.Connection = con;
            con.Open();
            DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            con.Close();
          }
        }

        //Read Data from the First Sheet.
        using (OleDbConnection con = new OleDbConnection(conStr))
        {
          using (OleDbCommand cmd = new OleDbCommand())
          {
            using (OleDbDataAdapter oda = new OleDbDataAdapter())
            {
              DataTable dt = new DataTable();
              cmd.CommandText = "SELECT * From [" + sheetName + "]";
              cmd.Connection = con;
              con.Open();
              oda.SelectCommand = cmd;
              oda.Fill(dt);
              con.Close();

              ////Populate DataGridView.
              dtgridviewImportPreview.ItemsSource = dt.DefaultView;
              btnSave.IsEnabled = true;
            }
          }
        }
        lblRows.Text = "Total Items = " + dtgridviewImportPreview.Items.Count.ToString();
      }


    }

    private void btnDataSample_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        // System.Diagnostics.Process.Start("Calc");
        // SendKeys.SendWait(lblTotal.Text);
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "items.xls";
        p.Start();
        p.WaitForInputIdle();

      }
      catch
      {
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      lblwaiting.Visibility = Visibility.Visible;
      lblwaiting.Text = "Please Wait ...";
      try
      {
        DataTable dt = new DataTable();
        dt = ((DataView)dtgridviewImportPreview.ItemsSource).ToTable();
        int rows = dtgridviewImportPreview.Items.Count;
        for (int i = 0; i < rows; i++)
        {
          string product_id = dt.Rows[i][0].ToString();
          string product_name = dt.Rows[i][1].ToString();
          decimal product_quantity = Convert.ToDecimal(dt.Rows[i][2].ToString());
          decimal cost_price = Convert.ToDecimal(dt.Rows[i][3].ToString());
          decimal retail_price = Convert.ToDecimal(dt.Rows[i][4].ToString());
          decimal total_cost_price = cost_price * product_quantity;
          decimal total_retail_price = retail_price * product_quantity;
          string category = dt.Rows[i][5].ToString();
          string supplier = dt.Rows[i][6].ToString();
          // string imagename            = "item.png";   //dt.Rows[i][7].ToString();
          decimal discount = Convert.ToDecimal(dt.Rows[i][7].ToString());
          int taxapply = Convert.ToInt32(dt.Rows[i][8].ToString());
          string Shopid = dt.Rows[i][9].ToString();
          int kitchendisplay = Convert.ToInt32(dt.Rows[i][10].ToString());
          string description = dt.Rows[i][11].ToString();
          string weight = dt.Rows[i][12].ToString();
          string mdate = dt.Rows[i][13].ToString();
          string edate = dt.Rows[i][14].ToString();

          string sqlCmd = " insert into purchase (product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, " +
                          " total_retail_price, category, supplier,  discount, taxapply, Shopid, status, description , weight, mdate, edate ) " +
                          "  values ('" + product_id + "', '" + product_name + "', '" + product_quantity + "', '" + cost_price + "', '" + retail_price + "', " +
                          " '" + total_cost_price + "', '" + total_retail_price + "', '" + category + "', '" + supplier + "', " +
                          " '" + discount + "', '" + taxapply + "', '" + Shopid + "', '" + kitchendisplay + "', " +
                          " '" + description + "', '" + weight + "', '" + mdate + "', '" + edate + "')";
          DataAccess.ExecuteSQL(sqlCmd);
          lblwaiting.Visibility = Visibility.Hidden;
          lblmsg.Text = "Successfully Added ";
          btnSave.IsEnabled = false;
        }
      }
      catch (Exception exp)
      {
        MessageBox.Show("Sorry\r\n this id already added \n Duplicate value \n  " + exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

      }
    }
  }
}
