using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace PosCube.Reports
{
  /// <summary>
  /// Interaction logic for DueReceive.xaml
  /// </summary>
  public partial class DueReceive : Window
  {
    public DueReceive(string invoiceno, string salesdate, string totalamount, string paidamount, string due, string contact)
    {
      InitializeComponent();
      lblInvoiceNo.Text = invoiceno;
      lblSalesdate.Text = salesdate;
      lbtotalamt.Text = totalamount;
      lbpaidamt.Text = paidamount;
      lbDueAmount.Text = due;
      lbcontact.Text = contact;
      txtReceive.Text = due;

      CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
      ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
      Thread.CurrentThread.CurrentCulture = ci;
      dtReceiveDate.SelectedDate = DateTime.Today;
      //txtReceive.Focus();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (txtReceive.Text == "")
      {
        MessageBox.Show("You are Not able to Update", "Button3 Title", MessageBoxButton.OK, MessageBoxImage.Warning);
      }
      else
      {
        try
        {
          if (Convert.ToDouble(txtReceive.Text) <= Convert.ToDouble(lbDueAmount.Text))
          {
            double Receiveamt = Convert.ToDouble(lbDueAmount.Text) - Convert.ToDouble(txtReceive.Text);
            string sql = "UPDATE sales_payment set due_amount = '" + Receiveamt + "'   where (sales_id = '" + lblInvoiceNo.Text + "')";
            DataAccess.ExecuteSQL(sql);


            double remainingdeu = Convert.ToDouble(lbDueAmount.Text) - Convert.ToDouble(txtReceive.Text);
            string sqlreceivedue = " insert into tbl_duepayment (receivedate, sales_id, totalamt , dueamt, receiveamt , custid) " +
                                    " values ('" + dtReceiveDate.Text + "' , '" + lblInvoiceNo.Text + "', '" + lbtotalamt.Text + "', " +
                                    " '" + remainingdeu + "', '" + txtReceive.Text + "', '" + lbcontact.Text + "') ";
            DataAccess.ExecuteSQL(sqlreceivedue);

            MessageBox.Show("Successfully Data Updated!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            txtReceive.Text = string.Empty;

          }
          else
          {
            MessageBox.Show("You are Not able to Update \n\n Excced Due amount ", "Button3 Title", MessageBoxButton.OK, MessageBoxImage.Warning);
          }

        }
        catch
        {
        }
      }
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
