using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PosCube.Items
{
  /// <summary>
  /// Interaction logic for Category.xaml
  /// </summary>
  public partial class Sauceoptions : UserControl
  {
    ResourceManager res_man;
    CultureInfo cul;
    public Sauceoptions()
    {
      InitializeComponent();
    }

    public void Sauceoptionsbind()
    {
      string sql = " select ID, saucename as 'Option Name', bgcolor as 'Back Color' from tbl_sauceoptions order by id desc ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);
      datagridsauceoptions.ItemsSource = dt1.DefaultView;
      lblMsg.Text = datagridsauceoptions.Items.Count.ToString() + " Found";
      datagridsauceoptions.Columns[0].Width = 32;
      datagridsauceoptions.Columns[1].Width = 52;
      datagridsauceoptions.Columns[2].Width = 242;
    }

    private void Sauceoptions_form_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        Sauceoptionsbind();
        this.colorList.ItemsSource = typeof(Brushes).GetProperties();
        switch_language();
      }
      catch
      {

      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (txtOptionsName.Text == "")
        {
          MessageBox.Show("Please Fill  Options Name");
          txtOptionsName.Focus();
        }
        else
        {
          if (lblID.Text == "-")
          {
            string sqlCmd = " insert into tbl_sauceoptions (saucename, bgcolor)  values ('" + txtOptionsName.Text + "', '" + rtlfill.Fill + "'  )";
            DataAccess.ExecuteSQL(sqlCmd);
            txtOptionsName.Text = "";
            Sauceoptionsbind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = "Successfully saved";
          }
          else  //Update 
          {
            string sqlUpdateCmd = " update tbl_sauceoptions set saucename = '" + txtOptionsName.Text + "', bgcolor = '" + rtlfill.Fill + "' " +
                                   "   where id = '" + lblID.Text + "'";
            DataAccess.ExecuteSQL(sqlUpdateCmd);
            Sauceoptionsbind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = lblID.Text + " Successfully Update";
            txtOptionsName.Text = string.Empty;
            lblID.Text = "-";
          }
        }

      }
      catch (Exception exp)
      {
        MessageBox.Show("Sorry\r\n this id already added \n\n " + exp.Message);
      }
    }

    private void grdvwbtnDeleteRows_Click(object sender, RoutedEventArgs e)
    {
      DataRowView drv = (DataRowView)datagridsauceoptions.SelectedItem;
      //  drv.Row.Delete();
      int i = datagridsauceoptions.CurrentCell.Column.DisplayIndex;
      for (i = 0; i < datagridsauceoptions.SelectedItems.Count; i++)
      {
        if (MessageBox.Show("Do you want to Delete?", "Yes or No", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {

          string sqldel = " delete from tbl_sauceoptions     where ID = '" + drv.Row[0].ToString() + "'";
          DataAccess.ExecuteSQL(sqldel);
          Sauceoptionsbind();
        }
      }
    }

    private void datagridsauceoptions_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      try
      {

        DataRowView dataRow = (DataRowView)datagridsauceoptions.SelectedItem;
        int index = datagridsauceoptions.CurrentCell.Column.DisplayIndex;
        lblID.Text = dataRow.Row.ItemArray[0].ToString();
        txtOptionsName.Text = dataRow.Row.ItemArray[1].ToString();
        var converter = new System.Windows.Media.BrushConverter();
        var brush = (Brush)converter.ConvertFromString(dataRow.Row.ItemArray[2].ToString());
        rtlfill.Fill = brush;
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

    private void switch_language()
    {
      res_man = new ResourceManager("PosCube.Resource.Res", typeof(Home).Assembly);
      if (language.ID == "1")
      {
        cul = CultureInfo.CreateSpecificCulture(language.languagecode);
        lblinserttitle.Text = res_man.GetString("lblinserttitle", cul);
        lblbackgrouncolortitle.Text = res_man.GetString("lblbackgrouncolortitle", cul);
        btnSave.Content = res_man.GetString("btnSave", cul);
      }
      else
      {
        // englishToolStripMenuItem.Checked = true;
      }
    }
  }
}
