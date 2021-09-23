using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RestPOS.Reports
{
  /// <summary>
  /// Interaction logic for DueReport.xaml
  /// </summary>
  public partial class DueReport : UserControl
  {
    DispatcherTimer autoupdate = new DispatcherTimer();
    public DueReport()
    {
      InitializeComponent();
    }

    public void listDatabind()
    {
      string sql = " select  sales_id as 'Invoice No' , sales_time as Date , payment_amount as Total , " +
                      " (payment_amount - due_amount) as 'Paid Amount' ,  payment_type as 'Payment Type' , " +
                      "  due_amount as Due, emp_id as 'Sold by' ,   C_id  as CustID , Comment as 'Comment' " +
                      " from sales_payment where due_amount !='0'  ";
      DataAccess.ExecuteSQL(sql);
      DataTable dt1 = DataAccess.GetDataTable(sql);
      datagridDueList.ItemsSource = dt1.DefaultView;
    }

    private void DueReport_form_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        listDatabind();

        autoupdate.Tick += new EventHandler(autoupdate_Tick);
        autoupdate.Interval = new TimeSpan(0, 0, 10);
        autoupdate.Start();
      }
      catch
      {

      }
    }

    ////  Synchronization 
    private void autoupdate_Tick(object sender, EventArgs e)
    {
      try
      {
        listDatabind();
      }
      catch
      {

      }
    }

    private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        autoupdate.Stop();
        string sql = "select  sales_id as 'Invoice No' , sales_time as Date , payment_amount as Total ,  " +
                     " (payment_amount - due_amount) as 'Paid Amount' , payment_type as 'Payment Type' , " +
                     "  due_amount as Due, emp_id as 'Sold by' ,    C_id  as Contact , Comment as 'Comment' " +
                     "  from sales_payment where (sales_id like '" + txtsearch.Text + "%' and due_amount !='0' ) " +
                     "  OR ( C_id like '" + txtsearch.Text + "%' and due_amount !='0' ) ";
        DataAccess.ExecuteSQL(sql);
        DataTable dt1 = DataAccess.GetDataTable(sql);
        datagridDueList.ItemsSource = dt1.DefaultView;


      }
      catch
      {
      }
    }

    //View Due details
    private void datagridDueList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      try
      {
        DataRowView dataRow = (DataRowView)datagridDueList.SelectedItem;
        int index = datagridDueList.CurrentCell.Column.DisplayIndex;

        string Salesid = dataRow.Row.ItemArray[0].ToString();
        string salesdate = dataRow.Row.ItemArray[1].ToString();
        string totalamount = dataRow.Row.ItemArray[2].ToString();
        string paidamount = dataRow.Row.ItemArray[3].ToString();
        string due = dataRow.Row.ItemArray[5].ToString();
        string contact = dataRow.Row.ItemArray[7].ToString();

        // Window win = new Window();
        Reports.DueReceive go = new Reports.DueReceive(Salesid, salesdate, totalamount, paidamount, due, contact);
        //  win.Content = go;
        //  win.Title = "User Control1";
        go.ShowDialog();

        autoupdate.Start();
        //  this.DueReceive.Visibility = Visibility.Visible;
        // go.Show();
      }
      catch
      {

      }

    }
  }
}
