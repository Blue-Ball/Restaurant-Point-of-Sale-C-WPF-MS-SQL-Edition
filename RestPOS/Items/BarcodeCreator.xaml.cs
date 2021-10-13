using System;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace PosCube.Items
{
    /// <summary>
    /// Interaction logic for BarcodeCreator.xaml
    /// </summary>
    public partial class BarcodeCreator : Window
    {
        public BarcodeCreator()
        {
            InitializeComponent();

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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            RefreshBarcode();
        }

        public void RefreshBarcode()
        {
            try
            {
                string strCode = cmbProductCode.Text.Trim();

                // int nFontSize = int.Parse(txtFontSize.Text);
                System.Drawing.Color cForeColor = System.Drawing.Color.FromArgb(colorPickerFore.SelectedColor.Value.A, colorPickerFore.SelectedColor.Value.R, colorPickerFore.SelectedColor.Value.G, colorPickerFore.SelectedColor.Value.B);
                System.Drawing.Color cBackColor = System.Drawing.Color.FromArgb(colorPickerBack.SelectedColor.Value.A, colorPickerBack.SelectedColor.Value.R, colorPickerBack.SelectedColor.Value.G, colorPickerBack.SelectedColor.Value.B);

                var barcodeWriter = new BarcodeWriter
                {
                    Renderer = new BitmapRenderer
                    {
                        Background =  cBackColor,
                        Foreground = cForeColor,
                        TextFont = new System.Drawing.Font("Times New Roman", 12)
                    },
                    Format = BarcodeFormat.EAN_13,
                    Options = new EncodingOptions
                    {
                        Height = 140,
                        Width = 200,
                        PureBarcode = chkShowBarcodeText.IsChecked == true? false : true,
                        Margin = 1
                    }
                };

                System.Drawing.Bitmap bmp = barcodeWriter.Write(strCode);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                imgBarcode.Height = 140;
                imgBarcode.Source = bi;
            }
            catch// (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private void chkShowBarcodeText_Checked(object sender, RoutedEventArgs e)
        {
            RefreshBarcode();
        }

        private void colorPickerBack_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            RefreshBarcode();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(gridPrint);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(gridPrint);
                context.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderTarget.Render(visual);
            PngBitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (Stream stm = File.Create(@"print_barcode.png"))
            {
                bitmapEncoder.Save(stm);
            }

            PrinterSettings settings = new PrinterSettings();
            PrintDocument pd = new PrintDocument();
            PrintDialog printDialog = new PrintDialog();
            var printers = new LocalPrintServer().GetPrintQueues();
            var selectedPrinter = printers.FirstOrDefault(p => p.Name == settings.PrinterName); //Replacement 
            // if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintQueue = selectedPrinter;
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                pd.Print();
            }
        }

        int pageNumber = 1;
        int numberOfPages = 1;
        int i = 0;

        private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile("print_barcode.png");
            //e.Graphics.DrawImage(image, 0, 0);

            numberOfPages = int.Parse(txtIntUpDown.Text);
            int x = 10;
            int y = 10;

            for (int j = 0; j <= 3; j++)
            {

                for (int m = 0; m <= 6; m++)
                {
                    System.Drawing.Rectangle drawRect = new System.Drawing.Rectangle(x, y, image.Width, image.Height);
                    e.Graphics.DrawImage(image, drawRect);
                    //x = x + EaN13Barcode1.Width
                    y = y + image.Height;
                }
                x = x + image.Width;
                y = 10;
            }


            if ((pageNumber < numberOfPages))
            {
                e.HasMorePages = true;
                i = i + 1;
                pageNumber = pageNumber + 1;
            }
            else
            {
                e.HasMorePages = false;
            }
        }
    }
}
