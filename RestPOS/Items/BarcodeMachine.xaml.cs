using System;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace PosCube.Items
{
    /// <summary>
    /// Interaction logic for BarcodeMachine.xaml
    /// </summary>
    public partial class BarcodeMachine : Window
    {
        public BarcodeMachine()
        {
            InitializeComponent();
        }

        public void RefreshBarcode()
        {
            try
            {
                string barcodeType = ((ComboBoxItem)cmbBarcodeType.SelectedItem).Content.ToString();
                int nFontSize = int.Parse(txtFontSize.Text);
                System.Drawing.Color cTextColor = System.Drawing.Color.FromName(((ComboBoxItem)cmbForeColor.SelectedItem).Content.ToString());

                var barcodeWriter = new BarcodeWriter
                {
                    Renderer = new BitmapRenderer
                    {
                        Foreground = cTextColor,
                        TextFont = new System.Drawing.Font(((ComboBoxItem)cmbFontFamily.SelectedItem).Content.ToString(), nFontSize)
                    },
                    Format = (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), barcodeType),
                    Options = new EncodingOptions
                    {
                        Height = int.Parse(txtBarHeight.Text),
                        Width = 274,
                        Margin = 1
                    }
                };

                System.Drawing.Bitmap bmp = barcodeWriter.Write(txtBarcode.Text);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                imgBarcode.Height = int.Parse(txtBarHeight.Text);
                imgBarcode.Source = bi;

                if(chkShowText.IsChecked == true)
                {
                    panelTopText.Visibility = Visibility.Visible;
                    lblBarcodeCompany.Content = txtCompanyName.Text;
                    txtBarcodeDescription.Text = txtDescription.Text;
                    lblBarcodePrice.Content = string.Format(@"Price: {0}{1}", txtCurrency.Text, txtPrice.Text);
                }
                else
                {
                    panelTopText.Visibility = Visibility.Collapsed;
                }

                if(chkShowBorder.IsChecked == true)
                {
                    borderGrid.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderGrid.BorderThickness = new Thickness(0);
                }

                borderGrid.UpdateLayout();
                SaveToPng(gridBarcode, "barcode.png");
            }
            catch// (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private void txtBarcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if((sender as TextBox).Text != null)
                {
                    RefreshBarcode();
                }
            }
            catch
            {

            }
            
        }

        private void cmbProductCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string strTemp = ((DataRowView)cmbProductCode.SelectedItem).Row.ItemArray[0].ToString();
                string sql5 = "select product_id , product_name , retail_price  from purchase where product_id = '" + strTemp + "' ";
                DataAccess.ExecuteSQL(sql5);
                DataTable dt5 = DataAccess.GetDataTable(sql5);
                txtBarcode.Text = dt5.Rows[0].ItemArray[0].ToString();
                txtDescription.Text = dt5.Rows[0].ItemArray[1].ToString() + "\nIngredients:";
                txtPrice.Text = dt5.Rows[0].ItemArray[2].ToString();
            }
            catch
            {

            }
        }

        private void txtFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void cmbBarcodeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void txtCompanyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void txtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void cmbForeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void txtBarHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }

        private void txtDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshBarcode();
        }


        private void chkShowText_Checked(object sender, RoutedEventArgs e)
        {
            RefreshBarcode();
        }

        public void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        public void SaveToPng(FrameworkElement grid, string fileName)
        {
            try
            {
                Rect bounds = VisualTreeHelper.GetDescendantBounds(grid);
                if(bounds != Rect.Empty)
                {
                    bounds.Height += 1;
                    bounds.Size = new Size(bounds.Size.Width, bounds.Size.Height + 1);

                    RenderTargetBitmap renderTarget = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

                    DrawingVisual visual = new DrawingVisual();

                    using (DrawingContext context = visual.RenderOpen())
                    {
                        VisualBrush visualBrush = new VisualBrush(grid);
                        context.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
                    }

                    renderTarget.Render(visual);
                    PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

                    using (Stream stm = File.Create(fileName))
                    {
                        bitmapEncoder.Save(stm);
                    }
                }
            }
            catch
            {

            }
        }

        public void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                stream.Position = 0;
                encoder.Save(stream);
            }
        }

        private void btnCreateBarcode_Click(object sender, RoutedEventArgs e)
        {
            printBarcode0x0.Width = borderGrid.Width;printBarcode0x0.Height = borderGrid.Height;
            printBarcode0x1.Width = borderGrid.Width; printBarcode0x1.Height = borderGrid.Height;
            printBarcode1x0.Width = borderGrid.Width; printBarcode1x0.Height = borderGrid.Height;
            printBarcode1x1.Width = borderGrid.Width; printBarcode1x1.Height = borderGrid.Height;
            printBarcode2x0.Width = borderGrid.Width; printBarcode2x0.Height = borderGrid.Height;
            printBarcode2x1.Width = borderGrid.Width; printBarcode2x1.Height = borderGrid.Height;

            string barcodeimagestore = System.AppDomain.CurrentDomain.BaseDirectory + @"\barcode.png";
            var image = new BitmapImage();
            var stream = File.OpenRead(barcodeimagestore);

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            stream.Close();
            stream.Dispose();
            image.Freeze();

            printBarcode0x0.Source = image;
            printBarcode0x1.Source = image;
            printBarcode1x0.Source = image;
            printBarcode1x1.Source = image;
            printBarcode2x0.Source = image;
            printBarcode2x1.Source = image;

            if (chkShowBorder.IsChecked == true)
            {
                printBorder0x0.BorderThickness = new Thickness(2);
                printBorder0x1.BorderThickness = new Thickness(2);
                printBorder1x0.BorderThickness = new Thickness(2);
                printBorder1x1.BorderThickness = new Thickness(2);
                printBorder2x0.BorderThickness = new Thickness(2);
                printBorder2x1.BorderThickness = new Thickness(2);
            }
            else
            {
                printBorder0x0.BorderThickness = new Thickness(0);
                printBorder0x1.BorderThickness = new Thickness(0);
                printBorder1x0.BorderThickness = new Thickness(0);
                printBorder1x1.BorderThickness = new Thickness(0);
                printBorder2x0.BorderThickness = new Thickness(0);
                printBorder2x1.BorderThickness = new Thickness(0);
            }
        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("barcode.png");
        }


        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(grdPrintPanel);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(grdPrintPanel);
                context.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            PrinterSettings settings = new PrinterSettings();
            System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            var printers = new LocalPrintServer().GetPrintQueues();
            var selectedPrinter = printers.FirstOrDefault(p => p.Name == settings.PrinterName); //Replacement 
            //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            if (selectedPrinter != null)
            {
                Printdlg.PrintQueue = selectedPrinter;
                System.Drawing.Printing.PaperSize pkCustomSize1 = new System.Drawing.Printing.PaperSize("My Envelope", 680, 1350);
                Printdlg.MaxPage = 10;
                Printdlg.PrintVisual(visual, "Bacode_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));
            }

            //renderTarget.Render(visual);
            //PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            //using (Stream stm = File.Create(@"print_barcode.png"))
            //{
            //    bitmapEncoder.Save(stm);
            //}

            //PrinterSettings settings = new PrinterSettings();
            //PrintDocument pd = new PrintDocument();
            //PrintDialog printDialog = new PrintDialog();
            //var printers = new LocalPrintServer().GetPrintQueues();
            //var selectedPrinter = printers.FirstOrDefault(p => p.Name == settings.PrinterName); //Replacement 
            //// if (printDialog.ShowDialog() == true)
            //{
            //    printDialog.PrintQueue = selectedPrinter;
            //    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            //    pd.Print();
            //}
        }

        private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //var image = new BitmapImage();
            //var stream = File.OpenRead("print_barcode.png");

            //image.BeginInit();
            //image.CacheOption = BitmapCacheOption.OnLoad;
            //image.StreamSource = stream;
            //image.EndInit();
            //stream.Close();
            //stream.Dispose();
            //image.Freeze();

            System.Drawing.Image image = System.Drawing.Image.FromFile("print_barcode.png");
            e.Graphics.DrawImage(image, 0, 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql5 = "select product_id from purchase ";
                DataAccess.ExecuteSQL(sql5);
                DataTable dt5 = DataAccess.GetDataTable(sql5);
                cmbProductCode.ItemsSource = dt5.DefaultView;
                cmbProductCode.DisplayMemberPath = "product_id";
                cmbProductCode.Text = dt5.Rows[0].ItemArray[0].ToString();
                cmbProductCode.SelectedIndex = 0;
            }
            catch
            {
            }
        }
    }
}
