using System;
using System.Collections.Generic;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Inż.Model;
using Inż.utils;

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

                    Sets[i - 1].Free.Org = CvInvoke.Imread($"Images/test{i}a.jpg", LoadImageType.AnyColor);
                    Sets[i - 1].Taken.Org = CvInvoke.Imread($"Images/test{i}b.jpg", LoadImageType.AnyColor);
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

            CvInvoke.CvtColor(subset.Org, subset.Gray, ColorConversion.Bgr2Gray);
            CvInvoke.Canny(subset.Gray, subset.Edges, 120, 150);
            subset.HoughSpaceP = CvInvoke.HoughLinesP(subset.Edges, 5, Math.PI/180, 10, 10, 10);

            DrawHouhgP(subset);
        }

        private static void DrawHouhgP(ImageSubset subset)
        {
            CvInvoke.CvtColor(subset.Edges, subset.HoughP, ColorConversion.Gray2Bgr);
            foreach (var lineSegment2D in subset.HoughSpaceP)
            {
                CvInvoke.Line(subset.HoughP, lineSegment2D.P1, lineSegment2D.P2, new MCvScalar(0, 255, 0));
            }
        }
    }
}