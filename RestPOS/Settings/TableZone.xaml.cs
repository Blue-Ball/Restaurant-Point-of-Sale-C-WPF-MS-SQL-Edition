using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace PosCube.Settings
{
  /// <summary>
  /// Interaction logic for Category.xaml
  /// </summary>
  public partial class TableZone : UserControl
  {
    ResourceManager res_man;
    CultureInfo cul;
    public TableZone()
    {
      InitializeComponent();
    }

    public void TableZonebind()
    {
      string sql = " select id,  tablename as 'Table', zonenam as 'Zone Name', seatqty as 'Seat', orderno as 'Sorting', fillcolor as 'Back color' " +
                  " from tbl_tablezone where status = 1 order by id desc ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);
      datagridTableZone.ItemsSource = dt1.DefaultView;
      lblMsg.Text = datagridTableZone.Items.Count.ToString() + " Found";
      datagridTableZone.Columns[0].Width = 32;
      datagridTableZone.Columns[1].Width = 82;
    }

    private void TableZone_form_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        TableZonebind();
        btnAddNew.Visibility = Visibility.Hidden;
        this.colorList.ItemsSource = typeof(Brushes).GetProperties();
        switch_language();
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

        lbltablezonetitle.Text = res_man.GetString("lbltablezonetitle", cul);
        lbltablezonenametitle.Text = res_man.GetString("lbltablezonenametitle", cul);
        lblzonenametitle.Text = res_man.GetString("lblzonenametitle", cul);
        lblseattitle.Text = res_man.GetString("lblseattitle", cul);
        lblsorttitle.Text = res_man.GetString("lblsorttitle", cul);
        lblbgcolortitle.Text = res_man.GetString("lblbgcolortitle", cul);

        btnSave.Content = res_man.GetString("btnSave", cul);
        btnAddNew.Content = res_man.GetString("btnAddnew", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (txtTableName.Text == "")
        {
          MessageBox.Show("Please Fill  Table Name");
          txtTableName.Focus();
        }
        if (txtzonename.Text == "")
        {
          MessageBox.Show("Please Fill  Zone Name");
          txtzonename.Focus();
        }
        if (txtsortingorder.Text == "")
        {
          MessageBox.Show("Please Fill Sorting Order");
          txtsortingorder.Focus();
        }
        else
        {
          if (lblID.Text == "-")
          {
            string sqlCmd = " insert into tbl_tablezone (tablename, zonenam , seatqty, orderno, fillcolor) " +
                            "  values ('" + txtTableName.Text + "' , '" + txtzonename.Text + "', '" + txtseatqty.Text + "', " +
                            " '" + txtsortingorder.Text + "', '" + rtlfill.Fill + "')";
            DataAccess.ExecuteSQL(sqlCmd);
            txtTableName.Text = "";
            TableZonebind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = "Successfully saved";
          }
          else  //Update 
          {
            string sqlUpdateCmd = " update tbl_tablezone set tablename = '" + txtTableName.Text + "', zonenam = '" + txtzonename.Text + "', " +
                                    " seatqty = '" + txtseatqty.Text + "', orderno =  '" + txtsortingorder.Text + "', " +
                                    " fillcolor = '" + rtlfill.Fill + "'    where id = '" + lblID.Text + "'";
            DataAccess.ExecuteSQL(sqlUpdateCmd);
            TableZonebind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = lblID.Text + " Successfully Update";
            txtTableName.Text = string.Empty;
            txtzonename.Text = string.Empty;
            lblID.Text = "-";
            btnAddNew.Visibility = Visibility.Hidden;
          }
        }

      }
      catch (Exception exp)
      {
        MessageBox.Show("Sorry\r\n this id already added \n\n " + exp.Message);
      }
    }

    private void btnAddnew_Click(object sender, RoutedEventArgs e)
    {
      txtTableName.Text = string.Empty;
      txtzonename.Text = string.Empty;
      txtseatqty.Text = string.Empty;
      txtsortingorder.Text = string.Empty;
      lblID.Text = "-";
      btnAddNew.Visibility = Visibility.Hidden;

    }

    private void grdvwbtnDeleteRows_Click(object sender, RoutedEventArgs e)
    {
      DataRowView drv = (DataRowView)datagridTableZone.SelectedItem;
      int i = datagridTableZone.CurrentCell.Column.DisplayIndex;
      for (i = 0; i < datagridTableZone.SelectedItems.Count; i++)
      {
        if (MessageBox.Show("Do you want to Delete?", "Yes or No", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {

          string sqldel = " delete from tbl_tablezone     where ID = '" + drv.Row[0].ToString() + "'";
          DataAccess.ExecuteSQL(sqldel);
          TableZonebind();
        }
      }
    }

    private void datagridTableZone_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      try
      {

        DataRowView dataRow = (DataRowView)datagridTableZone.SelectedItem;
        int index = datagridTableZone.CurrentCell.Column.DisplayIndex;
        lblID.Text = dataRow.Row.ItemArray[0].ToString();
        txtTableName.Text = dataRow.Row.ItemArray[1].ToString();
        txtzonename.Text = dataRow.Row.ItemArray[2].ToString();
        txtseatqty.Text = dataRow.Row.ItemArray[3].ToString();
        txtsortingorder.Text = dataRow.Row.ItemArray[4].ToString();

        btnAddNew.Visibility = Visibility.Visible;
        var converter = new System.Windows.Media.BrushConverter();
        var brush = (Brush)converter.ConvertFromString(dataRow.Row.ItemArray[5].ToString());
        rtlfill.Fill = brush;
        //  colorList.SelectedItem = dataRow.Row.ItemArray[5].ToString();
      }
      catch
      {

      }
    }

    private void colorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Brush selectedColor = (Brush)(e.AddedItems[0] as PropertyInfo).GetValue(null, null);
      rtlfill.Fill = selectedColor;
    }

  }
}
