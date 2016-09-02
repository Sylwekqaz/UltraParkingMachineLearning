using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using ContourMaker.utils;

namespace ContourMaker
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                BackgroundBitmap = new Bitmap("Images/test1a.jpg");
                MaskBitmap = new Bitmap(BackgroundBitmap.Width, BackgroundBitmap.Height, PixelFormat.Format32bppArgb);
                TempBitmap = new Bitmap(BackgroundBitmap.Width, BackgroundBitmap.Height, PixelFormat.Format32bppArgb);

                BackgroundImage.Source = BackgroundBitmap.ToImageSource();
                MaskImage.Source = MaskBitmap.ToImageSource();
                TempImage.Source = TempBitmap.ToImageSource();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Bitmap BackgroundBitmap { get; set; }
        public Bitmap MaskBitmap { get; set; }
        public Bitmap TempBitmap { get; set; }

        private void TempImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}