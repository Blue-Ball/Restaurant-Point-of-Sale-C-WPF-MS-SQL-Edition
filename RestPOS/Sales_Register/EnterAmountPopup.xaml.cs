using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

        public double m_fWeight;
        public string m_strUnit;
        public double m_fPrice;
        public double m_fWeightScale;
        SerialPort m_serialPort;

        public EnterAmountPopup(int nType)
        {
            InitializeComponent();

            Topmost = true;
            m_nType = nType;
            txtAmount.Text = "";
            m_fWeightScale = 0.0;

            if (nType == 0)
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
            m_serialPort = null;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (m_nType == 2)
                return;

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
            if (m_nType == 2)
                return;

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
            if(m_nType == 2)
            {
                if (m_serialPort != null && m_serialPort.IsOpen)
                    m_serialPort.Close();
                if (m_serialPort != null)
                    m_serialPort.Dispose();
            }
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (m_nType == 2)
            {
                if (m_serialPort != null && m_serialPort.IsOpen)
                    m_serialPort.Close();
                if (m_serialPort != null)
                    m_serialPort.Dispose();
            }

            DialogResult = false;
        }

        public void OpenWeightScale()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");

            string port = data["COM"]["WeightScale"];
            if (!port.Equals(""))
            {
                using (m_serialPort = new SerialPort(port))
                {
                    m_serialPort.Close();

                    m_serialPort.BaudRate = 9600;     // Pole Bound Rate 
                    m_serialPort.Parity = System.IO.Ports.Parity.None;
                    m_serialPort.DataBits = 8;   // Data Bits
                    m_serialPort.StopBits = System.IO.Ports.StopBits.One;

                    m_serialPort.DataReceived += SerialPortOnDataReceived;       //<-- this event happens everytime when new data is received by the ComPort
                    m_serialPort.Open();     //<-- make the comport listen
                }
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if(m_nType == 2)
            {
                if(m_fWeight > 0)
                {
                    txtAmount.Text = (m_fPrice * (m_fWeightScale/m_fWeight)).ToString();
                }
                
                this.Title = "Weight = " + m_fWeightScale + m_strUnit;

                OpenWeightScale();
            }
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            int dataLength = m_serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = m_serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;
            string str = System.Text.Encoding.UTF8.GetString(data);
            string numericValue = new String(str.Where(Char.IsDigit).ToArray());
            m_fWeightScale = double.Parse(numericValue);
            m_strUnit = new String(str.Where(Char.IsLetter).ToArray());

            if (m_fWeight > 0)
            {
                txtAmount.Text = (m_fPrice * (m_fWeightScale / m_fWeight)).ToString();
            }

            this.Title = "Weight = " + m_fWeightScale + m_strUnit;
        }
    }
}
