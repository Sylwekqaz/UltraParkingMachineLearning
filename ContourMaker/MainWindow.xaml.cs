using System;
using System.Windows.Input;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

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
                ReinitializeMats();
                Redraw();
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private int[] Sizes { get; set; }

        public Mat BackgroundBitmap { get; set; }
        public Mat MaskBitmap { get; set; }
        public Mat TempBitmap { get; set; }


        private void ReinitializeMats()
        {
            BackgroundBitmap = new Mat("Images/test1a.jpg");
            Sizes = new[] {BackgroundBitmap.Size(0), BackgroundBitmap.Size(1)};
            MaskBitmap = new Mat(Sizes, BackgroundBitmap.Type(), new Scalar(0, 0, 0, 0));
            TempBitmap = new Mat(Sizes, BackgroundBitmap.Type(), new Scalar(0, 0, 0, 0));

            Cv2.Rectangle(MaskBitmap,new Rect(10,10,100,100),new Scalar(255,255,255,255),3 );
        }

        private void Redraw()
        {
            var tempMat = new Mat();
            var tempMat2 = new Mat();
            Cv2.Add(BackgroundBitmap,MaskBitmap,tempMat);
            Cv2.Add(tempMat,TempBitmap, tempMat2);

            PreviewImage.Source = tempMat2.ToBitmapSource();
        }

        private void TempImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}