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
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private Point2f[] _points;
        private int[] _responses;

        public SVMPreview(SVM _svm, Point2f[] points, int[] responses)
        {
            this._svm = _svm;
            this._points = points;
            this._responses = responses;

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
                            retPlot.Rectangle(plotRect, Scalar.Red);
                        else if (ret == 2)
                            retPlot.Rectangle(plotRect, Scalar.GreenYellow);
                    }
                }

                //plot points
                for (int i = 0; i < _points.Length; i++)
                {
                    int x = (int) (_points[i].X * 300);
                    int y = (int) (300 - _points[i].Y*300);
                    int res = _responses[i];
                    Scalar color = (res == 1) ? Scalar.OrangeRed : Scalar.DarkOliveGreen;
                    retPlot.Circle(x, y, 2, color, -1);
                }

                ImageControl.Source = retPlot.ToBitmapSource();
            }
        }
    }
}