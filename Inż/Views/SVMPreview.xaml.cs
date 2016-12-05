using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Inż.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;
using Window = System.Windows.Window;

namespace Inż.Views
{
    /// <summary>
    /// Interaction logic for SVMPreview.xaml
    /// </summary>
    public partial class SVMPreview : Window
    {
        private readonly SVM _svm;
        private readonly List<ImageFeatures> _points;
        private List<ImageFeatures> _additionalPoints = new List<ImageFeatures>();

        public SVMPreview(SVM svm, List<ImageFeatures> points)
        {
            _svm = svm;
            _points = points;

            InitializeComponent();

            Draw();
        }

        


        private void Draw()
        {
            using (Mat retPlot = Mat.Zeros(300, 300, MatType.CV_8UC3))
            {
                //plot predictions
                for (int x = 0; x < 300; x++)
                {
                    for (int y = 0; y < 300; y++)
                    {
                        float[] sample = {x/300f, y/300f};
                        var sampleMat = new Mat(1, 2, MatType.CV_32FC1, sample);
                        int ret = (int) _svm.Predict(sampleMat);
                        var plotRect = new Rect(x, 300 - y, 1, 1);
                        if (ret == 1)
                            retPlot.Rectangle(plotRect, Scalar.Pink);
                        else if (ret == 2)
                            retPlot.Rectangle(plotRect, Scalar.DeepSkyBlue);
                    }
                }

                //plot points
                foreach (ImageFeatures point in _points)
                {
                    int x = (int) (point.SaturatedPixelsRatio * 300);
                    int y = (int) (300 - point.EdgePixelsRatio*300);
                    Scalar color = point.IsOccupied.Value ? Scalar.DarkRed : Scalar.DarkBlue;
                    retPlot.Circle(x, y, 2, color, -1);
                }

                foreach (var point in _additionalPoints)
                {
                    int x = (int)(point.SaturatedPixelsRatio * 300);
                    int y = (int)(300 - point.EdgePixelsRatio * 300);
                    Scalar color = Scalar.GreenYellow;
                    retPlot.Circle(x, y, 2, color, -1);
                }

                ImageControl.Source = retPlot.ToBitmapSource();
            }
        }

        public void PreviewPoints(List<ImageFeatures> points)
        {
            _additionalPoints = points;
            Draw();
        }
    }
}