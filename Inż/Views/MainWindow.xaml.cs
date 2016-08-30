using System;
using System.Collections.Generic;
using System.Windows;
using Inż.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ImageSet> Sets { get; set; }
        private readonly bool _initializing = true;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _initializing = false;

                Sets = new List<ImageSet>();

                for (int i = 1; i <= 5; i++)
                {
                    Sets.Add(new ImageSet() {Free = new ImageSubset() {}, Taken = new ImageSubset() {}});


                    Sets[i - 1].Free.Org = new Mat($"Images/test{i}a.jpg", ImreadModes.AnyColor);
                    Sets[i - 1].Taken.Org = new Mat($"Images/test{i}b.jpg", ImreadModes.AnyColor);
                }

                SetSlider.Maximum = 5;

                OnSliderChange(null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_initializing) return;

            Recalculate();

            ImagePreview.Source =
                Sets[(int) SetSlider.Value - 1]
                    [(int) FtSlider.Value]
                    [(int) ImgTypeSlider.Value]
                    .ToBitmapSource();
        }

        private void Recalculate()
        {
            var subset = Sets[(int) SetSlider.Value - 1][(int) FtSlider.Value];

            Cv2.CvtColor(subset.Org, subset.Gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(subset.Gray, subset.Edges, 120, 150);
            var threshold = (int) (ASlider.Value * Math.Sqrt(subset.Gray.Height * subset.Gray.Width) / 255);
            subset.HoughSpace = Cv2.HoughLines(subset.Edges, 5, Math.PI/180, threshold, (int)BSlider.Value, (int)CSlider.Value);

            DrawHouhgP(subset);
        }

        private static void DrawHouhgP(ImageSubset subset)
        {
            Cv2.CvtColor(subset.Edges, subset.HoughP, ColorConversionCodes.GRAY2BGR);
            foreach (var lineSegment2D in subset.HoughSpace)
            {
                OpenCvSharp.Point pt1;
                OpenCvSharp.Point pt2;
                double a = Math.Cos(lineSegment2D.Theta), b = Math.Sin(lineSegment2D.Theta);
                double x0 = a* lineSegment2D.Rho, y0 = b* lineSegment2D.Rho;
                pt1.X = (int) Math.Round(x0 + 1000*(-b));
                pt1.Y = (int) Math.Round(y0 + 1000*(a));
                pt2.X = (int) Math.Round(x0 - 1000*(-b));
                pt2.Y = (int) Math.Round(y0 - 1000*(a));
                Cv2.Line(subset.HoughP, pt1, pt2, Scalar.Green);
            }
        }
    }
}