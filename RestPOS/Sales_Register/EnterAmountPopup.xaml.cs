using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PosCube.Sales_Register
{
    /// <summary>
    /// Interaction logic for EnterAmountPopup.xaml
    /// </summary>
    public partial class EnterAmountPopup : Window
    {
        public int m_nType; // 0: QTY, 1: ChangePrice, 2: Weight, 3: Discount

        public EnterAmountPopup(int nType)
        {
            InitializeComponent();

            Topmost = true;
            m_nType = nType;
            txtAmount.Text = "";
            txtAmount.Focus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(txtAmount.Text.Length > 0)
                txtAmount.Text = txtAmount.Text.Substring(0, txtAmount.Text.Length-1);
            txtAmount.CaretIndex = txtAmount.Text.Length;
            txtAmount.Focus();
        }

        private void btn_Num_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btn_0":
                    txtAmount.Text += "0";
                    break;
                case "btn_1":
                    txtAmount.Text += "1";
                    break;
                case "btn_2":
                    txtAmount.Text += "2";
                    break;
                case "btn_3":
                    txtAmount.Text += "3";
                    break;
                case "btn_4":
                    txtAmount.Text += "4";
                    break;
                case "btn_5":
                    txtAmount.Text += "5";
                    break;
                case "btn_6":
                    txtAmount.Text += "6";
                    break;
                case "btn_7":
                    txtAmount.Text += "7";
                    break;
                case "btn_8":
                    txtAmount.Text += "8";
                    break;
                case "btn_9":
                    txtAmount.Text += "9";
                    break;
            }
            txtAmount.CaretIndex = txtAmount.Text.Length;
            txtAmount.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = "";
            txtAmount.Focus();
        }
    }
}
