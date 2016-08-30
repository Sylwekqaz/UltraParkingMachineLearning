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
            subset.HoughSpace = Cv2.HoughLinesP(subset.Edges, 5, Math.PI/180, (int) ASlider.Value, (int)BSlider.Value, (int)CSlider.Value);

            DrawHouhgP(subset);
        }

        private static void DrawHouhgP(ImageSubset subset)
        {
            Cv2.CvtColor(subset.Edges, subset.HoughP, ColorConversionCodes.GRAY2BGR);
            foreach (var lineSegment2D in subset.HoughSpace)
            {
                Cv2.Line(subset.HoughP, lineSegment2D.P1, lineSegment2D.P2, Scalar.Green);
            }
        }
    }
}