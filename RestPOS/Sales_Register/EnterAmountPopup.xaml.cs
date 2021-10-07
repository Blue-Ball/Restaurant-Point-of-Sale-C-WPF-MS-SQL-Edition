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

            if(nType == 0)
            {
                txtAmount.Text = "";
                txtAmount.IsReadOnly = true;
            }
            else
            {
                txtAmount.Text = "0.00";
                txtAmount.IsReadOnly = true;
            }
            txtAmount.CaretIndex = txtAmount.Text.Length;
            txtAmount.Focus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            string strTxtAmount = txtAmount.Text;
            if(m_nType == 0)
            {
                if(strTxtAmount.Length <= 1)
                {
                    txtAmount.Text = "";
                }
                else
                {
                    strTxtAmount = strTxtAmount.Substring(0, strTxtAmount.Length - 1);
                    txtAmount.Text = int.Parse(strTxtAmount).ToString();
                }
            }
            else
            {
                txtAmount.Text = string.Format(@"0.{0}{1}", strTxtAmount[0], strTxtAmount[2]);
            }
            txtAmount.CaretIndex = txtAmount.Text.Length;
            txtAmount.Focus();
        }

        private void btn_Num_Click(object sender, RoutedEventArgs e)
        {
            string strNumber = "0";
            switch (((Button)sender).Name)
            {
                case "btn_0":
                    strNumber = "0";
                    break;
                case "btn_1":
                    strNumber = "1";
                    break;
                case "btn_2":
                    strNumber = "2";
                    break;
                case "btn_3":
                    strNumber = "3";
                    break;
                case "btn_4":
                    strNumber = "4";
                    break;
                case "btn_5":
                    strNumber = "5";
                    break;
                case "btn_6":
                    strNumber = "6";
                    break;
                case "btn_7":
                    strNumber = "7";
                    break;
                case "btn_8":
                    strNumber = "8";
                    break;
                case "btn_9":
                    strNumber = "9";
                    break;
            }

            string strTxtAmount = txtAmount.Text;
            if (m_nType == 0)
            {
                strTxtAmount += strNumber;
                txtAmount.Text = int.Parse(strTxtAmount).ToString();
            }
            else
            {
                if(strTxtAmount.Length > 0)
                {
                    if (!strTxtAmount[0].Equals('0'))
                    {
                        strTxtAmount = "0.00";
                    }
                }
                else
                {
                    strTxtAmount = "0.00";
                }
                txtAmount.Text = string.Format(@"{0}.{1}{2}", strTxtAmount[2], strTxtAmount[3], strNumber);
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
            DialogResult = false;
        }
    }
}
