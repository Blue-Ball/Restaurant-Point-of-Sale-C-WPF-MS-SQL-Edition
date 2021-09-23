using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace RestPOS.Items
{
  /// <summary>
  /// Interaction logic for Category.xaml
  /// </summary>
  public partial class Category : UserControl
  {
    public Category()
    {
      InitializeComponent();
    }

    public void categorybind()
    {
      string sql = " select ID, category_name as 'Category' from tbl_category order by ID desc ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);
      datagridcategories.ItemsSource = dt1.DefaultView;
      lblMsg.Text = datagridcategories.Items.Count.ToString() + " Found";
      datagridcategories.Columns[0].Width = 32;
      datagridcategories.Columns[1].Width = 52;
    }

    private void Category_form_Loaded_1(object sender, RoutedEventArgs e)
    {
      try
      {
        categorybind();

      }
      catch
      {

      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (txtCategoryName.Text == "")
        {
          MessageBox.Show("Please Fill  Category Name");
          txtCategoryName.Focus();
        }
        else
        {
          if (lblID.Text == "-")
          {
            string sqlCmd = " insert into tbl_category (category_name)  values ('" + txtCategoryName.Text + "'  )";
            DataAccess.ExecuteSQL(sqlCmd);
            txtCategoryName.Text = "";
            categorybind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = "Successfully saved";
          }
          else  //Update 
          {
            string sqlUpdateCmd = " update tbl_category set category_name = '" + txtCategoryName.Text + "'   where ID = '" + lblID.Text + "'";
            DataAccess.ExecuteSQL(sqlUpdateCmd);
            categorybind();
            lblMsg.Visibility = Visibility.Visible;
            lblMsg.Text = lblID.Text + " Successfully Update";
            txtCategoryName.Text = string.Empty;
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
      DataRowView drv = (DataRowView)datagridcategories.SelectedItem;
      //  drv.Row.Delete();
      int i = datagridcategories.CurrentCell.Column.DisplayIndex;
      for (i = 0; i < datagridcategories.SelectedItems.Count; i++)
      {
        if (MessageBox.Show("Do you want to Delete?", "Yes or No", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {

          string sqldel = " delete from tbl_category     where ID = '" + drv.Row[0].ToString() + "'";
          DataAccess.ExecuteSQL(sqldel);
          categorybind();
        }
      }
    }

    private void datagridcategories_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      try
      {

        DataRowView dataRow = (DataRowView)datagridcategories.SelectedItem;
        int index = datagridcategories.CurrentCell.Column.DisplayIndex;
        lblID.Text = dataRow.Row.ItemArray[0].ToString();
        txtCategoryName.Text = dataRow.Row.ItemArray[1].ToString();
      }
      catch
      {

      }
    }
  }
}
